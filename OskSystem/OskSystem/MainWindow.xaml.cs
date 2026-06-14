using System.Windows;
using OskSystem.Views;

namespace OskSystem
{
    public partial class MainWindow : Window
    {
        public string RolaUzytkownika { get; set; }
        // Login zalogowanej osoby — potrzebny do wyświetlenia jej własnych jazd w kalendarzu.
        public string? ZalogowanyLogin { get; set; }

        public MainWindow(string rola, string? login = null)
        {
            InitializeComponent();

            RolaUzytkownika = rola;
            ZalogowanyLogin = login;

            this.Title = $"OSK System — zalogowano jako: {RolaUzytkownika}";
            TxtPowitanie.Text = $"Witaj w panelu: {RolaUzytkownika}";

            UstawMenuDlaRoli();
        }

        private void UstawMenuDlaRoli()
        {
            if (RolaUzytkownika == "Kursant")
            {
                // Kursant widzi tylko swoje rezerwacje — przyciski zarządzania są dla niego ukryte.
                BtnKursanci.Visibility = Visibility.Collapsed;
                BtnInstruktorzy.Visibility = Visibility.Collapsed;
                BtnPojazdy.Visibility = Visibility.Collapsed;
                BtnJazdy.Content = "Moje Rezerwacje";
            }
            else if (RolaUzytkownika == "Instruktor")
            {
                // Instruktor widzi tylko swój grafik — przyciski zarządzania są dla niego ukryte.
                BtnKursanci.Visibility = Visibility.Collapsed;
                BtnInstruktorzy.Visibility = Visibility.Collapsed;
                BtnPojazdy.Visibility = Visibility.Collapsed;
                BtnJazdy.Content = "Mój Grafik";
            }
        }

        private void BtnKursanci_Click(object sender, RoutedEventArgs e) =>
            GlownyObszar.Content = new KursanciView();

        private void BtnInstruktorzy_Click(object sender, RoutedEventArgs e) =>
            GlownyObszar.Content = new InstruktorzyView();

        private void BtnPojazdy_Click(object sender, RoutedEventArgs e) =>
            GlownyObszar.Content = new PojazdyView();

        private void BtnJazdy_Click(object sender, RoutedEventArgs e) =>
            // Przekaż rolę i login, żeby kalendarz pokazał właściwe dane dla zalogowanej osoby.
            GlownyObszar.Content = new RezerwacjeView(RolaUzytkownika, ZalogowanyLogin);

        private void BtnWyloguj_Click(object sender, RoutedEventArgs e)
        {
            new OskSystem.Views.LoginWindow().Show();
            this.Close();
        }
    }
}
