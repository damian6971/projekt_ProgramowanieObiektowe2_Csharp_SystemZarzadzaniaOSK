using System.Windows;
using System.Windows.Controls;
using OskSystem.Models;
using OskSystem.ViewModels;

namespace OskSystem.Views
{
    public partial class DodajInstruktoraWindow : Window
    {
        private readonly InstruktorzyViewModel _vm;
        // Jeśli przekazano instruktora, okno służy do edycji; jeśli nie — do dodania nowego.
        private readonly Instruktor? _edytowanyInstruktor;

        public DodajInstruktoraWindow(InstruktorzyViewModel vm, Instruktor? i = null)
        {
            InitializeComponent();
            _vm = vm;
            _edytowanyInstruktor = i;
            this.DataContext = _vm;
            if (i != null) this.Title = "Edytuj Instruktora";
        }

        private void PbHaslo_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _vm.NowyHaslo = ((PasswordBox)sender).Password;
        }

        private void BtnZapisz_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_vm.NowyImie) ||
                string.IsNullOrWhiteSpace(_vm.NowyNazwisko) ||
                string.IsNullOrWhiteSpace(_vm.NowyNumerLegitymacji))
            {
                MessageBox.Show("Uzupełnij wymagane dane (Imię, Nazwisko, Nr Legitymacji)!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Zamykamy okno tylko gdy zapis się udał — przy błędach zostaje otwarte.
            if (_vm.ZapiszDoBazy(_edytowanyInstruktor))
                this.Close();
        }
    }
}
