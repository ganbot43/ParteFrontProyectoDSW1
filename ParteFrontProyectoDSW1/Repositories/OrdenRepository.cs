using ParteFrontProyectoDSW1.Contracts.Entities;
using ParteFrontProyectoDSW1.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;

namespace ParteFrontProyectoDSW1.Repositories
{
    public class OrdenRepository : IOrdenRepository
    {
        private readonly string _connectionString;
        public OrdenRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Default")!;
        }

        public async Task GenerarOrdenAsync(int idUsuario)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(
                "sp_Orden_Generar",
                new { IdUsuario = idUsuario },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Orden>> ListarPorUsuarioAsync(int idUsuario)
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<Orden>(
                "sp_Orden_ListarPorUsuario",
                new { IdUsuario = idUsuario },
                commandType: CommandType.StoredProcedure);
        }
    }
}
