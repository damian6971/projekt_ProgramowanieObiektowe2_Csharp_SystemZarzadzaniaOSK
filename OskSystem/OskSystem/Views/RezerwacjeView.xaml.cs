using System.Windows;
using System.Windows.Controls;
using OskSystem.Models;
using OskSystem.ViewModels;

namespace OskSystem.Views
{
    public partial class RezerwacjeView : UserControl
    {
        private RezerwacjeViewModel VM => (RezerwacjeViewModel)this.DataContext;

        public RezerwacjeView(string rola = "Admin", string? login = null)
        {
            InitializeComponent();
            this.DataContext = new RezerwacjeViewModel(rola, login);
        }

        private void BtnSlot_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is SlotKalendarza slot)
            {
                if (slot.JestMoja)
                {
                    // Kliknięcie własnej jazdy pyta o jej odwołanie.
                    var wynik = MessageBox.Show(
                        $"Czy chcesz odwołać tę jazdę?\n{slot.GodzinaTekst}\n{slot.SzczegolyJazdy}",
                        "Odwołaj jazdę",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (wynik == MessageBoxResult.Yes)
                    {
                        using var context = new OskSystem.Data.OskDbContext();
                        var rez = context.RezerwacjeJazd
                            .FirstOrDefault(r => r.Termin == slot.CzasRozpoczecia && r.Status != "Odwołana");
                        if (rez != null)
                            VM.UsunRezerwacje(rez.Id);
                    }
                }
                else
                {
                    VM.WybranySlot = slot;
                }
            }
        }

        private void BtnZarezerwuj_Click(object sender, RoutedEventArgs e) => VM.ZapiszRezerwacje();
        private void BtnWstecz_Click(object sender, RoutedEventArgs e) => VM.PrzejdzTydzienWstecz();
        private void BtnDalej_Click(object sender, RoutedEventArgs e) => VM.PrzejdzTydzienDalej();
        private void BtnDzisiaj_Click(object sender, RoutedEventArgs e) => VM.WrocDoDzisiaj();
        // Przyciski z "x" resetują wybrany filtr — kursanta, instruktora lub pojazd.
        private void BtnWyczyscKursanta_Click(object sender, RoutedEventArgs e) => VM.WybranyKursant = null;
        private void BtnWyczyscInstruktora_Click(object sender, RoutedEventArgs e) => VM.WybranyInstruktor = null;
        private void BtnWyczyscPojazdu_Click(object sender, RoutedEventArgs e) => VM.WybranyPojazd = null;
    }
}
