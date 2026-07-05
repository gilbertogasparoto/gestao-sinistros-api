using gestao_sinistros_api.Application.Domain.Enums;

namespace gestao_sinistros_api.Application.Api.DTOs.Sinistro
{
    public class HistoricoSinistroResponseDto
    {
        public Guid Id { get; set; }
        public Guid SinistroId { get; set; }
        public SinistroStatus Status { get; set; }
        public string Observacao { get; set; } = string.Empty;
        public DateTime DataAlteracao { get; set; }
    }
}
