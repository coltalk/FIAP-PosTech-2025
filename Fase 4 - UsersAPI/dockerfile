# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia a solução e todos os projetos
COPY ["UsersAPI.sln", "."]
COPY ["src/UsersAPI.Api/UsersAPI.Api.csproj", "src/UsersAPI.Api/"]
COPY ["src/UsersAPI.Application/UsersAPI.Application.csproj", "src/UsersAPI.Application/"]
COPY ["src/UsersAPI.Authentication/UsersAPI.Authentication.csproj", "src/UsersAPI.Authentication/"]
COPY ["src/UsersAPI.Data/UsersAPI.Data.csproj", "src/UsersAPI.Data/"]
COPY ["src/UsersAPI.Domain/UsersAPI.Domain.csproj", "src/UsersAPI.Domain/"]
COPY ["src/UsersAPI.IoC/UsersAPI.IoC.csproj", "src/UsersAPI.IoC/"]
COPY ["src/UsersAPI.Messaging/UsersAPI.Messaging.csproj", "src/UsersAPI.Messaging/"]

# Restaura as dependências
RUN dotnet restore "UsersAPI.sln"

# Copia o restante do código
COPY . .

# Publica apenas o projeto principal
RUN dotnet publish "src/UsersAPI.Api/UsersAPI.Api.csproj" -c Release -o /app/publish

# Imagem final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UsersAPI.Api.dll"]