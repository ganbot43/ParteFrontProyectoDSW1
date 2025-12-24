using ParteFrontProyectoDSW1.Contracts.Entities;

namespace ParteFrontProyectoDSW1.Repositories.Interfaces
{
    public interface ICarritoRepository
    {
        Task<int> ObtenerCarritoActivoAsync(int idUsuario);
        Task AgregarProductoAsync(int idCarrito, int idProducto, int cantidad);
        Task<IEnumerable<CarritoDetalle>> VerCarritoAsync(int idCarrito);
    }
}
