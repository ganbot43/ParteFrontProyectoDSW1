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
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly string _connectionString;
        public CategoriaRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Default")!;
        }

        public async Task<IEnumerable<Categoria>> ListarAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<Categoria>("sp_Categoria_Listar", commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertarAsync(Categoria categoria)
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.ExecuteScalarAsync<int>(
                "sp_Categoria_Insertar",
                new { categoria.Codigo, categoria.Nombre, categoria.Descripcion },
                commandType: CommandType.StoredProcedure);
        }

        public async Task EditarAsync(Categoria categoria)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(
                "sp_Categoria_Editar",
                new { categoria.IdCategoria, categoria.Nombre, categoria.Descripcion, categoria.Activo  },
                commandType: CommandType.StoredProcedure);
        }
    }
}