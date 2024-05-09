
using System.Windows;
using System;

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

            //const string expectedUsername = "username";
            //const string expectedPassword = "password";

            //string username = txtUsername.Text;
            //string password = passwordBox.Password;

            //if (username == expectedUsername && password == expectedPassword)
            //{
            //    MessageBox.Show("Login successful");
            //    MainWindow mainWindow = new MainWindow();
            //    mainWindow.Show();
            //    this.Close();
            //}
            //else
            //{
            //    MessageBox.Show("Login failed");
            //}
        }
    }
}
