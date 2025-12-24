namespace ParteFrontProyectoDSW1.Contracts.Entities
{
    public class Carrito
    {
        public int IdCarrito { get; set; }
        public int IdUsuario { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }
}
