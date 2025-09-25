namespace ETLProyecto.Models
{
    public class Cliente
    {
        public int CustomerID { get; set; }       
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}
