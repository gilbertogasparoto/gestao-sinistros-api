using System.ComponentModel.DataAnnotations;
using gestao_sinistros_api.Application.Domain.Enums;

namespace gestao_sinistros_api.Application.Api.DTOs.Sinistro
{
    public class UpdateSinistroDto
    {
        [Required]
        [StringLength(500, MinimumLength = 5)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        public DateTime DataOcorrencia { get; set; }

        [Required]
        public decimal ValorEstimado { get; set; }
    }
}
