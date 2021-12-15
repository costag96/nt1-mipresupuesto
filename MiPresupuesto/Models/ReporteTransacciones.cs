namespace MiPresupuesto.Models
{
    public class ReporteTransacciones
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public IEnumerable<TransaccionesPorFecha> TransaccionesAgrupadas { get; set; }
        public float BalanceDepositos => TransaccionesAgrupadas.Sum(x => x.BalanceDepositos);
        public float BalanceRetiros => TransaccionesAgrupadas.Sum(x => x.BalanceRetiros);
        public float Total => BalanceDepositos - BalanceRetiros;

        public class TransaccionesPorFecha
        {
            public DateTime FechaTransaccion { get; set; }
            public IEnumerable<Transaccion> Transacciones { get; set; }
            public float BalanceDepositos =>
                Transacciones.Where(x => x.TipoOperacionId == TipoOperacion.Ingreso)
                .Sum(x => x.Monto);
            public float BalanceRetiros =>
                Transacciones.Where(x => x.TipoOperacionId == TipoOperacion.Gasto)
                .Sum(x => x.Monto);
        }
    }
}
