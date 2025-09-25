namespace ETLProyecto.Models
{
    public class Producto
    {
        public int ProductID { get; set; }          
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
