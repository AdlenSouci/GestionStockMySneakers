using GestionStockMySneakers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GestionStockMySneakers.Pages
{
    public partial class Commande : Page
    {
        private ObservableCollection<CommandeEntete> commandes = new ObservableCollection<CommandeEntete>();

        public Commande()
        {
            InitializeComponent();
            afficher();
        }

        private async void afficher()
        {
            pbLoading.Visibility = Visibility.Visible;
            dgCommandes.Visibility = Visibility.Collapsed;

            try
            {
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/commandes");

                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                commandes = JsonConvert.DeserializeObject<ObservableCollection<CommandeEntete>>(responseBody)
                            ?? new ObservableCollection<CommandeEntete>();

                dgCommandes.ItemsSource = commandes;
                lblCommandes.Content = $"Commandes ({commandes.Count})";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
            finally
            {
                pbLoading.Visibility = Visibility.Collapsed;
                dgCommandes.Visibility = Visibility.Visible;
            }
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (dgCommandes.SelectedItem != null)
            {
                dgCommandes.SelectedItem = null;
            }
        }
    }
}
