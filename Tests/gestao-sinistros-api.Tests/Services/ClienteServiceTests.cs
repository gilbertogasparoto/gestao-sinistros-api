using gestao_sinistros_api.Application.Api.DTOs.Cliente;
using gestao_sinistros_api.Application.Api.Services;
using gestao_sinistros_api.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace gestao_sinistros_api.Tests;

public class ClienteServiceTests
{
    private readonly AppDbContext _context;
    private readonly ClienteService _clienteService;

    public ClienteServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _clienteService = new ClienteService(_context);
    }

    [Fact]
    public async Task ClienteCrud_ShouldCreateReadUpdateAndDeleteSuccessfully()
    {
        var created = await _clienteService.Create(new UpsertClienteDto
        {
            Nome = "Maria",
            Documento = "11122233344",
            Email = "maria@email.com",
            Telefone = "11999998888"
        });

        var fetched = await _clienteService.GetById(created.Id);
        Assert.NotNull(fetched);
        Assert.Equal("Maria", fetched!.Nome);

        var updated = await _clienteService.Update(created.Id, new UpsertClienteDto
        {
            Nome = "Maria Souza",
            Documento = "11122233344",
            Email = "maria.souza@email.com",
            Telefone = "11888887777"
        });

        Assert.NotNull(updated);
        Assert.Equal("Maria Souza", updated!.Nome);

        var deleted = await _clienteService.Delete(created.Id);
        Assert.True(deleted);

        var afterDelete = await _clienteService.GetById(created.Id);
        Assert.Null(afterDelete);
    }
}
