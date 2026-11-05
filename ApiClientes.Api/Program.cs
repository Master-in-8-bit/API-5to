
using ApiClientes.Api.GraphQL;
using ApiClientes.Api.Infrastructure.Authentication;
using ApiClientes.Api.Infrastructure.Data;
using ApiClientes.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API Clientes",
        Version = "v1",
        Description = "API REST para registro, login y gestión de clientes con integración GraphQL."
    });
});

var configKey = builder.Configuration["Jwt:Key"] ?? "EsteEsUnSecretoMuySeguroParaApiClientes2026!";
var issuer = builder.Configuration["Jwt:Issuer"] ?? "ApiClientes";
var audience = builder.Configuration["Jwt:Audience"] ?? "ApiClientesUsers";

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=clientes.db"));

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddSingleton<ITokenService>(new JwtTokenService(configKey, issuer, audience));
builder.Services.AddCustomJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddGraphQLServer()
    .AddQueryType<ClienteQuery>()
    .AddMutationType<ClienteMutation>()
    .AddFiltering()
    .AddSorting();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Clientes v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGraphQL("/graphql");

app.Run();

public partial class Program { }
