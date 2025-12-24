using ParteFrontProyectoDSW1.Contracts.Entities;

namespace ParteFrontProyectoDSW1.Repositories.Interfaces
{
    public interface IOrdenRepository
    {
        Task GenerarOrdenAsync(int idUsuario);
        Task<IEnumerable<Orden>> ListarPorUsuarioAsync(int idUsuario);
    }
}
