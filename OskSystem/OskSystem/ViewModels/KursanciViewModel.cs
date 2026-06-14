using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using BCrypt.Net;
using OskSystem.Data;
using OskSystem.Models;

namespace OskSystem.ViewModels
{
    public class KursanciViewModel : ViewModelBase
    {
        private ObservableCollection<Kursant> _listaKursantow = new();
        public ObservableCollection<Kursant> ListaKursantow
        {
            get => _listaKursantow;
            set { _listaKursantow = value; OnPropertyChanged(); }
        }

        // Wszystkie kategorie prawa jazdy dostępne do wyboru przy rejestracji kursanta.
        public ObservableCollection<string> DostepneKategorie { get; } = new ObservableCollection<string>
            { "AM", "A1", "A2", "A", "B1", "B", "B+E", "C1", "C1+E", "C", "C+E", "D1", "D1+E", "D", "D+E", "T" };

        private string _nowyImie = ""; public string NowyImie { get => _nowyImie; set { _nowyImie = value; OnPropertyChanged(); } }
        private string _nowyNazwisko = ""; public string NowyNazwisko { get => _nowyNazwisko; set { _nowyNazwisko = value; OnPropertyChanged(); } }
        private string _nowyPesel = ""; public string NowyPesel { get => _nowyPesel; set { _nowyPesel = value; OnPropertyChanged(); } }
        private string _nowyLogin = ""; public string NowyLogin { get => _nowyLogin; set { _nowyLogin = value; OnPropertyChanged(); } }
        private string _nowyHaslo = ""; public string NowyHaslo { get => _nowyHaslo; set { _nowyHaslo = value; OnPropertyChanged(); } }
        private string _wybranaKategoria = "B"; public string WybranaKategoria { get => _wybranaKategoria; set { _wybranaKategoria = value; OnPropertyChanged(); } }

        public KursanciViewModel() { ZaladujKursantow(); }

        public void ZaladujKursantow()
        {
            try
            {
                using var context = new OskDbContext();
                ListaKursantow = new ObservableCollection<Kursant>(context.Kursanci.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się wczytać listy kursantów: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Zapisuje kursanta do bazy — okno formularza zamyka się tylko przy udanym zapisie.
        public bool ZapiszDoBazy(Kursant? edytowany = null)
        {
            if (string.IsNullOrWhiteSpace(NowyPesel))
            {
                MessageBox.Show("Pole PESEL jest wymagane!", "Brak danych", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!Regex.IsMatch(NowyPesel.Trim(), @"^\d{11}$"))
            {
                MessageBox.Show("PESEL musi składać się dokładnie z 11 cyfr!", "Błędny PESEL", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            // Sprawdź czy PESEL jest prawidłowy — liczymy sumę kontrolną.
            if (!WalidujSumeKontrolnaPesel(NowyPesel.Trim()))
            {
                MessageBox.Show("Podany PESEL jest nieprawidłowy — sprawdź czy wpisałeś go poprawnie.", "Błędny PESEL", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(NowyLogin))
            {
                MessageBox.Show("Pole Login jest wymagane!", "Brak danych", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Przy dodawaniu kursanta hasło jest obowiązkowe — bez niego nie da się zalogować.
            if (edytowany == null && string.IsNullOrWhiteSpace(NowyHaslo))
            {
                MessageBox.Show("Hasło jest wymagane przy dodawaniu nowego kursanta!", "Brak hasła", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            try
            {
                using var context = new OskDbContext();

                string peselTrim = NowyPesel.Trim();
                string loginTrim = NowyLogin.Trim();

                if (edytowany == null)
                {
                    if (context.Kursanci.Any(k => k.Login == loginTrim))
                    {
                        MessageBox.Show($"Login '{loginTrim}' jest już zajęty!", "Zajęty login", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    if (context.Kursanci.Any(k => k.Pesel == peselTrim))
                    {
                        MessageBox.Show($"Kursant z numerem PESEL '{peselTrim}' już istnieje w systemie!", "Zajęty PESEL", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }

                    context.Kursanci.Add(new Kursant
                    {
                        Imie = NowyImie,
                        Nazwisko = NowyNazwisko,
                        Pesel = peselTrim,
                        Login = loginTrim,
                        Haslo = BCrypt.Net.BCrypt.HashPassword(NowyHaslo),
                        Kategoria = WybranaKategoria
                    });
                }
                else
                {
                    if (context.Kursanci.Any(k2 => k2.Login == loginTrim && k2.Id != edytowany.Id))
                    {
                        MessageBox.Show($"Login '{loginTrim}' jest już zajęty!", "Zajęty login", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    if (context.Kursanci.Any(k2 => k2.Pesel == peselTrim && k2.Id != edytowany.Id))
                    {
                        MessageBox.Show($"Kursant z numerem PESEL '{peselTrim}' już istnieje w systemie!", "Zajęty PESEL", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }

                    var k = context.Kursanci.Find(edytowany.Id);
                    if (k == null)
                    {
                        MessageBox.Show("Ten kursant nie istnieje już w bazie danych. Odśwież listę.", "Błąd zapisu", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    k.Imie = NowyImie;
                    k.Nazwisko = NowyNazwisko;
                    k.Pesel = peselTrim;
                    k.Login = loginTrim;
                    k.Kategoria = WybranaKategoria;

                    // Hasło zmienia się tylko wtedy, gdy admin wpisał nowe — puste pole = bez zmian.
                    if (!string.IsNullOrWhiteSpace(NowyHaslo))
                        k.Haslo = BCrypt.Net.BCrypt.HashPassword(NowyHaslo);
                }

                context.SaveChanges();
                ZaladujKursantow();
                WyczyscPola();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się zapisać danych: {ex.Message}", "Błąd zapisu", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public void UsunKursanta(Kursant k)
        {
            var wynik = MessageBox.Show($"Czy na pewno usunąć kursanta {k.Imie} {k.Nazwisko}?", "Potwierdzenie usunięcia", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (wynik != MessageBoxResult.Yes) return;

            try
            {
                using var context = new OskDbContext();

                // Blokada gdy kursant ma jeszcze zaplanowane jazdy w przyszłości.
                bool maZaplanowane = context.RezerwacjeJazd.Any(r => r.KursantId == k.Id && r.Status == "Zaplanowana");
                if (maZaplanowane)
                {
                    MessageBox.Show($"Nie można usunąć kursanta {k.Imie} {k.Nazwisko} — ma zaplanowane jazdy.\nNajpierw odwołaj wszystkie jego rezerwacje.", "Nie można usunąć", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Jeśli kursant ma tylko historyczne (odwołane) rezerwacje — zapytaj czy usunąć historię.
                bool maHistorie = context.RezerwacjeJazd.Any(r => r.KursantId == k.Id);
                if (maHistorie)
                {
                    var pot = MessageBox.Show(
                        $"Kursant {k.Imie} {k.Nazwisko} ma historyczne (odwołane) jazdy.\nCzy usunąć kursanta WRAZ z historią jazd?",
                        "Usuń historię?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (pot != MessageBoxResult.Yes) return;

                    // Najpierw usuwamy jego jazdy, bo inaczej baza zgłosi błąd.
                    var historia = context.RezerwacjeJazd.Where(r => r.KursantId == k.Id).ToList();
                    context.RezerwacjeJazd.RemoveRange(historia);
                }

                var doUsuniecia = context.Kursanci.Find(k.Id);
                if (doUsuniecia != null)
                {
                    context.Kursanci.Remove(doUsuniecia);
                    context.SaveChanges();
                }

                ZaladujKursantow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się usunąć kursanta: {ex.Message}", "Błąd usuwania", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Liczy sumę kontrolną PESEL i sprawdza czy ostatnia cyfra się zgadza.
        private static bool WalidujSumeKontrolnaPesel(string pesel)
        {
            int[] wagi = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
            int suma = 0;
            for (int i = 0; i < 10; i++)
                suma += (pesel[i] - '0') * wagi[i];
            int cyfraKontrolna = (10 - (suma % 10)) % 10;
            return cyfraKontrolna == (pesel[10] - '0');
        }


        private void WyczyscPola() { NowyImie = ""; NowyNazwisko = ""; NowyPesel = ""; NowyLogin = ""; NowyHaslo = ""; WybranaKategoria = "B"; }
    }
}
