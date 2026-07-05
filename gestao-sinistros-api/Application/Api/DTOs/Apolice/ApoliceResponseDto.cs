using gestao_sinistros_api.Application.Domain.Enums;

namespace gestao_sinistros_api.Application.Api.DTOs
{
    public class ApoliceResponseDto
    {
        public Guid Id { get; set; }
        public string Numero { get; set; }
        public Guid ClienteId { get; set; }
        public TipoSeguro TipoSeguro { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}