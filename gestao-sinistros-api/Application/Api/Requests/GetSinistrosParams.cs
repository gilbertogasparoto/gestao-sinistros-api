using gestao_sinistros_api.Application.Domain.Enums;

public class GetSinistrosParams : PaginationParams
{
    public string? Keyword { get; set; }

    public SinistroStatus? Status { get; set; }

    public DateTime? DataInicio { get; set; }

    public DateTime? DataFim { get; set; }
}