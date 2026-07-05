namespace gestao_sinistros_api.Application.Api.DTOs.Cliente
{
    public class ClienteResponseDto
    {
        public Guid Id { get; set; }

        public string Nome { get; set; }

        public string Documento { get; set; }

        public string Email { get; set; }

        public string Telefone { get; set; }
    }
}