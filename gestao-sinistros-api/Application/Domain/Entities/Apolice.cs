using gestao_sinistros_api.Application.Domain.Enums;

namespace gestao_sinistros_api.Application.Domain.Entities
{
    public class Apolice : BaseEntity
    {
        public string Numero { get; set; } = string.Empty;

        public Guid ClienteId { get; set; }

        public Cliente Cliente { get; set; } = null!;

        public RamoSeguro RamoSeguro { get; set; }

        public bool Ativo { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public ICollection<Sinistro> Sinistros { get; set; } = new List<Sinistro>();
    }
}