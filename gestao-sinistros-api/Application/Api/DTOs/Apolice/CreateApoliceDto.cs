using System.ComponentModel.DataAnnotations;
using gestao_sinistros_api.Application.Domain.Enums;

namespace gestao_sinistros_api.Application.Api.DTOs
{
    public class CreateApoliceDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Numero { get; set; }

        [Required]
        public Guid ClienteId { get; set; }

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