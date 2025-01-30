
using System.Windows;
using System;

using System.Windows.Input;

namespace GestionStockMySneakers.Views
{
    /// <summary>
    /// Logique d'interaction pour Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GestionStockMySneakers.Login loginObj = new GestionStockMySneakers.Login();

            if (loginObj.logIn(txtUsername.Text, passwordBox.Password))
            {
                MessageBox.Show("Login successful");
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Login failed");
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
