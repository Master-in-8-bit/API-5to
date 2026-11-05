using ApiClientes.Api.Services;

namespace ApiClientes.Api.GraphQL;

public class ClienteQuery
{
    public async Task<IEnumerable<ClienteGraphQLDto>> GetClientesAsync([Service] IClienteService clienteService)
    {
        return (await clienteService.GetAllAsync())
            .Select(c => new ClienteGraphQLDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Email = c.Email,
                FechaRegistro = c.FechaRegistro
            });
    }

    public async Task<ClienteGraphQLDto?> GetClienteByIdAsync(int id, [Service] IClienteService clienteService)
    {
        var client = await clienteService.GetByIdAsync(id);
        if (client is null) return null;

        return new ClienteGraphQLDto
        {
            Id = client.Id,
            Nombre = client.Nombre,
            Email = client.Email,
            FechaRegistro = client.FechaRegistro
        };
    }
}

public class ClienteGraphQLDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
}
