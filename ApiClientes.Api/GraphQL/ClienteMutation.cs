using ApiClientes.Api.DTOs;
using ApiClientes.Api.Services;

namespace ApiClientes.Api.GraphQL;

public class ClienteMutation
{
    public async Task<ClienteGraphQLDto> RegisterClienteAsync(
        string nombre,
        string email,
        string password,
        [Service] IClienteService clienteService)
    {
        var cliente = await clienteService.RegisterAsync(new RegisterClientRequest
        {
            Nombre = nombre,
            Email = email,
            Password = password
        });

        return new ClienteGraphQLDto
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Email = cliente.Email,
            FechaRegistro = cliente.FechaRegistro
        };
    }

    public async Task<AuthResponse> LoginClienteAsync(
        string email,
        string password,
        [Service] IClienteService clienteService)
    {
        return await clienteService.LoginAsync(new LoginRequest
        {
            Email = email,
            Password = password
        });
    }
}
