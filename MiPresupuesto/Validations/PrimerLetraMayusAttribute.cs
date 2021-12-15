using System.ComponentModel.DataAnnotations;

namespace MiPresupuesto.Validations
{
    public class PrimerLetraMayusAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            } 

            var primerLetra = value.ToString()[0].ToString();
            if(primerLetra != primerLetra.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser en mayuscula");
            }

            return ValidationResult.Success;
        }
    }
}
