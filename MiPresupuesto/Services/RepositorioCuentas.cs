using Dapper;
using Microsoft.Data.SqlClient;
using MiPresupuesto.Models;

namespace MiPresupuesto.Services
{

    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> BuscarCuenta(int usuarioId);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    }

    public class RepositorioCuentas: IRepositorioCuentas
    {
        private readonly string connectionString;

        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                            @"INSERT INTO CUENTAS (Nombre, TipoCuentaId, Balance, Descripcion)
                            VALUES (@Nombre, @TipoCuentaId, @Balance, @Descripcion);

                            SELECT  SCOPE_IDENTITY();", cuenta);

            cuenta.Id = id;
        }

        public async Task<IEnumerable<Cuenta>> BuscarCuenta(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuenta>(@"
                                    SELECT Cuentas.Id, Cuentas.Nombre, Cuentas.Descripcion, Balance, TC.Nombre AS TipoCuenta
                                    FROM CUENTAS
                                    INNER JOIN TIPOSCUENTAS as TC
                                    ON TC.Id = CUENTAS.TipoCuentaId
                                    WHERE TC.UsuarioId = @UsuarioId
                                    ", new { usuarioId });
        }

        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(
                @"SELECT Cuentas.Id, Cuentas.Nombre, Balance, Descripcion, TipoCuentaId
                FROM CUENTAS
                INNER JOIN TIPOSCUENTAS as TC
                ON TC.Id = Cuentas.TipoCuentaId
                WHERE TC.UsuarioId = @UsuarioId AND Cuentas.Id = @Id", new { id, usuarioId });
        }

        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE CUENTAS
                                            SET Nombre = @Nombre, Balance = @Balance, Descripcion = @Descripcion,
                                            TipoCuentaId = @TipoCuentaId
                                            WHERE Id = @Id;", cuenta);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE CUENTAS WHERE Id = @Id", new { id });
        }
    }

    
}
