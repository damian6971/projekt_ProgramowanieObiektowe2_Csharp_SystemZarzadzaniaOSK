using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BCrypt.Net;
using OskSystem.Data;
using OskSystem.Models;

namespace OskSystem.ViewModels
{
    // Jedna kategoria prawa jazdy — przechowuje nazwę i czy jest zaznaczona.
    public class KategoriaCheck : ViewModelBase
    {
        public string Nazwa { get; set; } = "";
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }
    }

    public class InstruktorzyViewModel : ViewModelBase
    {
        private ObservableCollection<Instruktor> _listaInstruktorow = new();
        public ObservableCollection<Instruktor> ListaInstruktorow
        {
            get => _listaInstruktorow;
            set { _listaInstruktorow = value; OnPropertyChanged(); }
        }

        // Lista wszystkich kategorii z zaznaczeniem tych, które posiada dany instruktor.
        public ObservableCollection<KategoriaCheck> DostepneUprawnienia { get; set; } = new();

        private string _nowyImie = ""; public string NowyImie { get => _nowyImie; set { _nowyImie = value; OnPropertyChanged(); } }
        private string _nowyNazwisko = ""; public string NowyNazwisko { get => _nowyNazwisko; set { _nowyNazwisko = value; OnPropertyChanged(); } }
        private string _nowyNumerLegitymacji = ""; public string NowyNumerLegitymacji { get => _nowyNumerLegitymacji; set { _nowyNumerLegitymacji = value; OnPropertyChanged(); } }
        private string _nowyTelefon = ""; public string NowyTelefon { get => _nowyTelefon; set { _nowyTelefon = value; OnPropertyChanged(); } }
        private string _nowyLogin = ""; public string NowyLogin { get => _nowyLogin; set { _nowyLogin = value; OnPropertyChanged(); } }
        private string _nowyHaslo = ""; public string NowyHaslo { get => _nowyHaslo; set { _nowyHaslo = value; OnPropertyChanged(); } }

        public InstruktorzyViewModel()
        {
            ZbudujListeKategorii();
            ZaladujInstruktorow();
        }

        public void ZbudujListeKategorii()
        {
            var kategorie = new string[] { "AM", "A1", "A2", "A", "B", "B+E", "C", "C+E", "D", "D+E", "T" };
            DostepneUprawnienia = new ObservableCollection<KategoriaCheck>();
            foreach (var kat in kategorie)
                DostepneUprawnienia.Add(new KategoriaCheck { Nazwa = kat, IsSelected = false });
        }

        public void ZaladujInstruktorow()
        {
            try
            {
                using var context = new OskDbContext();
                ListaInstruktorow = new ObservableCollection<Instruktor>(context.Instruktorzy.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się wczytać listy instruktorów: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Zapisuje instruktora do bazy — okno formularza zamyka się tylko przy udanym zapisie.
        public bool ZapiszDoBazy(Instruktor? edytowany = null)
        {
            if (string.IsNullOrWhiteSpace(NowyLogin))
            {
                MessageBox.Show("Pole Login jest wymagane!", "Brak danych", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Przy dodawaniu instruktora hasło jest obowiązkowe — bez niego nie da się zalogować.
            if (edytowany == null && string.IsNullOrWhiteSpace(NowyHaslo))
            {
                MessageBox.Show("Hasło jest wymagane przy dodawaniu nowego instruktora!", "Brak hasła", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            var zaznaczone = DostepneUprawnienia.Where(k => k.IsSelected).Select(k => k.Nazwa);
            string uprawnieniaTekst = string.Join(", ", zaznaczone);

            try
            {
                using var context = new OskDbContext();

                if (edytowany == null)
                {
                    if (context.Instruktorzy.Any(i => i.Login == NowyLogin))
                    {
                        MessageBox.Show($"Login '{NowyLogin}' jest już zajęty!", "Zajęty login", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }

                    context.Instruktorzy.Add(new Instruktor
                    {
                        Imie = NowyImie,
                        Nazwisko = NowyNazwisko,
                        NumerLegitymacji = NowyNumerLegitymacji,
                        Telefon = NowyTelefon,
                        Login = NowyLogin.Trim(),
                        Haslo = BCrypt.Net.BCrypt.HashPassword(NowyHaslo),
                        Uprawnienia = uprawnieniaTekst
                    });
                }
                else
                {
                    if (context.Instruktorzy.Any(i2 => i2.Login == NowyLogin && i2.Id != edytowany.Id))
                    {
                        MessageBox.Show($"Login '{NowyLogin}' jest już zajęty!", "Zajęty login", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }

                    var i = context.Instruktorzy.Find(edytowany.Id);
                    if (i == null)
                    {
                        MessageBox.Show("Ten instruktor nie istnieje już w bazie danych. Odśwież listę.", "Błąd zapisu", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    i.Imie = NowyImie;
                    i.Nazwisko = NowyNazwisko;
                    i.NumerLegitymacji = NowyNumerLegitymacji;
                    i.Telefon = NowyTelefon;
                    i.Login = NowyLogin.Trim();
                    i.Uprawnienia = uprawnieniaTekst;

                    // Hasło zmienia się tylko wtedy, gdy admin wpisał nowe — puste pole = bez zmian.
                    if (!string.IsNullOrWhiteSpace(NowyHaslo))
                        i.Haslo = BCrypt.Net.BCrypt.HashPassword(NowyHaslo);
                }

                context.SaveChanges();
                ZaladujInstruktorow();
                WyczyscPola();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się zapisać danych: {ex.Message}", "Błąd zapisu", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public void UsunInstruktora(Instruktor instruktor)
        {
            var wynik = MessageBox.Show($"Czy usunąć instruktora {instruktor.Imie} {instruktor.Nazwisko}?", "Potwierdzenie usunięcia", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (wynik != MessageBoxResult.Yes) return;

            try
            {
                using var context = new OskDbContext();

                // Blokada gdy instruktor ma jeszcze zaplanowane jazdy w przyszłości.
                bool maZaplanowane = context.RezerwacjeJazd.Any(r => r.InstruktorId == instruktor.Id && r.Status == "Zaplanowana");
                if (maZaplanowane)
                {
                    MessageBox.Show($"Nie można usunąć instruktora {instruktor.Imie} {instruktor.Nazwisko} — ma zaplanowane jazdy.\nNajpierw odwołaj wszystkie jego rezerwacje.", "Nie można usunąć", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Jeśli instruktor ma tylko historyczne (odwołane) rezerwacje — zapytaj czy usunąć historię.
                bool maHistorie = context.RezerwacjeJazd.Any(r => r.InstruktorId == instruktor.Id);
                if (maHistorie)
                {
                    var pot = MessageBox.Show(
                        $"Instruktor {instruktor.Imie} {instruktor.Nazwisko} ma historyczne (odwołane) jazdy.\nCzy usunąć instruktora WRAZ z historią jazd?",
                        "Usuń historię?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (pot != MessageBoxResult.Yes) return;

                    // Najpierw usuwamy jego jazdy, bo inaczej baza zgłosi błąd.
                    var historia = context.RezerwacjeJazd.Where(r => r.InstruktorId == instruktor.Id).ToList();
                    context.RezerwacjeJazd.RemoveRange(historia);
                }

                var doUsuniecia = context.Instruktorzy.Find(instruktor.Id);
                if (doUsuniecia != null)
                {
                    context.Instruktorzy.Remove(doUsuniecia);
                    context.SaveChanges();
                }

                ZaladujInstruktorow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się usunąć instruktora: {ex.Message}", "Błąd usuwania", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WyczyscPola()
        {
            NowyImie = ""; NowyNazwisko = ""; NowyNumerLegitymacji = ""; NowyTelefon = ""; NowyLogin = ""; NowyHaslo = "";
            foreach (var k in DostepneUprawnienia) k.IsSelected = false;
        }
    }
}
