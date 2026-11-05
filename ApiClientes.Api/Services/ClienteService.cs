using ApiClientes.Api.DTOs;
using ApiClientes.Api.Infrastructure.Data;
using ApiClientes.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiClientes.Api.Services;

public class ClienteService : IClienteService
{
    private readonly ApiDbContext _context;
    private readonly ITokenService _tokenService;

    public ClienteService(ApiDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<ClienteDto> RegisterAsync(RegisterClientRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
            throw new ArgumentException("El nombre es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("El email es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            throw new ArgumentException("La contraseña debe tener al menos 6 caracteres.");

        var emailNormalized = request.Email.Trim();
        var exists = await _context.Clientes.AnyAsync(c => c.Email == emailNormalized);
        if (exists)
            throw new InvalidOperationException("Ya existe un cliente con ese email.");

        var cliente = new Cliente
        {
            Nombre = request.Nombre.Trim(),
            Email = emailNormalized,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FechaRegistro = DateTime.UtcNow
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        return new ClienteDto
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Email = cliente.Email,
            FechaRegistro = cliente.FechaRegistro
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return new AuthResponse { Success = false, Message = "Email y contraseña obligatorios." };

        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == request.Email.Trim());
        if (cliente is null)
            return new AuthResponse { Success = false, Message = "Credenciales inválidas." };

        var valid = BCrypt.Net.BCrypt.Verify(request.Password, cliente.PasswordHash);
        if (!valid)
            return new AuthResponse { Success = false, Message = "Credenciales inválidas." };

        var token = _tokenService.GenerateToken(cliente);

        return new AuthResponse
        {
            Success = true,
            Message = "Login exitoso.",
            Token = token,
            Cliente = new ClienteDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Email = cliente.Email,
                FechaRegistro = cliente.FechaRegistro
            }
        };
    }

    public async Task<IEnumerable<ClienteDto>> GetAllAsync()
    {
        return await _context.Clientes
            .OrderBy(c => c.Id)
            .Select(c => new ClienteDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Email = c.Email,
                FechaRegistro = c.FechaRegistro
            })
            .ToListAsync();
    }

    public async Task<ClienteDto?> GetByIdAsync(int id)
    {
        return await _context.Clientes
            .Where(c => c.Id == id)
            .Select(c => new ClienteDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Email = c.Email,
                FechaRegistro = c.FechaRegistro
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ClienteDto?> UpdateAsync(int id, UpdateClienteRequest request)
    {
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
        if (cliente is null) return null;

        if (string.IsNullOrWhiteSpace(request.Nombre))
            throw new ArgumentException("El nombre es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("El email es obligatorio.");

        var emailNormalized = request.Email.Trim();
        var exists = await _context.Clientes.AnyAsync(c => c.Id != id && c.Email == emailNormalized);
        if (exists)
            throw new InvalidOperationException("Ya existe un cliente con ese email.");

        cliente.Nombre = request.Nombre.Trim();
        cliente.Email = emailNormalized;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            if (request.Password.Length < 6)
                throw new ArgumentException("La contraseña debe tener al menos 6 caracteres.");

            cliente.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }

        await _context.SaveChangesAsync();

        return new ClienteDto
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Email = cliente.Email,
            FechaRegistro = cliente.FechaRegistro
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
        if (cliente is null) return false;

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();
        return true;
    }
}
