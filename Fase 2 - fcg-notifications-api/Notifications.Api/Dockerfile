FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Notifications.sln .
COPY Notifications.Api/Notifications.Api.csproj Notifications.Api/
COPY Notifications.Application/Notifications.Application.csproj Notifications.Application/
COPY Notifications.Infra/Notifications.Infra.csproj Notifications.Infra/
RUN dotnet restore

COPY . .
RUN dotnet publish Notifications.Api/Notifications.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Notifications.Api.dll"]
