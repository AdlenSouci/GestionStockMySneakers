using GestionStockMySneakers.Pages;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace GestionStockMySneakers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Accueil());
        }

        private void MaxBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void MenuItem_Click_Accueil(object sender, RoutedEventArgs e)
        {

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void MenuItem_Click_Articles(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new Uri("Pages/Articles.xaml", UriKind.Relative));
        }

        private void MenuItem_Click_Familles(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new Uri("Pages/Familles.xaml", UriKind.Relative));
        }

        private void MenuItem_Click_Marques(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new Uri("Pages/Marques.xaml", UriKind.Relative));
        }

        private void MenuItem_Click_Couleurs(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new Uri("Pages/Couleurs.xaml", UriKind.Relative));
        }

        private void MenuItem_Click_Avis(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new Uri("Pages/Avis.xaml", UriKind.Relative));
        }

        private void MenuItem_Click_Commandes(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new Uri("Pages/Commande.xaml", UriKind.Relative));
        }

        private void MenuItem_Click_APropos(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new Uri("Pages/APropos.xaml", UriKind.Relative));
        }

        private void MenuItem_Click_Parametres(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new Uri("Pages/Parametres.xaml", UriKind.Relative));
        }

        private void MenuItem_Click_Quitter(object sender, RoutedEventArgs e)
        {
            Close();
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
