namespace ParteFrontProyectoDSW1.Contracts.Dtos
{
    public class PagoProductoDto
    {
        public string Producto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal SubTotal => PrecioUnitario * Cantidad;
    }
}
