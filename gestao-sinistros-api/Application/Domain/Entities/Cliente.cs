namespace gestao_sinistros_api.Application.Domain.Entities
{
    public class Cliente : BaseEntity
    {
        public string Nome { get; set; } = string.Empty;

        public string Documento { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Telefone { get; set; } = string.Empty;

        public ICollection<Apolice> Apolices { get; set; } = new List<Apolice>();
    }
}