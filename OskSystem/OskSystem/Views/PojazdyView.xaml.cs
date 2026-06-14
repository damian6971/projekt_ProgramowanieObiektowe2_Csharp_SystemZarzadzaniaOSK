using System.Windows;
using System.Windows.Controls;
using OskSystem.ViewModels;
using OskSystem.Models;

namespace OskSystem.Views
{
    public partial class PojazdyView : UserControl
    {
        private PojazdyViewModel VM => (PojazdyViewModel)this.DataContext;

        public PojazdyView() { InitializeComponent(); this.DataContext = new PojazdyViewModel(); }

        private void BtnOtworzDodawanie_Click(object sender, RoutedEventArgs e)
        {
            var okno = new DodajPojazdWindow(VM);
            okno.Owner = Window.GetWindow(this);
            okno.ShowDialog();
        }

        private void BtnUsun_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is Pojazd p) VM.UsunPojazd(p);
        }

        private void BtnEdytuj_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is Pojazd p)
            {
                VM.NowaMarka = p.Marka; VM.NowyModel = p.ModelPojazdu;
                VM.NowyRejestracja = p.NumerRejestracyjny; VM.WybranaKategoria = p.Kategoria;
                var okno = new DodajPojazdWindow(VM, p);
                okno.Owner = Window.GetWindow(this);
                okno.ShowDialog();
            }
        }
    }
}