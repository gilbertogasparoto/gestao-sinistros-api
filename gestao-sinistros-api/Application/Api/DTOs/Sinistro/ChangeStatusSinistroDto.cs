using System.ComponentModel.DataAnnotations;
using gestao_sinistros_api.Application.Domain.Enums;

namespace gestao_sinistros_api.Application.Api.DTOs.Sinistro
{
    public class ChangeStatusSinistroDto
    {
        [Required]
        public SinistroStatus Status { get; set; }

        public decimal? ValorAprovado { get; set; }

        public string? MotivoNegacao { get; set; }

        public string? Observacao { get; set; }
    }
}
