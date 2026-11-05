using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly ClientesDbContext _context;

    public ClienteRepository(ClientesDbContext context)
    {
        _context = context;
    }

    public async Task<List<Cliente>> GetAllAsync()
    {
        return await _context.Clientes.OrderBy(c => c.Id).ToListAsync();
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Cliente?> GetByEmailAsync(string email)
    {
        return await _context.Clientes.FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task AddAsync(Cliente cliente)
    {
        await _context.Clientes.AddAsync(cliente);
    }

    public Task UpdateAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Cliente cliente)
    {
        _context.Clientes.Remove(cliente);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
