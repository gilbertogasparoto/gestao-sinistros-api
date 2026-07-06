using gestao_sinistros_api.Application.Api.Commom;
using gestao_sinistros_api.Application.Api.DTOs;
using gestao_sinistros_api.Application.Api.DTOs.Sinistro;
using gestao_sinistros_api.Application.Api.Services;
using gestao_sinistros_api.Application.Domain.Entities;
using gestao_sinistros_api.Application.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace gestao_sinistros_api.Tests;

public class SinistroServiceTests
{
    private readonly AppDbContext _context;
    private readonly SinistroService _service;

    public SinistroServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _service = new SinistroService(_context);
    }

    [Fact]
    public async Task Create_ShouldThrow_WhenPolicyIsInactive()
    {
        var cliente = await SeedClienteAsync();
        var apolice = await SeedApoliceAsync(cliente.Id, ativo: false);

        var dto = new CreateSinistroDto
        {
            Numero = "SIN-001",
            ApoliceId = apolice.Id,
            Descricao = "Sinistro de teste",
            DataOcorrencia = DateTime.UtcNow,
            ValorEstimado = 1000m
        };

        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.Create(dto));

        Assert.Equal("A apólice está inativa ou expirada para abertura de sinistro.", exception.Message);
    }

    [Fact]
    public async Task ChangeStatus_ShouldAllowForwardFlow_AndGenerateHistoryRecords()
    {
        var sinistro = await SeedSinistroAsync();

        await _service.ChangeStatus(sinistro.Id, new ChangeStatusSinistroDto
        {
            Status = SinistroStatus.EmAnalise,
            Observacao = "Em análise"
        });

        await _service.ChangeStatus(sinistro.Id, new ChangeStatusSinistroDto
        {
            Status = SinistroStatus.Aprovado,
            Observacao = "Aprovado"
        });

        var result = await _service.ChangeStatus(sinistro.Id, new ChangeStatusSinistroDto
        {
            Status = SinistroStatus.Encerrado,
            ValorAprovado = 2500m,
            Observacao = "Encerrado"
        });

        Assert.NotNull(result);
        Assert.Equal(SinistroStatus.Encerrado, result!.Status);

        var historico = await _context.HistoricoSinistros
            .Where(h => h.SinistroId == sinistro.Id)
            .ToListAsync();

        Assert.Equal(4, historico.Count);
        Assert.Contains(historico, h => h.Status == SinistroStatus.Aberto);
        Assert.Contains(historico, h => h.Status == SinistroStatus.EmAnalise);
        Assert.Contains(historico, h => h.Status == SinistroStatus.Aprovado);
        Assert.Contains(historico, h => h.Status == SinistroStatus.Encerrado);
    }

    [Fact]
    public async Task ChangeStatus_ShouldRejectInvalidTransition()
    {
        var sinistro = await SeedSinistroAsync();

        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.ChangeStatus(sinistro.Id, new ChangeStatusSinistroDto
        {
            Status = SinistroStatus.Encerrado,
            Observacao = "Mudança inválida"
        }));

        Assert.Contains("Não é permitido alterar o status", exception.Message);
    }

    [Fact]
    public async Task ChangeStatus_ShouldRequireMotivoNegacao_WhenStatusIsNegado()
    {
        var sinistro = await SeedSinistroAsync(status: SinistroStatus.EmAnalise);

        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.ChangeStatus(sinistro.Id, new ChangeStatusSinistroDto
        {
            Status = SinistroStatus.Negado,
            Observacao = "Negado sem motivo"
        }));

        Assert.Equal("O Motivo de Negação é obrigatório ao negar um sinistro.", exception.Message);

        var result = await _service.ChangeStatus(sinistro.Id, new ChangeStatusSinistroDto
        {
            Status = SinistroStatus.Negado,
            MotivoNegacao = "Falta de documentos",
            Observacao = "Negado"
        });

        Assert.NotNull(result);
        Assert.Equal(SinistroStatus.Negado, result!.Status);
    }

    [Fact]
    public async Task ChangeStatus_ShouldRequireValorAprovado_WhenClosingSinistro()
    {
        var sinistro = await SeedSinistroAsync(status: SinistroStatus.Aprovado);

        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.ChangeStatus(sinistro.Id, new ChangeStatusSinistroDto
        {
            Status = SinistroStatus.Encerrado,
            Observacao = "Encerramento sem valor"
        }));

        Assert.Equal("O Valor Aprovado é obrigatório ao encerrar um sinistro.", exception.Message);

        var result = await _service.ChangeStatus(sinistro.Id, new ChangeStatusSinistroDto
        {
            Status = SinistroStatus.Encerrado,
            ValorAprovado = 1800m,
            Observacao = "Encerrado"
        });

        Assert.NotNull(result);
        Assert.Equal(SinistroStatus.Encerrado, result!.Status);
    }

    private async Task<Cliente> SeedClienteAsync()
    {
        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = "Cliente Teste",
            Documento = "12345678900",
            Email = "cliente@teste.com",
            Telefone = "11999999999"
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }

    private async Task<Apolice> SeedApoliceAsync(Guid clienteId, bool ativo)
    {
        var apolice = new Apolice
        {
            Id = Guid.NewGuid(),
            Numero = "APL-001",
            ClienteId = clienteId,
            RamoSeguro = RamoSeguro.Automovel,
            Ativo = ativo,
            DataInicio = DateTime.UtcNow.AddDays(-10),
            DataFim = DateTime.UtcNow.AddDays(10)
        };

        _context.Apolices.Add(apolice);
        await _context.SaveChangesAsync();
        return apolice;
    }

    private async Task<Sinistro> SeedSinistroAsync(SinistroStatus status = SinistroStatus.Aberto)
    {
        var cliente = await SeedClienteAsync();
        var apolice = await SeedApoliceAsync(cliente.Id, ativo: true);

        var created = await _service.Create(new CreateSinistroDto
        {
            Numero = "SIN-TESTE",
            ApoliceId = apolice.Id,
            Descricao = "Descrição do sinistro",
            DataOcorrencia = DateTime.UtcNow,
            ValorEstimado = 1000m
        });

        var sinistro = await _context.Sinistros.SingleAsync(s => s.Id == created.Id);
        sinistro.Status = status;
        await _context.SaveChangesAsync();

        return sinistro;
    }
}
