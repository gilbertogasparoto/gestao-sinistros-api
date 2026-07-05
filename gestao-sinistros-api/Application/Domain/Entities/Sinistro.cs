using gestao_sinistros_api.Application.Domain.Enums;

namespace gestao_sinistros_api.Application.Domain.Entities
{
    public class Sinistro : BaseEntity
    {
        public string Numero { get; set; } = string.Empty;

        public Guid ApoliceId { get; set; }

        public Apolice Apolice { get; set; } = null!;

        public string Descricao { get; set; } = string.Empty;

        public SinistroStatus Status { get; set; }

        public DateTime DataOcorrencia { get; set; }

        public decimal ValorEstimado { get; set; }

        public decimal ValorAprovado { get; set; }

        public string? MotivoNegacao { get; set; }

        public ICollection<HistoricoSinistro> Historicos { get; set; } = new List<HistoricoSinistro>();
    }
}