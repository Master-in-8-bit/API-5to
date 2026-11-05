using ApiClientes.Api.DTOs;
using ApiClientes.Api.Infrastructure.Data;
using ApiClientes.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace ApiClientes.Tests;

public class ClienteServiceTests
{
    private static ApiDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApiDbContext(options);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateClientWithHashedPassword()
    {
        await using var context = CreateContext();
        var tokenService = new JwtTokenService("super-secret-key-for-testing-1234567890");
        var service = new ClienteService(context, tokenService);

        var result = await service.RegisterAsync(new RegisterClientRequest
        {
            Nombre = "Ana",
            Email = "ana@test.com",
            Password = "123456"
        });

        Assert.NotNull(result);
        Assert.Equal("Ana", result.Nombre);

        var stored = await context.Clientes.SingleAsync();
        Assert.NotEqual("123456", stored.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("123456", stored.PasswordHash));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreCorrect()
    {
        await using var context = CreateContext();
        var tokenService = new JwtTokenService("super-secret-key-for-testing-1234567890");
        var service = new ClienteService(context, tokenService);

        await service.RegisterAsync(new RegisterClientRequest
        {
            Nombre = "Ana",
            Email = "ana@test.com",
            Password = "123456"
        });

        var loginResult = await service.LoginAsync(new LoginRequest
        {
            Email = "ana@test.com",
            Password = "123456"
        });

        Assert.True(loginResult.Success);
        Assert.False(string.IsNullOrWhiteSpace(loginResult.Token));
    }
}
