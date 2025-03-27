using GestionStockMySneakers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace GestionStockMySneakers.Pages
{
    public partial class Avis : Page
    {

        ObservableCollection<GestionStockMySneakers.Models.Avis> avis = new ObservableCollection<GestionStockMySneakers.Models.Avis>();


        public Avis()
        {
            InitializeComponent();
            afficher();
        }
        private async void afficher()
        {
            // Afficher le spinner
            pbLoading.Visibility = Visibility.Visible;
            dgAvis.Visibility = Visibility.Collapsed;

            try
            {
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/avis");

                // Vérifie si la réponse est réussie
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Erreur serveur : {response.StatusCode}");
                }

                string responseBody = await response.Content.ReadAsStringAsync();

                // Désérialisation des données
                avis = JsonConvert.DeserializeObject<ObservableCollection<GestionStockMySneakers.Models.Avis>>(responseBody) ?? new ObservableCollection<GestionStockMySneakers.Models.Avis>();

                dgAvis.ItemsSource = avis;
                lblAvis.Content = $"Avis ({avis.Count})";
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Erreur de requête HTTP : " + ex.Message);
            }
            catch (JsonException ex)
            {
                MessageBox.Show("Erreur de désérialisation JSON : " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur générale : " + ex.Message);
            }
            finally
            {
                // Masquer le spinner et afficher les données
                pbLoading.Visibility = Visibility.Collapsed;
                dgAvis.Visibility = Visibility.Visible;
            }
        }







    }
}
