# 👤 UsersAPI — FCG FIAP Cloud Games

Microsserviço responsável pelo **cadastro, autenticação e gerenciamento de usuários** da plataforma FIAP Cloud Games.

---

## 🧱 Tecnologias

- .NET 9
- SQL Server (dados relacionais)
- MongoDB (logs via Serilog)
- Redis (cache distribuído)
- RabbitMQ (publicação de eventos)
- JWT Bearer Authentication
- Kubernetes (Azure AKS) — orquestração em produção
- Azure Container Registry (ACR) — registro de imagens
- Kong API Gateway — exposição externa do serviço
- GitHub Actions — pipeline de CI/CD

---

## 📡 Endpoints

### 🔐 Authentication — `/api/Authentication`

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| `POST` | `/api/Authentication/login/` | Login do usuário | ❌ Público |

**POST /login/**
Autentica o usuário e retorna um JWT Bearer Token.

```json
// Request
{
  "usuario": "string",
  "senha": "string"
}

// Response 200
{
  "token": "eyJhbGci..."
}

// Response 401
Usuário ou senha inválidos.
```

---

### 👥 Usuários — `/api/Usuarios`

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| `POST` | `/api/Usuarios/Cadastrar/` | Cadastra novo usuário | ❌ Público |
| `GET` | `/api/Usuarios/BuscarPorId/{id}` | Busca usuário por ID | ✅ usuario |
| `PUT` | `/api/Usuarios/AlterarSenha/` | Altera senha do usuário | ✅ usuario |
| `PUT` | `/api/Usuarios/AlterarStatus/` | Ativa ou desativa usuário | ✅ usuario |

---

### 👤 Perfil — `/api/usuarios/{usuarioId}/perfil`

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| `GET` | `/BuscarPorUsuarioId/` | Busca perfil do usuário | ✅ usuario |
| `POST` | `/Cadastrar/` | Cadastra perfil | ✅ usuario |
| `PUT` | `/Atualizar/{id}` | Atualiza perfil | ✅ usuario |
| `DELETE` | `/Deletar/{id}` | Remove perfil | ✅ usuario |

---

### 📞 Contatos — `/api/usuarios/{usuarioId}/contatos`

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| `GET` | `/BuscarPorUsuarioId/` | Lista contatos do usuário | ✅ usuario |
| `POST` | `/Cadastrar/` | Cadastra contato | ✅ usuario |
| `PUT` | `/Atualizar/{id}` | Atualiza contato | ✅ usuario |
| `DELETE` | `/Deletar/{id}` | Remove contato | ✅ usuario |

---

### 🏠 Endereços — `/api/usuarios/{usuarioId}/enderecos`

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| `GET` | `/BuscarPorUsuarioId/` | Lista endereços do usuário | ✅ usuario |
| `POST` | `/Cadastrar/` | Cadastra endereço | ✅ usuario |
| `PUT` | `/Atualizar/{id}` | Atualiza endereço | ✅ usuario |
| `DELETE` | `/Deletar/{id}` | Remove endereço | ✅ usuario |

---

### 🎭 Roles — `/api/Roles`

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| `POST` | `/Cadastrar/` | Cadastra nova role | ✅ administrador |
| `GET` | `/ListarRoles/` | Lista todas as roles | ✅ administrador |
| `PUT` | `/Atualizar/{id}` | Atualiza role | ✅ administrador |

---

### 🔗 UsuarioRole — `/api/UsuarioRole`

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| `GET` | `/ListarRolesPorUsuario/` | Lista roles do usuário | ✅ usuario |
| `PUT` | `/AlterarRoleUsuario` | Altera role do usuário | ✅ usuario |

---

## 📨 Eventos Publicados

| Exchange | Tipo | Quando |
|---|---|---|
| `user-created-exchange` | Fanout | Ao cadastrar novo usuário |

A NotificationsAPI consome esse evento e envia um e-mail de boas-vindas.

---

## 🔐 Autenticação

Esta API utiliza **JWT Bearer Token**. Para acessar endpoints protegidos:

1. Cadastre um usuário via `POST /api/Usuarios/Cadastrar/`
2. Faça login via `POST /api/Authentication/login/`
3. Copie o token retornado
4. No Swagger, clique em **Authorize** 🔒 e insira `Bearer <token>`

---

## 🗃️ Banco de Dados

| Configuração | Valor |
|---|---|
| Connection String | `MS_UserAPI` |
| Database | `MS_UsersAPI` |

---

## 🐳 Rodando Localmente (Docker Compose)

Este serviço faz parte da orquestração central. Para rodar o ambiente completo:

```bash
# Clone todos os repositórios na mesma pasta pai
git clone https://github.com/pablosdlima/OrchestrationApi
git clone https://github.com/marciotorquato/UsersAPI

# Suba o ambiente
cd OrchestrationAPI
docker compose up --build
```

Swagger disponível em: `http://localhost:5001/swagger`

> ℹ️ O ambiente completo inclui SQL Server, MongoDB, RabbitMQ e os 4 microsserviços. Consulte o repositório [OrchestrationAPI](https://github.com/pablosdlima/OrchestrationApi) para mais detalhes.

---

## ☸️ Rodando com Kubernetes

Este serviço roda em Kubernetes de duas formas: **localmente** (Docker Desktop, para desenvolvimento) e em **produção** (Azure AKS, via pipeline de CI/CD). A estrutura dos manifestos é a mesma nos dois casos — o que muda é de onde vem a imagem e como o serviço é exposto.

### Estrutura dos manifestos

```
UsersAPI/
└── k8s/
    ├── configmap.yaml        ← variáveis não sensíveis (inclui ConnectionStrings__Redis)
    ├── secret.yaml           ← variáveis sensíveis (Base64) — nunca commitar com valores reais
    ├── deployment.yaml       ← gerencia os Pods (2 réplicas)
    ├── service.yaml          ← expõe o serviço internamente (ClusterIP)
    ├── redis-deployment.yaml ← cache distribuído (Redis) usado pelo UsersAPI
    └── redis-service.yaml    ← Service interno (ClusterIP) do Redis
```

> 🔒 **Sobre o `secret.yaml`:** o arquivo do repositório deve conter apenas placeholders. As credenciais reais (senha do SQL Server, senha do RabbitMQ, chave JWT, connection string do MongoDB) nunca devem ser commitadas — em produção elas são gerenciadas via GitHub Secrets/Azure Key Vault e injetadas em tempo de execução.

### Opção A — Rodando localmente (Docker Desktop)

**Pré-requisitos:**
- Docker Desktop com **Kubernetes habilitado**
- `kubectl` disponível no terminal
- Infraestrutura já aplicada via OrchestrationAPI

**1. Aplicar os manifestos**

```bash
# Na raiz do repositório UsersAPI
kubectl apply -f k8s/
```

**2. Verificar se está rodando**

```bash
kubectl get pods
kubectl get services
```

**3. Acessar o Swagger**

Como o `service.yaml` é do tipo `ClusterIP`, use port-forward para acessar localmente:

```bash
kubectl port-forward service/users-api-service 5001:80
```

Acesse: `http://localhost:5001/swagger`

**Parar o serviço**

```bash
kubectl delete -f k8s/
```

### Opção B — Produção (Azure AKS)

Em produção, o serviço roda em um cluster **AKS** e segue este fluxo:

- **Imagem:** publicada no **Azure Container Registry** (`fcgacr.azurecr.io/fcg-users-api`) automaticamente pela pipeline de CI/CD a cada push na `main` — nunca é feito build/push manual.
- **Exposição externa:** o `users-api-service` é `ClusterIP` (não acessível diretamente de fora do cluster). O acesso externo é feito através do **Kong API Gateway** (`kong-proxy`, exposto como `LoadBalancer`), que roteia as requisições até o serviço.
- **Atualização:** o deploy é feito via `kubectl set image`, atualizando a imagem do `Deployment` com a nova tag gerada pela pipeline, em **Rolling Update** (sem downtime).
- **Credenciais do cluster:** obtidas via `az aks get-credentials`, autenticado por OIDC (sem uso de senha/token estático).

---

## 🔴 Cache com Redis

### O que foi implementado

Adicionamos cache distribuído com **Redis** no microsserviço UsersAPI para reduzir consultas ao banco de dados SQL Server em leituras repetidas de usuários.

**Arquivos criados/alterados:**

| Arquivo | O que faz |
|---|---|
| `UsersAPI.Application/Interfaces/ICacheService.cs` | Interface com os métodos `GetAsync`, `SetAsync` e `RemoveAsync` |
| `UsersAPI.Application/Services/RedisCacheService.cs` | Implementação usando `IDistributedCache` do .NET com serialização JSON |
| `UsersAPI.IoC/RedisServiceCollectionExtensions.cs` | Registra o Redis e o `RedisCacheService` no container de injeção de dependência |
| `UsersAPI.Application/AppServices/UsuarioAppService.cs` | Consome o `ICacheService` nos métodos `BuscarPorId`, `AlterarSenha` e `AlterarStatus` |
| `appsettings.Development.json` | Adicionada connection string do Redis (`localhost:6379`) e nível de log `Debug` para o namespace `UsersAPI.Application.AppServices` (necessário para visualizar os logs de cache no terminal) |
| `docker-compose.yml` | Container Redis já configurado (`redis:7-alpine`, porta `6379`) |

---

### Como funciona

| Operação | Comportamento |
|---|---|
| `GET BuscarPorId` (1ª vez) | Busca no banco SQL Server, grava no Redis com TTL de 10 minutos |
| `GET BuscarPorId` (2ª vez em diante) | Retorna direto do Redis, sem consultar o banco |
| `PUT AlterarSenha` | Remove o cache do usuário automaticamente |
| `PUT AlterarStatus` | Remove o cache do usuário automaticamente |

**Chave no Redis:** `UsersAPI:usuario:{guid-do-usuario}`

---

### Passo a passo para testar

**Pré-requisitos:** Docker e .NET 9 instalados.

#### 1. Subir o ambiente

```bash
docker compose up -d
dotnet run --project "src/UsersAPI.Api/UsersAPI.Api.csproj"
```

Acesse o Swagger em `http://localhost:5001/swagger`.

#### 2. Fazer login

Execute `POST /api/Authentication/login`:

```json
{
  "usuario": "seu_usuario",
  "senha": "sua_senha"
}
```

Na resposta, copie o valor do campo `"token"` diretamente do **Response body**. Clique em **Authorize** no Swagger e cole o token sem o prefixo "Bearer".

#### 3. Primeira busca — popula o cache

Execute `GET /api/Usuarios/BuscarPorId/{id}`.

**Log esperado no terminal:**
```
[DBG] Cache populado | UsuarioId: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
```
A API foi ao banco SQL Server e gravou o resultado no Redis.

#### 4. Segunda busca — cache hit

Execute o mesmo `GET` novamente.

**Log esperado no terminal:**
```
[DBG] Cache hit | UsuarioId: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
```
A API **não consultou o banco** — retornou direto do Redis.

#### 5. Verificar a chave no Redis

```bash
docker exec redis-fiap-cloud-games redis-cli KEYS "UsersAPI:*"
```

#### 6. Invalidar o cache

Execute `PUT /api/Usuarios/AlterarStatus?id={id}`. O cache do usuário é removido automaticamente.

#### 7. GET após invalidação

Execute o `GET` novamente. O log mostrará `Cache populado` — a API voltou ao banco pois o cache foi removido.

---

### Resultados comprovados em teste real (15/05/2026)

| Etapa | Operação | Log no terminal | Tempo |
|---|---|---|---|
| 1 | `GET BuscarPorId` — 1ª chamada | `[DBG] Cache populado` | 216ms |
| 2 | `GET BuscarPorId` — 2ª chamada | `[DBG] Cache hit` | 15ms |
| 3 | `PUT AlterarStatus` | cache removido do Redis | 46ms |
| 4 | `GET BuscarPorId` — após invalidação | `[DBG] Cache populado` | 16ms |

**Ganho de performance comprovado: 14x mais rápido com cache ativo (216ms → 15ms).**

---

## 🚀 Pipeline de CI/CD

O ciclo de vida do serviço é totalmente automatizado via **GitHub Actions** (`.github/workflows/ci-cd.yml`), disparado a cada push ou pull request para a branch `main`.

### Etapas do workflow

| Etapa | O que faz |
|---|---|
| **1. Build & Test** | Restaura dependências, compila a solução (`dotnet build`) e executa os testes automatizados (`dotnet test`) |
| **2. Build & Push da imagem** | Autentica na Azure via OIDC, gera a imagem Docker com tag baseada no SHA do commit (`sha-xxxxxxx`) e faz push para o Azure Container Registry |
| **3. Security Scan (Trivy)** | Escaneia a imagem em busca de vulnerabilidades `CRITICAL`/`HIGH` e publica o resultado no GitHub Security. Não bloqueia o deploy (etapa desejável, não obrigatória) |
| **4. Deploy (Rolling Update)** | Conecta ao cluster AKS, converte o kubeconfig para autenticação via Azure RBAC (`kubelogin`) e atualiza a imagem do `Deployment` com `kubectl set image`, aguardando a conclusão do rollout |

### Regras de execução

- **Build & Test** roda em qualquer push ou PR para `main`.
- **Build & Push** e **Deploy** só rodam fora de pull requests (ou seja, após o merge na `main`) — PRs passam apenas pela etapa de build/test como validação.
- Todas as credenciais (Azure, ACR, cluster) são fornecidas via **variáveis e secrets do GitHub Actions**, nunca hardcoded no workflow.

### Escopo atual

A pipeline cobre o **UsersAPI**. O mesmo padrão é replicado no **CatalogAPI**, conforme exigido pela Fase 4 do Tech Challenge.

---

## 🎓 Contexto Acadêmico

Desenvolvido para o **Tech Challenge — PosTech FIAP**, Arquitetura de Software em .NET com Azure.

Este repositório evoluiu ao longo das fases do curso:
- **Fase 2:** estrutura inicial da API, autenticação JWT e persistência relacional (SQL Server)
- **Fase 3:** containerização, Kubernetes local e mensageria (RabbitMQ)
- **Fase 4:** cache distribuído (Redis), deploy gerenciado em Azure AKS e automação de CI/CD com GitHub Actions



