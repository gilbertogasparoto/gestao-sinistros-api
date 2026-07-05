using System.ComponentModel.DataAnnotations;

namespace gestao_sinistros_api.Application.Api.DTOs.Cliente
{
    public class UpsertClienteDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Nome { get; set; }

        [Required]
        [StringLength(14, MinimumLength = 11)]
        public string Documento { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 10)]
        public string Telefone { get; set; }
    }
}