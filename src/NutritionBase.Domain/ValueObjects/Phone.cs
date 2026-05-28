using NutritionBase.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutritionBase.Domain.ValueObjects
{
    public class Phone
    {
        public string AreaCode { get; private set; }
        public string Number { get; private set; }

        private Phone(string areaCode, string number)
        {
            AreaCode = areaCode;
            Number = number;
        }
        public static Phone Create(string areaCode, string number)
        {
            if (string.IsNullOrWhiteSpace(areaCode))
            {
                throw new DomainException("O código de área não pode ser vazio.");
            }
            if (areaCode.Length != 2 || !areaCode.All(char.IsDigit))
            {
                throw new DomainException($"O código de área '{areaCode}' deve ter 2 dígitos numéricos.");
            }

            if (string.IsNullOrWhiteSpace(number))
            {
                throw new DomainException("O número de telefone não pode ser vazio.");
            }
            if (number.Length < 8 || number.Length > 9 || !number.All(char.IsDigit))
            {
                throw new DomainException($"O número de telefone '{number}' deve ter entre 8 e 9 dígitos numéricos.");
            }

            return new Phone(areaCode, number);
        }
    }
}
