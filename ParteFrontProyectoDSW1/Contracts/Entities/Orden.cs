namespace apiModeloExamen.Contracts.Entities
{
    public class Orden
    {
        public int IdOrden { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaOrden { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
