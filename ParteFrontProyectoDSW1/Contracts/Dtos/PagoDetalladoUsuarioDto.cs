namespace ParteFrontProyectoDSW1.Contracts.Dtos
{
    public class PagoDetalladoUsuarioDto
    {
        public int IdPago { get; set; }
        public int IdOrden { get; set; }
        public int IdUsuario { get; set; }
        public string Usuario { get; set; } = string.Empty;

        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string Metodo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        public string Producto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal SubTotal { get; set; }
    }
}
