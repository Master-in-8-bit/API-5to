using API.DTOs;
using API.Models;
using API.Repositories;
using API.Services;

namespace API.Services;

public interface IClienteService
{
    Task<List<ClienteResponse>> GetAllAsync();
    Task<ClienteResponse?> GetByIdAsync(int id);
    Task<ClienteResponse> RegisterAsync(RegisterClienteRequest request);
    Task<LoginResponse> LoginAsync(LoginClienteRequest request, string jwtKey);
    Task<ClienteResponse?> UpdateAsync(int id, UpdateClienteRequest request);
    Task<bool> DeleteAsync(int id);
}

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly TokenService _tokenService;

    public ClienteService(IClienteRepository clienteRepository, TokenService tokenService)
    {
        _clienteRepository = clienteRepository;
        _tokenService = tokenService;
    }

    public async Task<List<ClienteResponse>> GetAllAsync()
    {
        var clientes = await _clienteRepository.GetAllAsync();
        return clientes.Select(c => new ClienteResponse(c.Id, c.Nombre, c.Email, c.FechaRegistro)).ToList();
    }

    public async Task<ClienteResponse?> GetByIdAsync(int id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente is null)
        {
            return null;
        }

        return new ClienteResponse(cliente.Id, cliente.Nombre, cliente.Email, cliente.FechaRegistro);
    }

    public async Task<ClienteResponse> RegisterAsync(RegisterClienteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            throw new InvalidOperationException("Nombre, email y contraseña son obligatorios.");
        }

        var email = request.Email.Trim();
        var existing = await _clienteRepository.GetByEmailAsync(email);
        if (existing is not null)
        {
            throw new InvalidOperationException("Ya existe un cliente con ese email.");
        }

        var cliente = new Cliente
        {
            Nombre = request.Nombre.Trim(),
            Email = email,
            PasswordHash = PasswordService.HashPassword(request.Password),
            FechaRegistro = DateTime.UtcNow
        };

        await _clienteRepository.AddAsync(cliente);
        await _clienteRepository.SaveChangesAsync();

        return new ClienteResponse(cliente.Id, cliente.Nombre, cliente.Email, cliente.FechaRegistro);
    }

    public async Task<LoginResponse> LoginAsync(LoginClienteRequest request, string jwtKey)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new InvalidOperationException("Email y contraseña son obligatorios.");
        }

        var cliente = await _clienteRepository.GetByEmailAsync(request.Email.Trim());
        if (cliente is null)
        {
            throw new InvalidOperationException("Credenciales inválidas.");
        }

        if (!PasswordService.VerifyPassword(request.Password, cliente.PasswordHash))
        {
            throw new InvalidOperationException("Credenciales inválidas.");
        }

        var token = _tokenService.GenerateToken(cliente);

        return new LoginResponse(token, cliente.Id, cliente.Nombre, cliente.Email, cliente.FechaRegistro);
    }

    public async Task<ClienteResponse?> UpdateAsync(int id, UpdateClienteRequest request)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(request.Nombre))
        {
            cliente.Nombre = request.Nombre.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim();
            var existing = await _clienteRepository.GetByEmailAsync(email);
            if (existing is not null && existing.Id != id)
            {
                throw new InvalidOperationException("Ya existe otro cliente con ese email.");
            }

            cliente.Email = email;
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            cliente.PasswordHash = PasswordService.HashPassword(request.Password);
        }

        await _clienteRepository.UpdateAsync(cliente);
        await _clienteRepository.SaveChangesAsync();

        return new ClienteResponse(cliente.Id, cliente.Nombre, cliente.Email, cliente.FechaRegistro);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente is null)
        {
            return false;
        }

        await _clienteRepository.DeleteAsync(cliente);
        await _clienteRepository.SaveChangesAsync();

        return true;
    }
}
