using System.Windows;
using System.Windows.Controls;
using OskSystem.ViewModels;
using OskSystem.Models;

namespace OskSystem.Views
{
    public partial class KursanciView : UserControl
    {
        private KursanciViewModel VM => (KursanciViewModel)this.DataContext;

        public KursanciView()
        {
            InitializeComponent();
            this.DataContext = new KursanciViewModel();
        }

        private void BtnOtworzDodawanie_Click(object sender, RoutedEventArgs e)
        {
            var okno = new DodajKursantaWindow(VM);
            okno.Owner = Window.GetWindow(this);
            okno.ShowDialog();
        }

        private void BtnUsun_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is Kursant k) VM.UsunKursanta(k);
        }

        private void BtnEdytuj_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is Kursant k)
            {
                // Wypełnij formularz danymi wybranego kursanta — pole hasła celowo pozostaje puste.
                VM.NowyImie = k.Imie; VM.NowyNazwisko = k.Nazwisko;
                VM.NowyPesel = k.Pesel; VM.NowyLogin = k.Login;
                VM.NowyHaslo = ""; VM.WybranaKategoria = k.Kategoria;

                var okno = new DodajKursantaWindow(VM, k);
                okno.Owner = Window.GetWindow(this);
                okno.ShowDialog();
            }
        }
    }
}
