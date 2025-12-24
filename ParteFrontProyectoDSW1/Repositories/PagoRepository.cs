using ParteFrontProyectoDSW1.Contracts.Dtos;
using ParteFrontProyectoDSW1.Contracts.Entities;
using ParteFrontProyectoDSW1.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ParteFrontProyectoDSW1.Repositories
{
    public class PagoRepository : IPagoRepository
    {
        private readonly string _connectionString;
        public PagoRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Default")!;
        }

        public async Task RegistrarPagoAsync(int idOrden, decimal monto, string metodo)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(
                "sp_Pago_Registrar",
                new { IdOrden = idOrden, Monto = monto, Metodo = metodo },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Pago>> ListarAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<Pago>("sp_Pago_Listar", commandType: CommandType.StoredProcedure);
        }

       public async Task<IEnumerable<PagoDetalladoUsuarioDto>> ListarPorUsuarioAsync(int idUsuario)
{
    using var conn = new SqlConnection(_connectionString);
    return await conn.QueryAsync<PagoDetalladoUsuarioDto>(
        "sp_Pago_ListarPorUsuario",
        new { IdUsuario = idUsuario },
        commandType: CommandType.StoredProcedure);
}

        public async Task<IEnumerable<PagoDetalladoDto>> ListarDetalladoAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<PagoDetalladoDto>(
                "sp_Pago_ListarDetallado",
                commandType: CommandType.StoredProcedure);
        }
    }
}
