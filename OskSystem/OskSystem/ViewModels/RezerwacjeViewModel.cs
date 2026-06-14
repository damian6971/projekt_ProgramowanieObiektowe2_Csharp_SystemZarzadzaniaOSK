using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using OskSystem.Data;
using OskSystem.Models;

namespace OskSystem.ViewModels
{
    // Jeden blok 3-godzinny w kalendarzu tygodniowym — może być wolny, zajęty lub wybrany.
    public class SlotKalendarza : ViewModelBase
    {
        public DateTime CzasRozpoczecia { get; set; }
        // Etykieta godzinowa wyświetlana na kafelku, np. "07:00 – 10:00".
        public string GodzinaTekst => $"{CzasRozpoczecia:HH:mm} – {CzasRozpoczecia.AddHours(3):HH:mm}";
        public string DzienTekst => CzasRozpoczecia.ToString("dddd, dd.MM.yyyy");

        private bool _jestZajety;
        // Zmiana zajętości odświeża też kolor i napis na kafelku.
        public bool JestZajety
        {
            get => _jestZajety;
            set { _jestZajety = value; OnPropertyChanged(); OnPropertyChanged(nameof(Kolor)); OnPropertyChanged(nameof(KolorTekstu)); OnPropertyChanged(nameof(Opis)); }
        }

        private bool _jestWybrany;
        public bool JestWybrany
        {
            get => _jestWybrany;
            set { _jestWybrany = value; OnPropertyChanged(); OnPropertyChanged(nameof(Kolor)); }
        }

        // Czy ten termin należy do aktualnie zalogowanej osoby.
        public bool JestMoja { get; set; }
        public bool CzyAdmin { get; set; }
        public string ZajetePrzez { get; set; } = "";
        public string SzczegolyJazdy { get; set; } = "";

        // Kolor kafelka: żółty = wybrany, pomarańczowy = moja jazda, szary = zajęty, zielony = wolny.
        public string Kolor
        {
            get
            {
                if (JestWybrany) return "#FCD34D";
                if (JestMoja) return "#F97316";
                if (JestZajety) return "#E2E8F0";
                return "#DCFCE7";
            }
        }

        public string KolorTekstu
        {
            get
            {
                if (JestMoja) return "#7C2D12";
                if (JestZajety) return "#94A3B8";
                return "#166534";
            }
        }

        public string Opis
        {
            get
            {
                if (JestMoja && CzyAdmin) return "";
                if (JestMoja) return "twoja jazda";
                if (JestZajety) return "zajęty";
                return "wolny";
            }
        }

        public bool MoznaKliknac => !JestZajety && !JestMoja;
        public bool MoznaKliknacLubMoja => !JestZajety;
    }

    public class DzienKalendarza
    {
        public string NazwaDnia { get; set; } = "";
        public ObservableCollection<SlotKalendarza> Sloty { get; set; } = new();
    }

    public class RezerwacjeViewModel : ViewModelBase
    {
        // Rola i login osoby, która się zalogowała — decydują co widzi i co może robić.
        private readonly string _rola;
        private readonly string? _login;

        public Visibility WidocznoscKursanta => _rola == "Admin" ? Visibility.Visible : Visibility.Collapsed;
        public Visibility WidocznoscDodawania => _rola == "Instruktor" ? Visibility.Collapsed : Visibility.Visible;
        public Visibility WidocznoscPaneluWyboru => _rola == "Instruktor" ? Visibility.Collapsed : Visibility.Visible;
        public bool CzyMozeZmienicKursanta => _rola == "Admin";
        public string TytulWidoku => _rola == "Admin" ? "Harmonogram Jazd" : "Moje Rezerwacje";

        private ObservableCollection<RezerwacjaJazdy> _listaRezerwacji = new();
        public ObservableCollection<RezerwacjaJazdy> ListaRezerwacji
        {
            get => _listaRezerwacji;
            set { _listaRezerwacji = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Kursant> DostepniKursanci { get; set; } = new();
        public ObservableCollection<Instruktor> DostepniInstruktorzy { get; set; } = new();
        public ObservableCollection<Pojazd> DostepniPojazdy { get; set; } = new();

        private ObservableCollection<Instruktor> _przefiltrowaniInstruktorzy = new();
        // Instruktorzy posiadający uprawnienia dla kategorii wybranego kursanta.
        public ObservableCollection<Instruktor> PrzefiltrowaniInstruktorzy
        {
            get => _przefiltrowaniInstruktorzy;
            set { _przefiltrowaniInstruktorzy = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Pojazd> _przefiltrowanePojazdy = new();
        // Pojazdy pasujące do kategorii wybranego kursanta.
        public ObservableCollection<Pojazd> PrzefiltrowanePojazdy
        {
            get => _przefiltrowanePojazdy;
            set { _przefiltrowanePojazdy = value; OnPropertyChanged(); }
        }

        private Kursant? _wybranyKursant;
        // Wybór kursanta automatycznie zawęża listę dostępnych instruktorów i pojazdów.
        public Kursant? WybranyKursant
        {
            get => _wybranyKursant;
            set { _wybranyKursant = value; OnPropertyChanged(); AktualizujFiltry(); AktualizujKalendar(); }
        }

        private Instruktor? _wybranyInstruktor;
        public Instruktor? WybranyInstruktor
        {
            get => _wybranyInstruktor;
            set { _wybranyInstruktor = value; OnPropertyChanged(); AktualizujKalendar(); }
        }

        private Pojazd? _wybranyPojazd;
        public Pojazd? WybranyPojazd
        {
            get => _wybranyPojazd;
            set { _wybranyPojazd = value; OnPropertyChanged(); AktualizujKalendar(); }
        }

        private DateTime _tydzienOd = PoczatekTygodnia(DateTime.Today);
        // Data początku widocznego tygodnia — zawsze poniedziałek.
        public DateTime TydzienOd
        {
            get => _tydzienOd;
            set { _tydzienOd = value; OnPropertyChanged(); OnPropertyChanged(nameof(TydzienNapis)); AktualizujKalendar(); }
        }
        public string TydzienNapis => $"{TydzienOd:dd.MM.yyyy} – {TydzienOd.AddDays(5):dd.MM.yyyy}";

        private ObservableCollection<DzienKalendarza> _dni = new();
        public ObservableCollection<DzienKalendarza> Dni
        {
            get => _dni;
            set { _dni = value; OnPropertyChanged(); }
        }

        private SlotKalendarza? _wybranySlot;
        // Kliknięcie nowego terminu odznacza poprzedni i zaznacza nowy.
        public SlotKalendarza? WybranySlot
        {
            get => _wybranySlot;
            set
            {
                if (_wybranySlot != null) _wybranySlot.JestWybrany = false;
                _wybranySlot = value;
                if (_wybranySlot != null) _wybranySlot.JestWybrany = true;
                OnPropertyChanged();
                OnPropertyChanged(nameof(WybranyTerminTekst));
            }
        }

        // Opis wybranego terminu pokazywany nad przyciskiem "Zarezerwuj".
        public string WybranyTerminTekst => WybranySlot != null
            ? $"{WybranySlot.CzasRozpoczecia:dddd dd.MM.yyyy}, {WybranySlot.CzasRozpoczecia:HH:mm}–{WybranySlot.CzasRozpoczecia.AddHours(3):HH:mm}"
            : "— kliknij wolny termin w kalendarzu —";

        public RezerwacjeViewModel(string rola = "Admin", string? login = null)
        {
            _rola = rola;
            _login = login;
            ZaladujDane();
        }

        public void ZaladujDane()
        {
            try
            {
                using var context = new OskDbContext();

                var zapytanie = context.RezerwacjeJazd
                    .Include(r => r.Kursant)
                    .Include(r => r.Instruktor)
                    .Include(r => r.Pojazd)
                    .AsQueryable();

                // Kursant i instruktor widzą wyłącznie swoje własne jazdy.
                if (_rola == "Kursant")
                    zapytanie = zapytanie.Where(r => r.Kursant!.Login == _login);
                else if (_rola == "Instruktor")
                    zapytanie = zapytanie.Where(r => r.Instruktor!.Login == _login);

                ListaRezerwacji = new ObservableCollection<RezerwacjaJazdy>(zapytanie.ToList());
                DostepniKursanci = new ObservableCollection<Kursant>(context.Kursanci.ToList());
                DostepniInstruktorzy = new ObservableCollection<Instruktor>(context.Instruktorzy.ToList());
                DostepniPojazdy = new ObservableCollection<Pojazd>(context.Pojazdy.ToList());

                AktualizujFiltry();

                // Po zalogowaniu automatycznie zaznacz zalogowanego kursanta lub instruktora.
                if (_rola == "Kursant" && !string.IsNullOrEmpty(_login))
                    WybranyKursant = DostepniKursanci.FirstOrDefault(k =>
                        k.Login != null && k.Login.Trim().ToLower() == _login.Trim().ToLower());

                if (_rola == "Instruktor" && !string.IsNullOrEmpty(_login))
                    WybranyInstruktor = DostepniInstruktorzy.FirstOrDefault(i =>
                        i.Login != null && i.Login.Trim().ToLower() == _login.Trim().ToLower());

                AktualizujKalendar();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się wczytać danych: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AktualizujFiltry()
        {
            if (WybranyKursant == null)
            {
                // Bez wybranego kursanta pokazuj wszystkich instruktorów i pojazdy.
                PrzefiltrowaniInstruktorzy = new ObservableCollection<Instruktor>(DostepniInstruktorzy);
                PrzefiltrowanePojazdy = new ObservableCollection<Pojazd>(DostepniPojazdy);
                return;
            }

            // Pokaż tylko instruktorów i pojazdy pasujące do kategorii wybranego kursanta.
            string kat = WybranyKursant.Kategoria;
            var inst = DostepniInstruktorzy
                .Where(i => !string.IsNullOrEmpty(i.Uprawnienia) &&
                            i.Uprawnienia.Split(new[] { ", " }, StringSplitOptions.None).Contains(kat))
                .ToList();
            var poj = DostepniPojazdy
                .Where(p => p.Kategoria == kat)
                .ToList();

            PrzefiltrowaniInstruktorzy = new ObservableCollection<Instruktor>(inst);
            PrzefiltrowanePojazdy = new ObservableCollection<Pojazd>(poj);
            WybranyInstruktor = null;
            WybranyPojazd = null;
        }

        public void AktualizujKalendar()
        {
            DateTime koniec = TydzienOd.AddDays(6);

            try
            {
                using var context = new OskDbContext();

                var rezerwacje = context.RezerwacjeJazd
                    .Include(r => r.Instruktor)
                    .Include(r => r.Pojazd)
                    .Include(r => r.Kursant)
                    .Where(r => r.Status != "Odwołana"
                             && r.Termin >= TydzienOd
                             && r.Termin < koniec.AddDays(1))
                    .ToList();

                var dni = new ObservableCollection<DzienKalendarza>();

                // Wyświetlaj 6 dni (poniedziałek–sobota), każdy podzielony na 3 bloki 3-godzinne.
                for (int d = 0; d < 6; d++)
                {
                    DateTime dzien = TydzienOd.AddDays(d);
                    var sloty = new ObservableCollection<SlotKalendarza>();

                    for (int g = 7; g <= 13; g += 3)
                    {
                        DateTime start = dzien.Date.AddHours(g);
                        DateTime stop = start.AddHours(3);

                        // Sprawdź czy w tym bloku jest rezerwacja należąca do aktualnego użytkownika lub wybranego filtra.
                        var moja = rezerwacje.FirstOrDefault(r => r.Termin == start
                            && ((_rola == "Kursant" && r.KursantId == WybranyKursant?.Id)
                             || (_rola == "Instruktor" && r.InstruktorId == WybranyInstruktor?.Id)
                             || (_rola == "Admin" && (
                                    (WybranyKursant != null && r.KursantId == WybranyKursant.Id)
                                 || (WybranyInstruktor != null && r.InstruktorId == WybranyInstruktor.Id)
                                 || (WybranyPojazd != null && r.PojazdId == WybranyPojazd.Id)))));
                        bool jestMoja = moja != null;

                        string szczegolyJazdy = "";
                        if (moja != null)
                        {
                            if (_rola == "Kursant")
                                szczegolyJazdy = $"{moja.Instruktor?.Imie} {moja.Instruktor?.Nazwisko}\n{moja.Pojazd?.NumerRejestracyjny}";
                            else if (_rola == "Instruktor")
                                szczegolyJazdy = $"{moja.Kursant?.Imie} {moja.Kursant?.Nazwisko}\n{moja.Pojazd?.NumerRejestracyjny}";
                            else
                                szczegolyJazdy = $"{moja.Kursant?.Imie} {moja.Kursant?.Nazwisko}\n{moja.Instruktor?.Imie} {moja.Instruktor?.Nazwisko}\n{moja.Pojazd?.NumerRejestracyjny}";
                        }

                        // Sprawdź czy blok jest zajęty przez kogoś innego (instruktor, pojazd lub kursant).
                        bool zajety = false;
                        string zajetePrzez = "";
                        if (!jestMoja && _rola != "Instruktor")
                        {
                            foreach (var r in rezerwacje)
                            {
                                DateTime rs = r.Termin;
                                DateTime rk = r.Termin.AddHours(r.CzasTrwaniaGodziny);
                                if (start >= rk || stop <= rs) continue;

                                bool dotyczy = false;
                                if (WybranyInstruktor != null && r.InstruktorId == WybranyInstruktor.Id)
                                    dotyczy = true;
                                else if (WybranyInstruktor == null && WybranyPojazd != null && r.PojazdId == WybranyPojazd.Id)
                                    dotyczy = true;
                                else if (WybranyInstruktor == null && WybranyPojazd == null && WybranyKursant != null && r.KursantId == WybranyKursant.Id)
                                    dotyczy = true;

                                if (dotyczy)
                                {
                                    zajety = true;
                                    zajetePrzez = $"{r.Instruktor?.Nazwisko} / {r.Pojazd?.NumerRejestracyjny}";
                                    break;
                                }
                            }
                        }

                        sloty.Add(new SlotKalendarza
                        {
                            CzasRozpoczecia = start,
                            JestZajety = zajety,
                            JestMoja = jestMoja,
                            CzyAdmin = _rola == "Admin",
                            SzczegolyJazdy = szczegolyJazdy,
                            ZajetePrzez = zajetePrzez
                        });
                    }

                    dni.Add(new DzienKalendarza
                    {
                        NazwaDnia = dzien.ToString("dddd, dd.MM.yyyy"),
                        Sloty = sloty
                    });
                }

                Dni = dni;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się załadować kalendarza: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            WybranySlot = null;
        }

        public void PrzejdzTydzienWstecz() => TydzienOd = TydzienOd.AddDays(-7);
        public void PrzejdzTydzienDalej() => TydzienOd = TydzienOd.AddDays(7);
        public void WrocDoDzisiaj() => TydzienOd = PoczatekTygodnia(DateTime.Today);

        public void ZapiszRezerwacje()
        {
            if (WybranySlot == null)
            {
                MessageBox.Show("Najpierw kliknij wolny termin w kalendarzu!", "Brak terminu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (WybranyKursant == null || WybranyInstruktor == null || WybranyPojazd == null)
            {
                MessageBox.Show("Musisz wybrać kursanta, instruktora i pojazd!", "Uzupełnij dane", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime start = WybranySlot.CzasRozpoczecia;
            DateTime stop = start.AddHours(3);

            // Blokada rezerwacji terminów, które już minęły.
            if (start < DateTime.Now)
            {
                MessageBox.Show("Nie można zarezerwować jazdy w terminie, który już minął!", "Nieprawidłowy termin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var context = new OskDbContext();

                // Przed zapisem sprawdź czy nikt inny nie jest już zajęty w tym samym czasie.
                var kolizje = context.RezerwacjeJazd
                    .Where(r => r.Status != "Odwołana" && r.Termin < stop && r.Termin.AddHours(r.CzasTrwaniaGodziny) > start)
                    .ToList();

                foreach (var r in kolizje)
                {
                    if (r.InstruktorId == WybranyInstruktor.Id)
                    { MessageBox.Show("Ten instruktor jest już zajęty w tym terminie!", "Konflikt terminu", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                    if (r.PojazdId == WybranyPojazd.Id)
                    { MessageBox.Show("Ten pojazd jest już zajęty w tym terminie!", "Konflikt terminu", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                    if (r.KursantId == WybranyKursant.Id)
                    { MessageBox.Show("Ten kursant ma już zaplanowaną jazdę w tym terminie!", "Konflikt terminu", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                }

                context.RezerwacjeJazd.Add(new RezerwacjaJazdy
                {
                    Termin = start,
                    CzasTrwaniaGodziny = 3,
                    Status = "Zaplanowana",
                    KursantId = WybranyKursant.Id,
                    InstruktorId = WybranyInstruktor.Id,
                    PojazdId = WybranyPojazd.Id
                });
                context.SaveChanges();

                MessageBox.Show($"Jazda zarezerwowana!\n{start:dd.MM.yyyy HH:mm} – {stop:HH:mm}", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                ZaladujDane();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się zapisać rezerwacji: {ex.Message}", "Błąd zapisu", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UsunRezerwacje(int idRezerwacji)
        {
            var wynik = MessageBox.Show("Czy na pewno odwołać tę rezerwację?", "Potwierdzenie odwołania", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (wynik != MessageBoxResult.Yes) return;

            try
            {
                using var context = new OskDbContext();

                var r = context.RezerwacjeJazd
                    .Include(x => x.Kursant)
                    .Include(x => x.Instruktor)
                    .FirstOrDefault(x => x.Id == idRezerwacji);

                if (r == null) return;

                // Kursant i instruktor mogą odwoływać tylko własne jazdy, nie cudze.
                if (_rola == "Kursant" && r.Kursant?.Login != _login)
                {
                    MessageBox.Show("Możesz odwołać tylko własne jazdy!", "Brak dostępu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (_rola == "Instruktor" && r.Instruktor?.Login != _login)
                {
                    MessageBox.Show("Możesz odwołać tylko własne jazdy!", "Brak dostępu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                r.Status = "Odwołana";
                context.SaveChanges();
                ZaladujDane();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się odwołać rezerwacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Wyznacza poniedziałek danego tygodnia na podstawie dowolnej daty.
        public static DateTime PoczatekTygodnia(DateTime data)
        {
            int diff = (7 + (data.DayOfWeek - DayOfWeek.Monday)) % 7;
            return data.AddDays(-diff).Date;
        }
    }
}
