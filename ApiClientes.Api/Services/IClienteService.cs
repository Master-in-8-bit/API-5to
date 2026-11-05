using ApiClientes.Api.DTOs;
using ApiClientes.Api.Models;

namespace ApiClientes.Api.Services;

public interface IClienteService
{
    Task<ClienteDto> RegisterAsync(RegisterClientRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<IEnumerable<ClienteDto>> GetAllAsync();
    Task<ClienteDto?> GetByIdAsync(int id);
    Task<ClienteDto?> UpdateAsync(int id, UpdateClienteRequest request);
    Task<bool> DeleteAsync(int id);
}
