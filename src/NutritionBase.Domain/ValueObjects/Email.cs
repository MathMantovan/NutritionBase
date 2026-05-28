using NutritionBase.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NutritionBase.Domain.ValueObjects
{
    public class Email
    {
        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }
        public static Email Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new DomainException("O endereço de e-mail não pode ser vazio.");
            }

            // Regex para validação de e-mail (simplificado para exemplo)
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
            {
                throw new DomainException($"O formato do e-mail '{email}' é inválido.");
            }

            return new Email(email);
        }
        public override string ToString() => Value;

    }
}
