# TcUser API

API em **.NET 8** para cadastro e autenticação de usuários, com **JWT Bearer**, autorização por papéis e persistência via **Entity Framework Core** com **SQL Server**.

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

- `TcUser.Api`: camada de apresentação (controllers, middlewares, autenticação, Swagger)
- `TcUser.Application`: regras de aplicação, DTOs, validações e serviços
- `TcUser.Domain`: entidades e enums de domínio
- `TcUser.Infra`: contexto EF Core, mapeamentos, repositórios e migrations

## ✅ Pré-requisitos

- .NET SDK 8.0+
- SQL Server (local ou remoto)
- RabbitMQ (local ou remoto)
- Docker Desktop (opcional, para execução em container)

## ⚙️ Configuração

1. Ajuste a conexão com banco no arquivo `TcUser.Api/appsettings.json` em `ConnectionStrings:DefaultConnection`.
2. Revise as configurações JWT em `Jwt` (`Issuer`, `Audience`, `Secret`, `ExpirationMinutes`).
3. Configure a seção `RabbitMq` em `TcCatalog.Api/appsettings.json` (`HostName`, `Port`, `UserName`, `Password`, `VirtualHost` e nomes das filas).
4. Em ambiente local, prefira sobrescrever segredos por variáveis de ambiente.

## ▶️ Como executar

No diretório raiz do repositório:

```bash
dotnet restore
dotnet build
dotnet run --project TcUser.Api
```

Por padrão, a API sobe com Swagger em ambiente de desenvolvimento em `http://localhost:5002/swagger`.

## 🗄️ Banco de dados (migrations)

Para aplicar as migrations existentes:

```bash
dotnet tool restore
dotnet ef database update --project TcUser.Infra --startup-project TcUser.Api
```

Se precisar criar uma nova migration:

```bash
dotnet ef migrations add NomeDaMigration --project TcUser.Infra --startup-project TcUser.Api
```

## 🔐 Autenticação e autorização

A API usa **Bearer Token (JWT)**.

- `POST /api/auth/register` e `POST /api/auth/login` são públicos.
- `GET /api/auth/me` exige autenticação.
- `GET /api/auth/admin-only` exige o papel `Admin`.

No Swagger, clique em **Authorize** e informe:

```text
Bearer {seu_token}
```

## 🧭 Principais endpoints

### Autenticação

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/auth/me`
- `GET /api/auth/admin-only`

### Exemplo de registro

```json
{
  "name": "Joao Silva",
  "email": "joao@email.com",
  "password": "Senha@123"
}
```

### Exemplo de login

```json
{
  "email": "joao@email.com",
  "password": "Senha@123"
}
```

### Exemplo de resposta no login

```json
{
  "accessToken": "string",
  "expiresAtUtc": "2026-03-18T12:00:00Z"
}
```

### Exemplo de resposta em `/api/auth/me`

```json
{
  "id": "guid-ou-sub",
  "email": "joao@email.com",
  "name": "Joao Silva",
  "role": "User"
}
```

## Regras de negócio relevantes

- o e-mail é normalizado com `trim` e `lowercase` antes de consultar e salvar
- não permite cadastro com e-mail já existente
- a senha é armazenada com hash BCrypt
- a senha deve conter letras, números e caractere especial
- o endpoint administrativo exige role `Admin`

## 📣 Mensageria de eventos (RabbitMQ)

- Após um novo usuário, a API publica `UserCreatedEvent` na fila.


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
http://localhost:5002/swagger
```

### Observações sobre Docker

- o `docker-compose.yml` publica a API na porta `5002`
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
kubectl port-forward service/tcuser-api 5002:80
```

Depois disso, o Swagger pode ser acessado em:

```text
http://localhost:5002/swagger
```

## 👨🏻‍🎓 Alunos

- Clovis Ribeiro Ramos - rm369652
- Matheus Machado Pinheiro do Valle - rm369919
- Pedro Delgado Henriques -rm369869
- Jose Mauro Morais Mani -rm
