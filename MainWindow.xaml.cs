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

      


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            MainWindow mainWindow = new MainWindow();


            mainWindow.Show();

            this.Close();
        }

        private void MenuItem_Click_A(object sender, RoutedEventArgs e)
        {

            MainFrame.NavigationService.Navigate(new Uri("Pages/Articles.xaml", UriKind.Relative));
        }
      
        private void MenuItem_Click_M(object sender, RoutedEventArgs e)
        {

            MainFrame.NavigationService.Navigate(new Uri("Pages/Marques.xaml", UriKind.Relative));
        }

        private void MenuItem_Click_Avis(object sender, RoutedEventArgs e)
        {

            MainFrame.NavigationService.Navigate(new Uri("Pages/Avis.xaml", UriKind.Relative));
        }

        private void MenuItem_Click_Consulter(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new Uri("Pages/consulter.xaml", UriKind.Relative));
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

        }
        private void MenuItem_Click_Gerer(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new Uri("Pages/Stock.xaml", UriKind.Relative));
        }

        private void MenuItem_Click_C(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new Uri("Pages/Commande.xaml", UriKind.Relative));
        }


        private void MenuItem_Click_8(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new Uri("Pages/Commande.xaml", UriKind.Relative));
        }


        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
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
