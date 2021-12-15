namespace MiPresupuesto.Models
{
    public class IndiceCuentaViewModel
    {
        public string TipoCuenta { get; set; }
        public IEnumerable<Cuenta> Cuentas { get; set; }
        public float Balance => Cuentas.Sum(x => x.Balance);

    }
}
