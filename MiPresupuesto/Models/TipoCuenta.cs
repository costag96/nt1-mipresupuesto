using Microsoft.AspNetCore.Mvc;
using MiPresupuesto.Validations;
using System.ComponentModel.DataAnnotations;

namespace MiPresupuesto.Models
{
    public class TipoCuenta
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El campo es requeredio")]
        [PrimerLetraMayus]
        [Remote(action: "ExisteTipoCuenta",controller: "TiposCuentas")]
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }
    }
}
