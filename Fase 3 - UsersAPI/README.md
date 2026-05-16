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
    ├── configmap.yaml   ← variáveis não sensíveis
    ├── secret.yaml      ← variáveis sensíveis (Base64)
    ├── deployment.yaml  ← gerencia os Pods
    └── service.yaml     ← expõe o serviço na rede
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

## 🎓 Contexto Acadêmico

Desenvolvido para o **Tech Challenge Fase 2 — PosTech FIAP**
Arquitetura de Software em .NET com Azure.