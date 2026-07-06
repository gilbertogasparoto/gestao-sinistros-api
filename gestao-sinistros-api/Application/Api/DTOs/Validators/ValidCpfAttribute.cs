using System.ComponentModel.DataAnnotations;

namespace gestao_sinistros_api.Application.Api.DTOs.Validators
{
    public class ValidCpfAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;

            string cpf = value.ToString().Trim().Replace(".", "").Replace("-", "");

            if (cpf.Length != 11 || !cpf.All(char.IsDigit))
                return false;

            if (cpf.Distinct().Count() == 1)
                return false;

            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += int.Parse(cpf[i].ToString()) * (10 - i);

            int remainder = sum % 11;
            int firstDigit = remainder < 2 ? 0 : 11 - remainder;

            if (int.Parse(cpf[9].ToString()) != firstDigit)
                return false;

            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += int.Parse(cpf[i].ToString()) * (11 - i);

            remainder = sum % 11;
            int secondDigit = remainder < 2 ? 0 : 11 - remainder;

            if (int.Parse(cpf[10].ToString()) != secondDigit)
                return false;

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"O campo {name} deve conter um CPF válido.";
        }
    }
}
