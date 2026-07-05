using gestao_sinistros_api.Application.Domain.Enums;

public class GetApolicesParams : PaginationParams
{
    public string? Keyword { get; set; }

    public TipoSeguro? TipoSeguro { get; set; }

    public bool? Ativo { get; set; }
}