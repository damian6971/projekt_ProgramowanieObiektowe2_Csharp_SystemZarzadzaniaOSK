using System.Windows;
using System.Windows.Controls;
using OskSystem.Models;
using OskSystem.ViewModels;

namespace OskSystem.Views
{
    public partial class DodajKursantaWindow : Window
    {
        private readonly KursanciViewModel _vm;
        // Jeśli przekazano kursanta, okno służy do edycji; jeśli nie — do dodania nowego.
        private readonly Kursant? _edytowanyKursant;

        public DodajKursantaWindow(KursanciViewModel vm, Kursant? k = null)
        {
            InitializeComponent();
            _vm = vm;
            _edytowanyKursant = k;
            this.DataContext = _vm;
            if (k != null) this.Title = "Edytuj Kursanta";
        }

        private void PbHaslo_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _vm.NowyHaslo = ((PasswordBox)sender).Password;
        }

        private void BtnZapisz_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_vm.NowyImie) || string.IsNullOrWhiteSpace(_vm.NowyNazwisko))
            {
                MessageBox.Show("Pola Imię i Nazwisko są wymagane!", "Brak danych", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Zamykamy okno tylko gdy zapis się udał — przy błędach zostaje otwarte.
            if (_vm.ZapiszDoBazy(_edytowanyKursant))
                this.Close();
        }
    }
}
