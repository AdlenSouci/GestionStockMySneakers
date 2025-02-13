using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GestionStockMySneakers.Views
{
    public partial class Login : Window
    {
        private LoginService loginService = new LoginService();

        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = passwordBox.Password.Trim();

            if (loginService.logIn(email, password))
            {
                MessageBox.Show("Connexion réussie !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Email ou mot de passe incorrect.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Visibility == Visibility.Visible)
            {
                // Créer un TextBox pour afficher le mot de passe
                TextBox txtPassword = new TextBox
                {
                    Text = passwordBox.Password,
                    Width = passwordBox.Width,
                    Margin = passwordBox.Margin,
                    FontSize = passwordBox.FontSize,
                    FontWeight = passwordBox.FontWeight,
                    Background = passwordBox.Background,
                    Foreground = passwordBox.Foreground
                };
                txtPassword.LostFocus += (s, args) =>
                {
                    passwordBox.Password = txtPassword.Text;
                    passwordBox.Visibility = Visibility.Visible;
                    txtPassword.Visibility = Visibility.Collapsed;
                };
                passwordBox.Visibility = Visibility.Collapsed;
                StackPanel parent = (StackPanel)passwordBox.Parent;
                parent.Children.Insert(parent.Children.IndexOf(passwordBox), txtPassword);
                txtPassword.Focus();
                txtPassword.SelectAll();
                
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

    }
}

