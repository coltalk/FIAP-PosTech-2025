# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TcUser.sln ./
COPY TcUser.Api/TcUser.Api.csproj TcUser.Api/
COPY TcUser.Application/TcUser.Application.csproj TcUser.Application/
COPY TcUser.Domain/TcUser.Domain.csproj TcUser.Domain/
COPY TcUser.Infra/TcUser.Infra.csproj TcUser.Infra/
RUN dotnet restore TcUser.Api/TcUser.Api.csproj

COPY . .
RUN dotnet publish TcUser.Api/TcUser.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5002
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

EXPOSE 5002
ENTRYPOINT ["dotnet", "TcUser.Api.dll"]
