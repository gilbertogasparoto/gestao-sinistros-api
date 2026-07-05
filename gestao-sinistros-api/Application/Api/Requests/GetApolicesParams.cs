using gestao_sinistros_api.Application.Domain.Enums;

public class GetApolicesParams : PaginationParams
{
    public string? Keyword { get; set; }

    public RamoSeguro? RamoSeguro { get; set; }

    public bool? Ativo { get; set; }
}