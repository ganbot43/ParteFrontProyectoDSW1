using ParteFrontProyectoDSW1.Contracts.Dtos;
using ParteFrontProyectoDSW1.Contracts.Entities;

namespace ParteFrontProyectoDSW1.Repositories.Interfaces
{
    public interface IPagoRepository
    {
        Task RegistrarPagoAsync(int idOrden, decimal monto, string metodo);

        //parte admin
        Task<IEnumerable<Pago>> ListarAsync();
        Task<IEnumerable<PagoDetalladoDto>> ListarDetalladoAsync();

        // parte cliente
        Task<IEnumerable<PagoDetalladoUsuarioDto>> ListarPorUsuarioAsync(int idUsuario);
    }
}
