FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TcCatalog.sln .
COPY TcCatalog.Api/TcCatalog.Api.csproj TcCatalog.Api/
COPY TcCatalog.Application/TcCatalog.Application.csproj TcCatalog.Application/
COPY TcCatalog.Domain/TcCatalog.Domain.csproj TcCatalog.Domain/
COPY TcCatalog.Infra/TcCatalog.Infra.csproj TcCatalog.Infra/
RUN dotnet restore

COPY . .
RUN dotnet publish TcCatalog.Api/TcCatalog.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5001
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

EXPOSE 5001
ENTRYPOINT ["dotnet", "TcCatalog.Api.dll"]
