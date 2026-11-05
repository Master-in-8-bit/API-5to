using ApiClientes.Api.Models;

namespace ApiClientes.Api.Services;

public interface ITokenService
{
    string GenerateToken(Cliente cliente);
}
