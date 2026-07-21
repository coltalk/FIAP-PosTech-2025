using Microsoft.EntityFrameworkCore;
using Prometheus;
using Serilog;
using UsersAPI.Api.Endpoints;
using UsersAPI.Api.Middleware;
using UsersAPI.Data;
using UsersAPI.IoC;
using UsersAPI.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJwtAuthenticationConfig(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddControllers();
builder.AddSerilogConfiguration();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<UsersApiDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("MS_UserAPI")));
builder.Services.AddApplicationServices();
builder.Services.AddDomainServices();
builder.Services.AddRepositories();
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.AddAuthenticationDependencies(builder.Configuration);
builder.Host.UseSerilog();

// Registrar RabbitMQ
builder.Services.AddRabbitMQMessaging(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

// Inicializar RabbitMQ
try
{
    using var scope = app.Services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<RabbitMQInitializer>();
    await initializer.InitializeAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro ao inicializar RabbitMQ");
    throw;
}

//rodar migrations Auto.
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<UsersApiDbContext>();
    db.Database.Migrate();

    Log.Information("Migrations aplicadas com sucesso.");
}
catch(Exception ex)
{
    Log.Error(ex, "Erro ao aplicar migrations");
    throw;
}

app.UseMiddleware<LoggingMiddleware>();
app.UseSerilogRequestLoggingConfiguration();
app.UseHttpsRedirection();
app.UseHttpMetrics();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapMetrics();
app.MapControllers();
app.MapAuthentication();
app.MapContatos();
app.MapEnderecos();
app.MapRoles();
app.MapUsuarioPerfil();
app.MapUsuarioRole();
app.MapUsuarios();

app.Run();