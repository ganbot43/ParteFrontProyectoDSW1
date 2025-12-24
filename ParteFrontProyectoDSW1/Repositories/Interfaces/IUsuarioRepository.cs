using ParteFrontProyectoDSW1.Contracts.Dtos;

namespace ParteFrontProyectoDSW1.Repositories
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<UsuarioLoginDto>> ListarAsync();
        Task<int> InsertarAsync(UsuarioLoginDto usuario);
        Task EditarAsync(UsuarioLoginDto usuario);
        Task<UsuarioLoginDto?> LoginAsync(string email, string passwordHash);

    }
}