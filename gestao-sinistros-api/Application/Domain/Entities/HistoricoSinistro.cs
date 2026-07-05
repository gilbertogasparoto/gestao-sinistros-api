using gestao_sinistros_api.Application.Domain.Enums;

namespace gestao_sinistros_api.Application.Domain.Entities
{
    public class HistoricoSinistro : BaseEntity
    {
        public Guid SinistroId { get; set; }

        public Sinistro Sinistro { get; set; } = null!;

        public SinistroStatus Status { get; set; }

        public string Observacao { get; set; } = string.Empty;

        public DateTime DataAlteracao { get; set; }
    }
}