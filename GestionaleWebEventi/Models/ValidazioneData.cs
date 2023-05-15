using System;
using System.ComponentModel.DataAnnotations;

namespace GestionaleWebEventi.Models
{
	public class ValidazioneData : ValidationAttribute
	{
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dataInserita = (DateTime)value;
            DateTime dataAttuale = DateTime.Now;

            if (dataInserita < dataAttuale)
            {
                return new ValidationResult("La data deve essere successiva alla data attuale.");
            }

            return ValidationResult.Success;
        }
    }
}

