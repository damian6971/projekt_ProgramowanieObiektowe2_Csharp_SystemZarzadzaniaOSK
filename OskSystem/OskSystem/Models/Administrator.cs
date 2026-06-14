namespace OskSystem.Models
{
    public class Administrator
    {
        public int Id { get; set; }
        public string Login { get; set; } = "";
        // Hasło trzymamy zakodowane, nigdy jako zwykły tekst.
        public string Haslo { get; set; } = "";
        public string Imie { get; set; } = "";
    }
}
