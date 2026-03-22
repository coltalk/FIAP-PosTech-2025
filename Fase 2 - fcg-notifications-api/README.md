# FCG Notifications API

Microservico de notificacoes do **FIAP Cloud Games (FCG)**, desenvolvido como parte da **Fase 2 do Tech Challenge** da pos-graduacao FIAP.

Consome eventos via **RabbitMQ** (com MassTransit) disparados por outros microservicos do ecossistema (UsersAPI e PaymentsAPI) e simula o envio de e-mails via structured logging no console.

---

## Indice

- [Visao Geral](#visao-geral)
- [Arquitetura](#arquitetura)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Eventos Consumidos](#eventos-consumidos)
- [Endpoints HTTP](#endpoints-http)
- [Middlewares](#middlewares)
- [Variaveis de Ambiente](#variaveis-de-ambiente)
- [Como Executar](#como-executar)
- [Como Testar](#como-testar)
- [Decisoes Tecnicas](#decisoes-tecnicas)
- [Tecnologias](#tecnologias)

---

## Visao Geral

O NotificationsAPI e um microservico **reativo** — ele nao inicia acoes, apenas **reage a eventos** disparados por outros microservicos do ecossistema FCG.

### Fluxo de Funcionamento

```
┌──────────┐                    ┌────────────┐                    ┌─────────────────────┐
│ UsersAPI  │ ── publish ──────>│            │                    │                     │
└──────────┘  UserCreatedEvent  │            │ ── consume ──────> │  NotificationsAPI   │
                                │  RabbitMQ  │                    │                     │
┌────────────┐                  │            │                    │  [LOG] Email         │
│ PaymentsAPI│ ── publish ─────>│            │ ── consume ──────> │  simulado            │
└────────────┘  PaymentProcessed└────────────┘                    └─────────────────────┘
                Event
```

**Fluxo de Cadastro de Usuario:**
1. UsersAPI cria um usuario e publica `UserCreatedEvent` no RabbitMQ
2. NotificationsAPI consome o evento da fila e simula e-mail de **boas-vindas**

**Fluxo de Compra de Jogo:**
1. PaymentsAPI processa um pagamento e publica `PaymentProcessedEvent` no RabbitMQ
2. NotificationsAPI consome o evento da fila e verifica o status:
   - **Status 2 (Aprovado)** → simula e-mail de **confirmacao de compra**
   - **Status 3 (Reprovado)** → simula **aviso de pagamento reprovado**
   - **Outros status** → registra o evento no log

---

## Arquitetura

O projeto segue **Clean Architecture** com 3 camadas:

```
┌─────────────────────────────────────────────┐
│              Notifications.Api               │  ← Controllers, Middlewares, Config, MassTransit setup
├─────────────────────────────────────────────┤
│          Notifications.Application           │  ← Events, Interfaces, Services
├─────────────────────────────────────────────┤
│             Notifications.Infra              │  ← Consumers RabbitMQ (MassTransit)
└─────────────────────────────────────────────┘
```

| Camada | Responsabilidade |
|---|---|
| **Notifications.Api** | Controllers (endpoints HTTP), Middlewares (CorrelationId, ErrorHandling), configuracao (Program.cs, Serilog, Swagger, MassTransit) |
| **Notifications.Application** | Eventos (`UserCreatedEvent`, `PaymentProcessedEvent`), interface `INotificationService`, servico `NotificationService` |
| **Notifications.Infra** | Consumers do RabbitMQ via MassTransit (`UserCreatedConsumer`, `PaymentProcessedConsumer`) |

---

## Estrutura do Projeto

```
fcg-notifications-api/
├── Notifications.sln
├── docker-compose.yml                        # API + RabbitMQ
├── README.md
│
├── Notifications.Api/
│   ├── Program.cs                            # Entry point, Serilog, MassTransit config
│   ├── Dockerfile                            # Build multi-stage
│   ├── appsettings.json                      # Config padrao (RabbitMQ host, credentials)
│   ├── Controllers/
│   │   ├── UserEventsController.cs           # POST /user-created (HTTP alternativo)
│   │   └── PaymentEventsController.cs        # POST /payment-processed (HTTP alternativo)
│   └── Middlewares/
│       ├── CorrelationIdMiddleware.cs         # Rastreamento distribuido
│       └── ErrorHandlingMiddleware.cs         # Tratamento global de erros
│
├── Notifications.Application/
│   ├── Events/
│   │   ├── UserCreatedEvent.cs               # Record: UserId, Name, Email
│   │   └── PaymentProcessedEvent.cs          # Record: UserId, JogoId, PayId, Status
│   ├── Interfaces/
│   │   └── INotificationService.cs           # Contrato do servico
│   └── Services/
│       └── NotificationService.cs            # Logica de notificacao (log simulado)
│
├── Notifications.Infra/
│   └── Consumers/
│       ├── UserCreatedConsumer.cs             # Consome UserCreatedEvent do RabbitMQ
│       └── PaymentProcessedConsumer.cs        # Consome PaymentProcessedEvent do RabbitMQ
│
└── k8s/
    ├── configmap.yaml                        # ASPNETCORE_ENVIRONMENT, RabbitMQ host
    ├── deployment.yaml                       # 1 replica, limites CPU/memoria
    ├── secret.yaml                           # Credenciais RabbitMQ (base64)
    └── service.yaml                          # ClusterIP porta 80 → 8080
```

---

## Eventos Consumidos

Os eventos sao consumidos automaticamente via **filas do RabbitMQ** usando MassTransit. Os endpoints HTTP tambem estao disponiveis como alternativa para testes manuais.

### UserCreatedEvent

| Campo | Tipo | Descricao |
|---|---|---|
| `userId` | `Guid` | ID do usuario criado |
| `name` | `string` | Nome do usuario |
| `email` | `string` | E-mail do usuario |

**Acao:** Simula envio de e-mail de boas-vindas ao FCG.

### PaymentProcessedEvent

| Campo | Tipo | Descricao |
|---|---|---|
| `userId` | `Guid` | ID do usuario |
| `jogoId` | `Guid` | ID do jogo comprado |
| `payId` | `Guid` | ID do pagamento |
| `status` | `int` | Status do pagamento (2 = Aprovado, 3 = Reprovado) |

**Acao:**

| Status | Acao |
|---|---|
| 2 (Aprovado) | Simula e-mail de confirmacao de compra |
| 3 (Reprovado) | Simula aviso de pagamento reprovado |
| Outros | Registra o evento no log |

---

## Endpoints HTTP

Os endpoints HTTP servem como **alternativa** para testes manuais. Em producao, os eventos chegam via RabbitMQ.

| Metodo | Rota | Descricao | Response |
|---|---|---|---|
| `POST` | `/api/notifications/user-created` | Recebe evento de criacao de usuario | `204 No Content` |
| `POST` | `/api/notifications/payment-processed` | Recebe evento de pagamento processado | `204 No Content` |

Swagger UI disponivel em `/swagger` quando a aplicacao esta rodando.

---

## Middlewares

### CorrelationIdMiddleware

Garante rastreamento distribuido entre microservicos. Se a request tiver o header `X-Correlation-Id`, ele e propagado; caso contrario, um novo GUID e gerado. O valor e incluido na response e em todos os logs via Serilog `LogContext`.

### ErrorHandlingMiddleware

Captura excecoes nao tratadas e retorna respostas padronizadas em JSON com `traceId`, `status`, `title`, `detail` e `timestamp`. Mapeia tipos de excecao para HTTP status codes adequados (400, 404, 409, 401, 500).

---

## Variaveis de Ambiente

| Variavel | Descricao | Valor Padrao |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | Ambiente da aplicacao | `Development` |
| `RabbitMQ__Host` | Hostname do broker RabbitMQ | `localhost` |
| `RabbitMQ__Username` | Usuario do RabbitMQ | `guest` |
| `RabbitMQ__Password` | Senha do RabbitMQ | `guest` |

> No Kubernetes, `RabbitMQ__Host` deve ser o nome do Service do RabbitMQ (ex: `rabbitmq-service`).

---

## Como Executar

### Com Docker (recomendado)

```bash
docker-compose up --build
```

Isso sobe **RabbitMQ + NotificationsAPI** juntos. A API aguarda o RabbitMQ ficar saudavel antes de iniciar.

| Servico | URL |
|---|---|
| NotificationsAPI | http://localhost:5003 |
| Swagger UI | http://localhost:5003/swagger |
| RabbitMQ Management | http://localhost:15672 (guest/guest) |

### Localmente (requer RabbitMQ rodando)

```bash
# 1. Subir RabbitMQ (se nao tiver rodando)
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management

# 2. Executar a API
dotnet restore
dotnet build
dotnet run --project Notifications.Api
```

A API estara disponivel em `http://localhost:5000`.
Swagger UI: `http://localhost:5000/swagger`

### Kubernetes

```bash
kubectl apply -f k8s/
```

Os manifests incluem: `Deployment` (1 replica, limites de CPU/memoria), `Service` (ClusterIP na porta 80 → 8080), `ConfigMap` (host do RabbitMQ) e `Secret` (credenciais do RabbitMQ).

---

## Como Testar

### Passo a passo completo

Siga os passos abaixo para testar toda a integracao com RabbitMQ do zero:

**Passo 1 — Abrir o Docker Desktop**

Abra o **Docker Desktop** no Windows e aguarde ele iniciar completamente (o icone fica verde na barra de tarefas).

**Passo 2 — Subir os containers**

Abra um terminal na pasta do projeto e execute:

```bash
docker-compose up --build
```

Aguarde ate ver nos logs que o MassTransit conectou ao RabbitMQ (mensagem `Bus started` ou `Connected`). Isso pode levar alguns segundos na primeira vez pois o Docker vai baixar as imagens.

**Passo 3 — Testar evento de usuario criado (e-mail de boas-vindas)**

Abra um **segundo terminal** (sem fechar o primeiro) e execute:

```bash
curl -X POST http://localhost:5003/api/notifications/user-created -H "Content-Type: application/json" -d "{\"userId\":\"550e8400-e29b-41d4-a716-446655440000\",\"name\":\"Pedro\",\"email\":\"pedro@test.com\"}"
```

Volte no primeiro terminal e procure nos logs:
```
[EMAIL SIMULADO] Bem-vindo ao FCG! | Para: pedro@test.com | Nome: Pedro | UserId: 550e8400-...
```

**Passo 4 — Testar evento de pagamento aprovado**

No segundo terminal, execute:

```bash
curl -X POST http://localhost:5003/api/notifications/payment-processed -H "Content-Type: application/json" -d "{\"userId\":\"550e8400-e29b-41d4-a716-446655440000\",\"jogoId\":\"660e8400-e29b-41d4-a716-446655440001\",\"payId\":\"770e8400-e29b-41d4-a716-446655440002\",\"status\":2}"
```

No primeiro terminal, procure nos logs:
```
[EMAIL SIMULADO] Pagamento aprovado! | UserId: 550e8400-... | JogoId: 660e8400-... | PayId: 770e8400-...
```

**Passo 5 — Testar evento de pagamento rejeitado**

No segundo terminal, execute:

```bash
curl -X POST http://localhost:5003/api/notifications/payment-processed -H "Content-Type: application/json" -d "{\"userId\":\"550e8400-e29b-41d4-a716-446655440000\",\"jogoId\":\"660e8400-e29b-41d4-a716-446655440001\",\"payId\":\"770e8400-e29b-41d4-a716-446655440002\",\"status\":3}"
```

No primeiro terminal, procure nos logs:
```
[EMAIL SIMULADO] Pagamento reprovado. | UserId: 550e8400-... | JogoId: 660e8400-... | PayId: 770e8400-...
```

**Passo 6 — Verificar o RabbitMQ Management (opcional)**

1. Abra no navegador: http://localhost:15672
2. Login: `guest` | Senha: `guest`
3. Clique na aba **Queues** para ver as filas criadas pelo MassTransit

**Passo 7 — Parar os containers**

No terminal do docker-compose, pressione `Ctrl+C` e depois:

```bash
docker-compose down
```

> **Dica:** Voce tambem pode testar pelo **Swagger UI** em http://localhost:5003/swagger — basta clicar no endpoint, clicar em **Try it out**, colar o JSON e clicar em **Execute**.

---

### 1. Testar via Swagger (endpoints HTTP)

1. Acesse o Swagger UI: http://localhost:5003/swagger (Docker) ou http://localhost:5000/swagger (local)
2. Clique em um endpoint → **Try it out** → cole o JSON → **Execute**
3. Verifique o response **204 No Content**
4. Verifique os logs no terminal (e-mail simulado)

**Teste de boas-vindas (user-created):**

```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Joao Silva",
  "email": "joao@email.com"
}
```

**Teste de pagamento aprovado (status 2):**

```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "jogoId": "1fa85f64-5717-4562-b3fc-2c963f66afa6",
  "payId": "2fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": 2
}
```

**Teste de pagamento reprovado (status 3):**

```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "jogoId": "1fa85f64-5717-4562-b3fc-2c963f66afa6",
  "payId": "2fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": 3
}
```

### 2. Testar via RabbitMQ Management (mensageria)

1. Acesse http://localhost:15672 (login: guest / guest)
2. Va na aba **Queues** — voce vera as filas criadas pelo MassTransit:
   - `user-created-event`
   - `payment-processed-event`
3. Clique em uma fila → **Publish message**
4. Em **Properties**, adicione: `content_type = application/json`
5. No **Payload**, cole o JSON do evento (mesmo formato acima)
6. Clique **Publish message**
7. Verifique nos logs do NotificationsAPI que o evento foi consumido

### 3. Testar via curl (terminal)

```bash
# Boas-vindas
curl -X POST http://localhost:5003/api/notifications/user-created \
  -H "Content-Type: application/json" \
  -d '{"userId":"3fa85f64-5717-4562-b3fc-2c963f66afa6","name":"Joao Silva","email":"joao@email.com"}'

# Pagamento aprovado
curl -X POST http://localhost:5003/api/notifications/payment-processed \
  -H "Content-Type: application/json" \
  -d '{"userId":"3fa85f64-5717-4562-b3fc-2c963f66afa6","jogoId":"1fa85f64-5717-4562-b3fc-2c963f66afa6","payId":"2fa85f64-5717-4562-b3fc-2c963f66afa6","status":2}'

# Pagamento reprovado
curl -X POST http://localhost:5003/api/notifications/payment-processed \
  -H "Content-Type: application/json" \
  -d '{"userId":"3fa85f64-5717-4562-b3fc-2c963f66afa6","jogoId":"1fa85f64-5717-4562-b3fc-2c963f66afa6","payId":"2fa85f64-5717-4562-b3fc-2c963f66afa6","status":3}'
```

### Logs esperados

**Boas-vindas:**
```
[EMAIL SIMULADO] Bem-vindo ao FCG! | Para: joao@email.com | Nome: Joao Silva | UserId: 3fa85f64-...
```

**Pagamento aprovado:**
```
[EMAIL SIMULADO] Pagamento aprovado! | UserId: 3fa85f64-... | JogoId: 1fa85f64-... | PayId: 2fa85f64-...
```

**Pagamento reprovado:**
```
[EMAIL SIMULADO] Pagamento reprovado. | UserId: 3fa85f64-... | JogoId: 1fa85f64-... | PayId: 2fa85f64-...
```

---

## Decisoes Tecnicas

### Mensageria com MassTransit + RabbitMQ

O Tech Challenge exige comunicacao assincrona via mensageria. Utilizamos **MassTransit** como abstraction layer sobre o **RabbitMQ**, o que permite:

- Consumers tipados (`IConsumer<T>`) com injecao de dependencia
- Configuracao automatica de filas e exchanges
- Retry e error handling built-in
- Futuramente trocar RabbitMQ por Kafka sem alterar os consumers

### Por que simular e-mails via log?

O Tech Challenge exige decomposicao em microservicos, mas nao requer integracao real com provedores de e-mail (SendGrid, SES, etc.). O envio e **simulado via structured logging com Serilog**, o que permite:

- Validar que os eventos estao sendo recebidos e processados corretamente
- Visualizar os "e-mails" no console e em arquivos de log (formato JSON)
- Futuramente trocar o log por um envio real sem alterar a interface (`INotificationService`)

### Por que nao tem banco de dados?

Diferente dos outros microservicos (Catalog, Users, Payments), o NotificationsAPI **nao precisa persistir dados**. Ele recebe um evento, processa (loga o e-mail simulado) e responde. Se no futuro for necessario manter historico de notificacoes, basta adicionar um repositorio na camada Infra.

### Por que os endpoints sao AllowAnonymous?

Este servico recebe chamadas **internas** de outros microservicos, nao de usuarios finais. Em producao, a seguranca e garantida pela rede interna do cluster Kubernetes — o Service e do tipo `ClusterIP`, acessivel apenas dentro do cluster.

### Logging estruturado

O Serilog esta configurado com:

- **Console** em formato JSON compacto (`CompactJsonFormatter`)
- **Arquivo de info** (`logs/info-{data}.json`) com retencao de 7 dias
- **Arquivo de erros** (`logs/error-{data}.json`) com retencao de 30 dias
- **Enrichers**: `MachineName`, `ThreadId`, `EnvironmentName`, `CorrelationId`

---

## Tecnologias

| Tecnologia | Uso |
|---|---|
| **.NET 8** | Framework da aplicacao |
| **MassTransit** | Abstraction layer para mensageria |
| **RabbitMQ** | Message broker para comunicacao assincrona |
| **Serilog** | Structured logging (JSON) |
| **Swagger / OpenAPI** | Documentacao interativa dos endpoints |
| **Docker** | Containerizacao (build multi-stage) |
| **Kubernetes** | Orquestracao (Deployment, Service, ConfigMap, Secret) |
