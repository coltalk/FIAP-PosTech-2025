# TcCatalog API

API em **.NET 8** para catálogo de jogos e gerenciamento de biblioteca de usuários, com autenticação JWT, autorização por papéis e persistência via Entity Framework Core.

## 📦 Tecnologias

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- SQL Server
- RabbitMQ (mensageria de eventos)
- JWT Bearer Authentication
- Swagger / OpenAPI
- Serilog (logs estruturados)
- Docker / Docker Compose

## 🧱 Estrutura da solução

- `TcCatalog.Api`: camada de apresentação (controllers, middlewares, autenticação, Swagger)
- `TcCatalog.Application`: regras de aplicação, DTOs e serviços
- `TcCatalog.Domain`: entidades e enums de domínio
- `TcCatalog.Infra`: contexto EF Core, mapeamentos, repositórios e migrations

## ✅ Pré-requisitos

- .NET SDK 8.0+
- SQL Server (local ou remoto)
- RabbitMQ (local ou remoto)
- Docker Desktop (opcional, para execução em container)

## ⚙️ Configuração

1. Ajuste a conexão com banco no arquivo `TcCatalog.Api/appsettings.json` em `ConnectionStrings:DefaultConnection`.
2. Revise as configurações JWT em `Jwt` (`Issuer`, `Audience`, `Secret`, `ExpirationMinutes`).
3. Configure a seção `RabbitMq` em `TcCatalog.Api/appsettings.json` (`HostName`, `Port`, `UserName`, `Password`, `VirtualHost` e nomes das filas).
4. Em ambiente local, prefira sobrescrever segredos por variáveis de ambiente.

## ▶️ Como executar

No diretório raiz do repositório:

```bash
dotnet restore
dotnet build
dotnet run --project TcCatalog.Api
```

Por padrão, a API sobe com Swagger em ambiente de desenvolvimento em `http://localhost:5001/swagger`.

## 🗄️ Banco de dados (migrations)

Para aplicar as migrations existentes:

```bash
dotnet tool restore
dotnet ef database update --project TcCatalog.Infra --startup-project TcCatalog.Api
```

Se precisar criar uma nova migration:

```bash
dotnet ef migrations add NomeDaMigration --project TcCatalog.Infra --startup-project TcCatalog.Api
```

## 🔐 Autenticação e autorização

A API usa **Bearer Token (JWT)**.

- Endpoints de jogos (`/api/jogos`) exigem autenticação.
- Operações administrativas (criar/editar/remover jogo e administração de biblioteca) exigem papel `Admin`.
- Endpoints de biblioteca do usuário usam o `UserId` do token autenticado.

No Swagger, clique em **Authorize** e informe:

```text
Bearer {seu_token}
```

## 🧭 Principais endpoints

### Jogos

- `GET /api/jogos`
- `GET /api/jogos/{id}`
- `POST /api/jogos` (Admin)
- `PUT /api/jogos/{id}` (Admin)
- `DELETE /api/jogos/{id}` (Admin)
- `POST /api/jogos/{id}/promocao` (Admin)

### Biblioteca do usuário autenticado

- `GET /api/user/biblioteca/me`
- Retorna itens no formato `BibliotecaJogoDto` (dados do jogo + informações da biblioteca):
  - `status` (int): código do status (`0` EmAberto, `1` Pendente, `2` Aprovado, `3` Reprovado)
  - `statusDescricao` (string): descrição amigável do status
  - `payId` (Guid?): identificador do pagamento quando disponível

Exemplo de resposta:

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "descricao": "Jogo Exemplo",
    "genero": "Aventura",
    "preco": 99.90,
    "dataCadastro": "2026-03-07T12:00:00Z",
    "status": 2,
    "statusDescricao": "Pagamento APROVADO",
    "payId": "b3a6fa5b-95f5-4cb2-9f0a-9fd60ab8d2d1"
  }
]
```

### Administração de biblioteca (Admin)

- `POST /api/admin/biblioteca`
- `GET /api/admin/biblioteca?userId={guid}`
- `DELETE /api/admin/biblioteca?userId={guid}&jogoId={guid}`
- `DELETE /api/admin/biblioteca/todos?userId={guid}`
- O endpoint `GET` também retorna `BibliotecaJogoDto` com `status`, `statusDescricao` e `payId`.

### Diagnóstico do usuário autenticado

- `GET /api/UserId/me`

## 📣 Mensageria de eventos (RabbitMQ)

- Ao vincular um jogo para o usuário, a API publica `OrderPlacedEvent` na fila.
- O processamento de pagamento deve publicar `PaymentProcessedEvent` na fila.
- A API possui um consumidor em background que lê `PaymentProcessedEvent` e atualiza o status da biblioteca.

## 💳 Processamento de pagamento

O evento de pagamento processado usa o contrato:

```csharp
PaymentProcessedEvent(Guid UserId, Guid JogoId, Guid PayId, int Status)
```

Regras para `Status` no processamento:

- Apenas `2` (**Aprovado**) e `3` (**Reprovado**) são aceitos.
- Qualquer outro valor é rejeitado com erro de validação.

## 📝 Logs

Os logs são gravados em:

- `logs/info-*.json` para eventos até nível `Information`
- `logs/error-*.json` para eventos de erro

Além dos arquivos, a aplicação também escreve logs no console.

## 🐳 Docker

Suba o RabbitMQ separado (fora do `docker-compose.yml` da API):

```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

Painel RabbitMQ (opcional):

```text
http://localhost:15672
```

Credenciais padrão: `guest / guest`.

Para executar os containers:

```bash
docker compose up -d --build
```

Para derrubar os containers:

```bash
docker compose down
```

Swagger disponível em:

```text
http://localhost:5001/swagger
```

### Observações sobre Docker

- o `docker-compose.yml` publica a API na porta `5001`
- a connection string no compose usa `host.docker.internal` para acessar o SQL Server da máquina host
- antes de subir o container, garanta que o SQL Server esteja acessível em `host.docker.internal:1433`

## ☸️ Kubernetes

Os manifestos estão na pasta `k8s/` na raiz do projeto e incluem:

- `k8s/deployment.yaml`: cria a aplicação com `Deployment`
- `k8s/configmap.yaml`: armazena configurações não sensíveis
- `k8s/secret.yaml`: armazena segredos e credenciais
- `k8s/service.yaml`: expõe a API dentro do cluster
- `k8s/kustomization.yaml`: aplica todos os recursos em conjunto

Antes do deploy, revise principalmente os valores em `k8s/secret.yaml`.

Para aplicar os recursos:

```bash
kubectl apply -k k8s/
```

Para acessar a API localmente via port-forward:

```bash
kubectl port-forward service/tccatalog-api 5001:80
```

Depois disso, o Swagger pode ser acessado em:

```text
http://localhost:5001/swagger
```

## 👨🏻‍🎓 Alunos

- Clovis Ribeiro Ramos - rm369652
- Matheus Machado Pinheiro do Valle - rm369919
- Pedro Delgado Henriques -rm369869
- Jose Mauro Morais Mani -rm
