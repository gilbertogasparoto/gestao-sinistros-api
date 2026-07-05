using gestao_sinistros_api.Application.Domain.Enums;

namespace gestao_sinistros_api.Application.Api.DTOs.Sinistro
{
    public class SinistroResponseDto
    {
        public Guid Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public Guid ApoliceId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public SinistroStatus Status { get; set; }
        public DateTime DataOcorrencia { get; set; }
        public decimal ValorEstimado { get; set; }
        public decimal ValorAprovado { get; set; }
        public string? MotivoNegacao { get; set; }
    }
}
