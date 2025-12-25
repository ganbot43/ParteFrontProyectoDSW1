namespace ParteFrontProyectoDSW1.Contracts.Entities
{
    public class CarritoDetalle
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal SubTotal { get; set; }
    }
}
