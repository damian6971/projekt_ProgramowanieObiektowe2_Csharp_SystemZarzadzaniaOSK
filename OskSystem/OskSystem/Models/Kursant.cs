namespace OskSystem.Models
{
    public class Kursant
    {
        public int Id { get; set; }
        public string Imie { get; set; } = "";
        public string Nazwisko { get; set; } = "";
        // Numer PESEL jest wymagany i nie może się powtarzać wśród kursantów.
        public string Pesel { get; set; } = "";
        public string Login { get; set; } = "";
        // Hasło trzymamy zakodowane, nigdy jako zwykły tekst.
        public string Haslo { get; set; } = "";
        // Kategoria prawa jazdy, na którą kursant uczęszcza na zajęcia.
        public string Kategoria { get; set; } = "B";
        public string PelneDane => $"{Imie} {Nazwisko} (Kat. {Kategoria})";
    }
}
