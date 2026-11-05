namespace API.DTOs;

public record RegisterClienteRequest(string Nombre, string Email, string Password);

public record LoginClienteRequest(string Email, string Password);

public record UpdateClienteRequest(string? Nombre, string? Email, string? Password);

public record ClienteResponse(int Id, string Nombre, string Email, DateTime FechaRegistro);

public record LoginResponse(string Token, int Id, string Nombre, string Email, DateTime FechaRegistro);
