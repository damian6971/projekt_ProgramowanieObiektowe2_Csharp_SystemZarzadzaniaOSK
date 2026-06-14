using System;

namespace OskSystem.Models
{
    public class RezerwacjaJazdy
    {
        public int Id { get; set; }
        public DateTime Termin { get; set; }
        // Jazda trwa standardowo 3 godziny — ta wartość musi być zgodna z widokiem kalendarza.
        public int CzasTrwaniaGodziny { get; set; } = 3;
        // Możliwe stany rezerwacji: "Zaplanowana" lub "Odwołana".
        public string Status { get; set; } = "Zaplanowana";
        public int KursantId { get; set; }
        public virtual Kursant? Kursant { get; set; }
        public int InstruktorId { get; set; }
        public virtual Instruktor? Instruktor { get; set; }
        public int PojazdId { get; set; }
        public virtual Pojazd? Pojazd { get; set; }
    }
}
