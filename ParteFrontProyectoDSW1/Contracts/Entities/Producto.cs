namespace ParteFrontProyectoDSW1.Contracts.Entities
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string SKU { get; set; } = default!;
        public string Nombre { get; set; } = default!;
        public string Descripcion { get; set; } = default!;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string? ImagenUrl { get; set; }
        public int IdCategoria { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
