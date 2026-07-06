using gestao_sinistros_api.Application.Api.Commom;
using gestao_sinistros_api.Application.Api.DTOs;
using gestao_sinistros_api.Application.Api.DTOs.Cliente;
using gestao_sinistros_api.Application.Api.Services;
using gestao_sinistros_api.Application.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace gestao_sinistros_api.Tests;

public class ApoliceServiceTests
{
    private readonly AppDbContext _context;
    private readonly ClienteService _clienteService;
    private readonly ApoliceService _apoliceService;

    public ApoliceServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _clienteService = new ClienteService(_context);
        _apoliceService = new ApoliceService(_context);
    }

    [Fact]
    public async Task ApoliceCreate_ShouldRequireExistingCliente()
    {
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _apoliceService.Create(new CreateApoliceDto
        {
            Numero = "APL-100",
            ClienteId = Guid.NewGuid(),
            RamoSeguro = RamoSeguro.Automovel,
            Ativo = true,
            DataInicio = DateTime.UtcNow.AddDays(-1),
            DataFim = DateTime.UtcNow.AddDays(30)
        }));

        Assert.Equal("Cliente não encontrado.", exception.Message);
    }

    [Fact]
    public async Task ApoliceCrud_ShouldCreateReadUpdateAndDeleteSuccessfully()
    {
        var cliente = await _clienteService.Create(new UpsertClienteDto
        {
            Nome = "João",
            Documento = "55566677788",
            Email = "joao@email.com",
            Telefone = "11777776666"
        });

        var created = await _apoliceService.Create(new CreateApoliceDto
        {
            Numero = "APL-200",
            ClienteId = cliente.Id,
            RamoSeguro = RamoSeguro.Residencial,
            Ativo = true,
            DataInicio = DateTime.UtcNow.AddDays(-5),
            DataFim = DateTime.UtcNow.AddDays(30)
        });

        var fetched = await _apoliceService.GetById(created.Id);
        Assert.NotNull(fetched);
        Assert.Equal("APL-200", fetched!.Numero);

        var updated = await _apoliceService.Update(created.Id, new UpdateApoliceDto
        {
            RamoSeguro = RamoSeguro.Vida,
            Ativo = false,
            DataInicio = DateTime.UtcNow.AddDays(-10),
            DataFim = DateTime.UtcNow.AddDays(20)
        });

        Assert.NotNull(updated);
        Assert.False(updated!.Ativo);
        Assert.Equal(RamoSeguro.Vida, updated.RamoSeguro);

        var deleted = await _apoliceService.Delete(created.Id);
        Assert.True(deleted);

        var afterDelete = await _apoliceService.GetById(created.Id);
        Assert.Null(afterDelete);
    }
}
