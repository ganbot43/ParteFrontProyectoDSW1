using ParteFrontProyectoDSW1.Contracts.Entities;
using ParteFrontProyectoDSW1.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ParteFrontProyectoDSW1.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly string _connectionString;
        public ProductoRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Default")!;
        }

        public async Task<IEnumerable<Producto>> ListarAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<Producto>("sp_Producto_Listar", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Producto>> ListarPorCategoriaAsync(int idCategoria)
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<Producto>(
                "sp_Producto_ListarPorCategoria",
                new { IdCategoria = idCategoria },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertarAsync(Producto producto)
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.ExecuteScalarAsync<int>(
                "sp_Producto_Insertar",
                new { producto.SKU, producto.Nombre, producto.Descripcion, producto.Precio, producto.Stock, producto.IdCategoria },
                commandType: CommandType.StoredProcedure);
        }

        public async Task EditarAsync(Producto producto)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(
                "sp_Producto_Editar",
                new { producto.IdProducto, producto.Nombre, producto.Descripcion, producto.Precio, producto.Stock, producto.IdCategoria, producto.Activo },
                commandType: CommandType.StoredProcedure);
        }
    }
}