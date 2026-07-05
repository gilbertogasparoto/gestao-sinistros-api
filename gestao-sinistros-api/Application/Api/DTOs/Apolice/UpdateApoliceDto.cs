using System.ComponentModel.DataAnnotations;
using gestao_sinistros_api.Application.Domain.Enums;

namespace gestao_sinistros_api.Application.Api.DTOs
{
    public class UpdateApoliceDto
    {
        [Required]
        public TipoSeguro TipoSeguro { get; set; }

        [Required]
        public bool Ativo { get; set; }

        [Required]
        public DateTime DataInicio { get; set; }
        [Required]
        public DateTime DataFim { get; set; }
    }
}