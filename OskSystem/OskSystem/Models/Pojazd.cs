namespace OskSystem.Models
{
    public class Pojazd
    {
        public int Id { get; set; }
        public string Marka { get; set; } = "";
        public string ModelPojazdu { get; set; } = "";
        public string NumerRejestracyjny { get; set; } = "";
        // Kategoria pojazdu musi pasować do kategorii kursanta, który nim jeździ.
        public string Kategoria { get; set; } = "";
        public string PelneDane => $"{Marka} {ModelPojazdu} ({NumerRejestracyjny}) [Kat. {Kategoria}]";
    }
}
