using ParteFrontProyectoDSW1.Contracts.Dtos;
using ParteFrontProyectoDSW1.Repositories;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;


namespace ParteFrontProyectoDSW1.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _connectionString;
        public UsuarioRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Default")!;
        }

        public async Task<IEnumerable<UsuarioLoginDto>> ListarAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<UsuarioLoginDto>("sp_Usuario_Listar", commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertarAsync(UsuarioLoginDto usuario)
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.ExecuteScalarAsync<int>(
                "sp_Usuario_Insertar",
                new { usuario.Nombre, usuario.Email, usuario.PasswordHash },
                commandType: CommandType.StoredProcedure);
        }

        public async Task EditarAsync(UsuarioLoginDto usuario)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(
                "sp_Usuario_Editar",
                new { usuario.IdUsuario, usuario.Nombre, usuario.Email, usuario.Activo },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<UsuarioLoginDto?> LoginAsync(string email, string passwordHash)
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<UsuarioLoginDto>(
                "sp_Usuario_Login",
                new { Email = email, PasswordHash = passwordHash },
                commandType: CommandType.StoredProcedure);
        }
    }
}