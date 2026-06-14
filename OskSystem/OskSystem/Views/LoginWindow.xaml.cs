using System.Linq;
using System.Windows;
using BCrypt.Net;
using OskSystem.Data;

namespace OskSystem.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnZaloguj_Click(object sender, RoutedEventArgs e)
        {
            string? rola = (ComboRola.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();
            string login = TxtLogin.Text.Trim();
            string haslo = TxtHaslo.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(haslo))
            {
                MessageBox.Show("Wpisz login i hasło!", "Puste pola", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = new OskDbContext();

            // Sprawdzamy login i hasło osobno dla każdej roli.
            if (rola == "Administrator")
            {
                var admin = context.Administratorzy.FirstOrDefault(a => a.Login == login);
                if (admin != null && BCrypt.Net.BCrypt.Verify(haslo, admin.Haslo))
                { OtworzMainWindow("Admin", login); return; }
            }
            else if (rola == "Kursant")
            {
                var kursant = context.Kursanci.FirstOrDefault(k => k.Login == login);
                if (kursant != null && BCrypt.Net.BCrypt.Verify(haslo, kursant.Haslo))
                { OtworzMainWindow("Kursant", login); return; }
            }
            else if (rola == "Instruktor")
            {
                var instruktor = context.Instruktorzy.FirstOrDefault(i => i.Login == login);
                if (instruktor != null && BCrypt.Net.BCrypt.Verify(haslo, instruktor.Haslo))
                { OtworzMainWindow("Instruktor", login); return; }
            }

            MessageBox.Show("Niepoprawny login lub hasło!", "Błąd logowania", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OtworzMainWindow(string rola, string login)
        {
            var glowneOkno = new MainWindow(rola, login);
            glowneOkno.Show();
            this.Close();
        }
    }
}
