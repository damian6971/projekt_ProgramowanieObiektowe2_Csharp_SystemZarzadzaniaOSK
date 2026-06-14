using System;
using System.Collections.ObjectModel;
using System.Windows;
using OskSystem.Data;
using OskSystem.Models;

namespace OskSystem.ViewModels
{
    public class PojazdyViewModel : ViewModelBase
    {
        private ObservableCollection<Pojazd> _listaPojazdow = new();
        public ObservableCollection<Pojazd> ListaPojazdow
        {
            get => _listaPojazdow;
            set { _listaPojazdow = value; OnPropertyChanged(); }
        }

        // Szerszy zestaw kategorii niż u kursantów — obejmuje też np. tramwaj.
        public ObservableCollection<string> DostepneKategorie { get; } = new ObservableCollection<string>
            { "AM", "A1", "A2", "A", "B1", "B", "B+E", "C1", "C1+E", "C", "C+E", "D1", "D1+E", "D", "D+E", "T", "Tramwaj" };

        private string _nowaMarka = ""; public string NowaMarka { get => _nowaMarka; set { _nowaMarka = value; OnPropertyChanged(); } }
        private string _nowyModel = ""; public string NowyModel { get => _nowyModel; set { _nowyModel = value; OnPropertyChanged(); } }
        private string _nowyRejestracja = ""; public string NowyRejestracja { get => _nowyRejestracja; set { _nowyRejestracja = value; OnPropertyChanged(); } }
        private string _wybranaKategoria = "B"; public string WybranaKategoria { get => _wybranaKategoria; set { _wybranaKategoria = value; OnPropertyChanged(); } }

        public PojazdyViewModel() { ZaladujPojazdy(); }

        public void ZaladujPojazdy()
        {
            try
            {
                using var context = new OskDbContext();
                ListaPojazdow = new ObservableCollection<Pojazd>(context.Pojazdy.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się wczytać listy pojazdów: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Zapisuje pojazd do bazy — okno formularza zamyka się tylko przy udanym zapisie.
        public bool ZapiszDoBazy(Pojazd? edytowany = null)
        {
            if (string.IsNullOrWhiteSpace(NowaMarka) || string.IsNullOrWhiteSpace(NowyModel) || string.IsNullOrWhiteSpace(NowyRejestracja))
            {
                MessageBox.Show("Pola Marka, Model i Numer rejestracyjny są wymagane!", "Brak danych", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            try
            {
                using var context = new OskDbContext();

                if (edytowany == null)
                {
                    context.Pojazdy.Add(new Pojazd { Marka = NowaMarka, ModelPojazdu = NowyModel, NumerRejestracyjny = NowyRejestracja, Kategoria = WybranaKategoria });
                }
                else
                {
                    var p = context.Pojazdy.Find(edytowany.Id);
                    if (p == null)
                    {
                        MessageBox.Show("Ten pojazd nie istnieje już w bazie danych. Odśwież listę.", "Błąd zapisu", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                    p.Marka = NowaMarka; p.ModelPojazdu = NowyModel;
                    p.NumerRejestracyjny = NowyRejestracja; p.Kategoria = WybranaKategoria;
                }

                context.SaveChanges();
                ZaladujPojazdy();
                WyczyscPola();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się zapisać danych pojazdu: {ex.Message}", "Błąd zapisu", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public void UsunPojazd(Pojazd p)
        {
            var wynik = MessageBox.Show($"Czy usunąć pojazd {p.Marka} {p.ModelPojazdu} ({p.NumerRejestracyjny})?", "Potwierdzenie usunięcia", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (wynik != MessageBoxResult.Yes) return;

            try
            {
                using var context = new OskDbContext();

                // Blokada usunięcia pojazdu mającego zaplanowane jazdy — historia odwołanych jest zachowywana.
                bool maZaplanowane = context.RezerwacjeJazd.Any(r => r.PojazdId == p.Id && r.Status == "Zaplanowana");
                if (maZaplanowane)
                {
                    MessageBox.Show($"Nie można usunąć pojazdu {p.Marka} {p.ModelPojazdu} — ma zaplanowane jazdy.\nNajpierw odwołaj wszystkie powiązane rezerwacje.", "Nie można usunąć", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Jeśli pojazd ma tylko historyczne (odwołane) rezerwacje — zapytaj czy usunąć historię.
                bool maHistorie = context.RezerwacjeJazd.Any(r => r.PojazdId == p.Id);
                if (maHistorie)
                {
                    var pot = MessageBox.Show(
                        $"Pojazd {p.Marka} {p.ModelPojazdu} ma historyczne (odwołane) rezerwacje.\nCzy usunąć pojazd WRAZ z historią jazd?",
                        "Usuń historię?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (pot != MessageBoxResult.Yes) return;

                    // Najpierw usuwamy powiązane jazdy, bo inaczej baza zgłosi błąd.
                    var historia = context.RezerwacjeJazd.Where(r => r.PojazdId == p.Id).ToList();
                    context.RezerwacjeJazd.RemoveRange(historia);
                }

                var doUsuniecia = context.Pojazdy.Find(p.Id);
                if (doUsuniecia != null)
                {
                    context.Pojazdy.Remove(doUsuniecia);
                    context.SaveChanges();
                }
                ZaladujPojazdy();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się usunąć pojazdu: {ex.Message}", "Błąd usuwania", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WyczyscPola() { NowaMarka = ""; NowyModel = ""; NowyRejestracja = ""; WybranaKategoria = "B"; }
    }
}
