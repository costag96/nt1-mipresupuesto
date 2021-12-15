namespace MiPresupuesto.Models
{
    public class TransaccionActualizacionViewModel: TransaccionCreacionViewModel
    {
        public int CuentaAnteriorId { get; set; }
        public float MontoAnterior { get; set; }    

        public string UrlRetorno { get; set; }  
    }
}
