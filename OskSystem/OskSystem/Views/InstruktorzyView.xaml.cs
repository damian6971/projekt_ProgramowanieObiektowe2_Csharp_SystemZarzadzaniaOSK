using System.Windows;
using System.Windows.Controls;
using OskSystem.ViewModels;
using OskSystem.Models;

namespace OskSystem.Views
{
    public partial class InstruktorzyView : UserControl
    {
        private InstruktorzyViewModel VM => (InstruktorzyViewModel)this.DataContext;

        public InstruktorzyView()
        {
            InitializeComponent();
            this.DataContext = new InstruktorzyViewModel();
        }

        private void BtnOtworzDodawanie_Click(object sender, RoutedEventArgs e)
        {
            var okno = new DodajInstruktoraWindow(VM);
            okno.Owner = Window.GetWindow(this);
            okno.ShowDialog();
        }

        private void BtnUsun_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is Instruktor i)
                VM.UsunInstruktora(i);
        }

        private void BtnEdytuj_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is Instruktor i)
            {
                // Wypełnij formularz danymi wybranego instruktora — pole hasła celowo pozostaje puste.
                VM.NowyImie = i.Imie; VM.NowyNazwisko = i.Nazwisko;
                VM.NowyNumerLegitymacji = i.NumerLegitymacji; VM.NowyTelefon = i.Telefon;
                VM.NowyLogin = i.Login; VM.NowyHaslo = "";

                // Zaznacz kategorie, które instruktor już posiada.
                foreach (var kat in VM.DostepneUprawnienia)
                    kat.IsSelected = !string.IsNullOrEmpty(i.Uprawnienia) && i.Uprawnienia.Contains(kat.Nazwa);

                var okno = new DodajInstruktoraWindow(VM, i);
                okno.Owner = Window.GetWindow(this);
                okno.ShowDialog();
            }
        }
    }
}
