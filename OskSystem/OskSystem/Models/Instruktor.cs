namespace OskSystem.Models
{
    public class Instruktor
    {
        public int Id { get; set; }
        public string Imie { get; set; } = "";
        public string Nazwisko { get; set; } = "";
        public string NumerLegitymacji { get; set; } = "";
        public string Telefon { get; set; } = "";
        // Kategorie, których instruktor może uczyć — wpisane po przecinku, np. "B, C, D".
        public string Uprawnienia { get; set; } = "";
        public string Login { get; set; } = "";
        // Hasło trzymamy zakodowane, nigdy jako zwykły tekst.
        public string Haslo { get; set; } = "";
        public string PelneDane => $"{Imie} {Nazwisko} (Kat: {Uprawnienia})";
    }
}
