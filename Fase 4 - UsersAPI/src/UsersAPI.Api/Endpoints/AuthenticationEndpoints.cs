using UsersAPI.Api.Filters;
using UsersAPI.Api.Helpers;
using UsersAPI.Application.Interfaces;
using UsersAPI.Domain.Dtos.Request.Authentication;
using UsersAPI.Domain.Dtos.Responses.Authentication;

namespace UsersAPI.Api.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthentication(this IEndpointRouteBuilder route)
    {
        var app = route.MapGroup("/api/Authentication").WithTags("Authentication");

        app.MapPost("login/", async (LoginRequest request, IAuthenticationAppService authenticationService) =>
        {
            var loginResponse = await authenticationService.Login(request.Usuario, request.Senha);

            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                return ApiResponses.Unauthorized("Usuário ou senha inválidos.");
            }

            return ApiResponses.Ok(loginResponse, "Login realizado com sucesso.");
        })
        .AddEndpointFilter<ValidationEndpointFilter<LoginRequest>>()
        .WithName("Login")
        .Produces<LoginResponse>(200)
        .Produces(400)
        .Produces(401);
    }
}
