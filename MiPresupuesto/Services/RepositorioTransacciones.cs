using Dapper;
using MiPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace MiPresupuesto.Services
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, float montoAnterior, int cuentaAnterior);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
        Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroTransaccionesPorUsuario modelo);
    }

    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string connectionString;

        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("InsertarTransacciones",
                new
                {
                    transaccion.UsuarioId,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Descripcion
                },
                commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id = id;
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(
            ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>
                (@"SELECT T.Id, T.Monto, T.FechaTransaccion, C.Nombre as Categoria,
                    CUENTAS.Nombre as Cuenta, C.TipoOperacionId
                    FROM TRANSACCIONES as T
                    INNER JOIN CATEGORIAS as C
                    ON C.Id = T.CategoriaId
                    INNER JOIN CUENTAS
                    ON CUENTAS.Id = T.CuentaId
                    WHERE T.CuentaId = @CuentaId AND T.UsuarioId = @UsuarioId
                    AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin", modelo);

        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(
            ParametroTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>
                (@"SELECT T.Id, T.Monto, T.FechaTransaccion, C.Nombre as Categoria,
                    CUENTAS.Nombre as Cuenta, C.TipoOperacionId
                    FROM TRANSACCIONES as T
                    INNER JOIN CATEGORIAS as C
                    ON C.Id = T.CategoriaId
                    INNER JOIN CUENTAS
                    ON CUENTAS.Id = T.CuentaId
                    WHERE T.UsuarioId = @UsuarioId
                    AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
                    ORDER BY T.FechaTransaccion DESC", modelo);

        }

        public async Task Actualizar(Transaccion transaccion, float montoAnterior,
            int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("ActualizarTransacciones",
                new
                {
                    transaccion.Id,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Descripcion,
                    montoAnterior,
                    cuentaAnteriorId
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(
                @"SELECT TRANSACCIONES.*, CAT.TipoOperacionId
                FROM TRANSACCIONES
                INNER JOIN CATEGORIAS as CAT
                ON CAT.Id = TRANSACCIONES.CategoriaId
                WHERE Transacciones.Id = @Id AND Transacciones.UsuarioId = @UsuarioId",
                new { id, usuarioId });
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("BorrarTransacciones",
                new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
