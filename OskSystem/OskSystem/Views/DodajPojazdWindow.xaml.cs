using System.Windows;
using OskSystem.ViewModels;
using OskSystem.Models;

namespace OskSystem.Views
{
    public partial class DodajPojazdWindow : Window
    {
        private readonly PojazdyViewModel _vm;
        // Jeśli przekazano pojazd, okno służy do edycji; jeśli nie — do dodania nowego.
        private readonly Pojazd? _edytowanyPojazd;

        public DodajPojazdWindow(PojazdyViewModel vm, Pojazd? p = null)
        {
            InitializeComponent();
            _vm = vm;
            _edytowanyPojazd = p;
            this.DataContext = _vm;
            if (p != null) this.Title = "Edytuj Pojazd";
        }

        private void BtnZapisz_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_vm.NowaMarka) ||
                string.IsNullOrWhiteSpace(_vm.NowyModel) ||
                string.IsNullOrWhiteSpace(_vm.NowyRejestracja))
            {
                MessageBox.Show("Uzupełnij wymagane dane (Marka, Model, Nr Rejestracyjny)!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Zamykamy okno tylko gdy zapis się udał — przy błędach zostaje otwarte.
            if (_vm.ZapiszDoBazy(_edytowanyPojazd))
                this.Close();
        }
    }
}
