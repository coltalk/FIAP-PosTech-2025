# 👤 UsersAPI — FCG FIAP Cloud Games

Microsserviço responsável pelo **cadastro, autenticação e gerenciamento de usuários** da plataforma FIAP Cloud Games.

---

## 🧱 Tecnologias

- .NET 9
- SQL Server (dados relacionais)
- MongoDB (logs via Serilog)
- RabbitMQ (publicação de eventos)
- JWT Bearer Authentication

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

### Pré-requisitos

- Docker Desktop com **Kubernetes habilitado**
- `kubectl` disponível no terminal
- Infraestrutura já aplicada via OrchestrationAPI

### Estrutura dos manifestos

```
UsersAPI/
└── k8s/
    ├── configmap.yaml        ← variáveis não sensíveis (inclui ConnectionStrings__Redis)
    ├── secret.yaml           ← variáveis sensíveis (Base64)
    ├── deployment.yaml       ← gerencia os Pods
    ├── service.yaml          ← expõe o serviço na rede
    ├── redis-deployment.yaml ← cache distribuído (Redis) usado pelo UsersAPI
    └── redis-service.yaml    ← Service interno (ClusterIP) do Redis
```

### 1. Aplicar os manifestos

```bash
# Na raiz do repositório UsersAPI
kubectl apply -f k8s/
```

### 2. Verificar se está rodando

```bash
kubectl get pods
kubectl get services
```

### 3. Acessar o Swagger

```bash
# Descubra a porta externa atribuída
kubectl get services

# Acesse no browser (substitua pela porta real)
http://localhost:30001/swagger
```

> ⚠️ O Docker Desktop pode atribuir uma porta diferente da definida no manifesto. Verifique a porta real com `kubectl get services` na coluna `PORT(S)`.

### Parar o serviço

```bash
kubectl delete -f k8s/
```

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

## 🎓 Contexto Acadêmico

Desenvolvido para o **Tech Challenge Fase 2 — PosTech FIAP**
Arquitetura de Software em .NET com Azure.