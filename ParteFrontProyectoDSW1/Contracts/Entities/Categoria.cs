namespace apiModeloExamen.Contracts.Entities
{
    public class Categoria
    {
        public int IdCategoria { get; set; }
        public string Codigo { get; set; } = default!;
        public string Nombre { get; set; } = default!;
        public string? Descripcion { get; set; }

        public bool Activo { get; set; }
    }
}
