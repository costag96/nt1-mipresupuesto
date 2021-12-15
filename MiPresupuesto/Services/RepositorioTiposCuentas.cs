using Dapper;
using Microsoft.Data.SqlClient;
using MiPresupuesto.Models;

namespace MiPresupuesto.Services
{
    public interface IRepositorioTiposCuentas
    {
        Task ActualizarTiposCuenta(TipoCuenta tipoCuenta);
        Task BorrarTipoCUenta(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<TipoCuenta> ObtenerTipoCuentaPorId(int id, int usuarioId);
        Task<IEnumerable<TipoCuenta>> ObtenerTiposCuenta(int usuarioId);
    }

    public class RepositorioTiposCuentas: IRepositorioTiposCuentas
    {

        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);

            //query que devuelve un solo resultado
            var id = await connection.QuerySingleAsync<int>
                                                (@"INSERT INTO TIPOSCUENTAS (Nombre, UsuarioId, Orden)
                                                    VALUES (@Nombre, @UsuarioId, 0);
                                                    SELECT SCOPE_IDENTITY();", tipoCuenta);
            tipoCuenta.Id = id;
        }

        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>
                                                 (@"SELECT 1
                                                    FROM TIPOSCUENTAS
                                                    WHERE Nombre=@Nombre AND UsuarioId = @UsuarioId;",
                                                    new {nombre, usuarioId});
            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> ObtenerTiposCuenta(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                             FROM TIPOSCUENTAS
                                                                WHERE UsuarioId = @UsuarioID", new {usuarioId});
        }
        public async Task ActualizarTiposCuenta(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TIPOSCUENTAS
                                            SET Nombre = @Nombre
                                            WHERE Id = @Id", tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerTipoCuentaPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"
                                                             SELECT Id, Nombre, Orden
                                                             FROM TIPOSCUENTAS
                                                             WHERE Id = @Id AND UsuarioId = @UsuarioID",
                                                             new { id, usuarioId });
        }

        public async Task BorrarTipoCUenta(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE TIPOSCUENTAS WHERE Id = @Id", new { id });
        }
    }
}
