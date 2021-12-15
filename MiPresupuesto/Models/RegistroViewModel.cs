using System.ComponentModel.DataAnnotations;

namespace MiPresupuesto.Models
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El campo es requerido")]
        [EmailAddress(ErrorMessage = "Debe ser un correo electrónico válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo es requerido")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

