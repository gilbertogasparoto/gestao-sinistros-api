namespace gestao_sinistros_api.Application.Api.Commom;

public class BusinessException : Exception
{
    public BusinessException(string message)
        : base(message)
    {
    }
}