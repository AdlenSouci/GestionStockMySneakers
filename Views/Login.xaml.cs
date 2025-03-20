using System;
using System.Windows;

namespace GestionStockMySneakers.Views
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click_quitt(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = passwordBox.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Veuillez remplir tous les champs.");
                return;
            }

            // Utilisation de la classe Login.cs pour l'authentification
            string? token = await GestionStockMySneakers.Login.LogInAsync(email, password);

            if (token == null)
            {
                MessageBox.Show("Échec de la connexion. Vérifiez vos identifiants.");
            }
            else
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        private void btnShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Visibility == Visibility.Visible)
            {
                passwordBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                passwordBox.Visibility = Visibility.Visible;
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                this.DragMove();
            }
        }

        
    }
}
