using ParteFrontProyectoDSW1.Contracts.Entities;
using ParteFrontProyectoDSW1.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;

namespace apiModeloExamen.Repositories
{
    public class CarritoRepository : ICarritoRepository
    {
        private readonly string _connectionString;
        public CarritoRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Default")!;
        }

        public async Task<int> ObtenerCarritoActivoAsync(int idUsuario)
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.ExecuteScalarAsync<int>(
                "sp_Carrito_ObtenerActivo",
                new { IdUsuario = idUsuario },
                commandType: CommandType.StoredProcedure);
        }

        public async Task AgregarProductoAsync(int idCarrito, int idProducto, int cantidad)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(
                "sp_Carrito_AgregarProducto",
                new { IdCarrito = idCarrito, IdProducto = idProducto, Cantidad = cantidad },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<CarritoDetalle>> VerCarritoAsync(int idCarrito)
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<CarritoDetalle>(
                "sp_Carrito_Ver",
                new { IdCarrito = idCarrito },
                commandType: CommandType.StoredProcedure);
        }
    }
}
