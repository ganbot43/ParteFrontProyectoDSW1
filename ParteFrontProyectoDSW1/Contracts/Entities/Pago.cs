namespace ParteFrontProyectoDSW1.Contracts.Entities
{
    public class Pago
    {
        public int IdPago { get; set; }
        public int IdOrden { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string Metodo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}
