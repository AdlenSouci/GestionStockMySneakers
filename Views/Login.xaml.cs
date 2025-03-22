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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Password;

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
            if (btnShowPassword.IsChecked == true)
            {
                // Afficher le mot de passe
                txtPasswordVisible.Text = txtPassword.Password;
                txtPasswordVisible.Visibility = Visibility.Visible;
                txtPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Masquer le mot de passe
                txtPassword.Password = txtPasswordVisible.Text;
                txtPassword.Visibility = Visibility.Visible;
                txtPasswordVisible.Visibility = Visibility.Collapsed;
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void btnShowPassword_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
