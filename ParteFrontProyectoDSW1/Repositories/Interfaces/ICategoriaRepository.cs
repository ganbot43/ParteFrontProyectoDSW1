using ParteFrontProyectoDSW1.Contracts.Entities;

namespace ParteFrontProyectoDSW1.Repositories.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> ListarAsync();
        Task<int> InsertarAsync(Categoria categoria);
        Task EditarAsync(Categoria categoria);
    }
}
