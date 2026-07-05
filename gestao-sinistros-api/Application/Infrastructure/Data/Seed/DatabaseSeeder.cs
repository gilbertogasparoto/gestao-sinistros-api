using Bogus;
using gestao_sinistros_api.Application.Domain.Entities;
using gestao_sinistros_api.Application.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace gestao_sinistros_api.Application.Infrastructure.Data.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Clientes.AnyAsync(cancellationToken))
        {
            return;
        }

        var faker = new Faker("pt_BR");

        var clientes = Enumerable.Range(1, 9)
            .Select(_ => new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = faker.Name.FullName(),
                Documento = faker.Random.Replace("###########"),
                Email = faker.Internet.Email(),
                Telefone = faker.Phone.PhoneNumber("###########")
            })
            .ToList();

        await context.Clientes.AddRangeAsync(clientes, cancellationToken);

        var apolices = new List<Apolice>();
        foreach (var cliente in clientes)
        {
            var quantidadeApolices = faker.Random.Int(1, 3);

            for (var index = 0; index < quantidadeApolices; index++)
            {
                var dataInicio = DateTime.UtcNow.AddMonths(-faker.Random.Int(6, 36));

                apolices.Add(new Apolice
                {
                    Id = Guid.NewGuid(),
                    ClienteId = cliente.Id,
                    Numero = $"AP-{faker.Random.Replace("#######")}",
                    RamoSeguro = faker.PickRandom<RamoSeguro>(),
                    Ativo = faker.Random.Bool(0.8f),
                    DataInicio = dataInicio,
                    DataFim = dataInicio.AddMonths(faker.Random.Int(6, 24))
                });
            }
        }

        await context.Apolices.AddRangeAsync(apolices, cancellationToken);

        var statusValores = Enum.GetValues<SinistroStatus>();
        var sinistros = Enumerable.Range(1, 20)
            .Select(_ =>
            {
                var apolice = faker.PickRandom(apolices);
                var status = faker.PickRandom(statusValores);
                var dataOcorrencia = faker.Date.Between(DateTime.UtcNow.AddYears(-2), DateTime.UtcNow.AddDays(-5));
                var closedAt = status is SinistroStatus.Aberto or SinistroStatus.EmAnalise or SinistroStatus.Aprovado
                    ? (DateTime?)null
                    : faker.Date.Between(dataOcorrencia, DateTime.UtcNow.AddDays(7));

                return new Sinistro
                {
                    Id = Guid.NewGuid(),
                    ApoliceId = apolice.Id,
                    Numero = $"SIN-{faker.Random.Replace("#######")}",
                    Descricao = faker.Lorem.Sentence(10),
                    Status = status,
                    DataOcorrencia = dataOcorrencia,
                    ValorEstimado = Math.Round(faker.Random.Decimal(1000, 50000), 2),
                    ValorAprovado = status == SinistroStatus.Negado
                        ? 0m
                        : Math.Round(faker.Random.Decimal(0, 50000), 2),
                    MotivoNegacao = status == SinistroStatus.Negado ? faker.Lorem.Sentence(6) : null,
                    ClosedAt = closedAt
                };
            })
            .ToList();

        await context.Sinistros.AddRangeAsync(sinistros, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
