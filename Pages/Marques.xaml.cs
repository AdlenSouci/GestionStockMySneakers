using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace GestionStockMySneakers.Pages
{
    public partial class Marques : Page
    {
       

        private static readonly HttpClient client = new HttpClient();
        private readonly string apiUrl = "http://127.0.0.1:8000/api/marques";

        public Marques()
        {
            InitializeComponent();
            afficher(); // Chargement initial

            // Configuration du Timer (exécute `afficher` toutes les 10 secondes)
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10); // Rafraîchissement toutes les 10 secondes
            timer.Tick += (sender, e) => afficher();
            timer.Start();
        }

        private async void afficher()
        {
            if (!semaphore.Wait(0)) return; // Évite de lancer plusieurs requêtes en même temps
            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                // Gestion des erreurs 429 (trop de requêtes)
                if (response.StatusCode == (System.Net.HttpStatusCode)429)
                {
                    MessageBox.Show("Trop de requêtes ! Attente de 10 secondes...");
                    await Task.Delay(10000); // Attendre 10 secondes avant de réessayer
                    return;
                }

                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var marques = JsonConvert.DeserializeObject<List<Marque>>(responseBody);

                // Mise à jour de l'affichage
                dgMarques.ItemsSource = marques;
                lblMarques.Content = $"Marques ({marques.Count})";
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show("Erreur réseau : " + httpEx.Message);
            }
        }

        private void effacer()
        {
            txtId.Content = "";
            SAI_Nom.Text = "";
        }

        private async void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            var nouvelleMarque = new { nom_marque = SAI_Nom.Text };

            try
            {
                var json = JsonConvert.SerializeObject(nouvelleMarque);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                afficher();
                effacer();
                MessageBox.Show("Marque ajoutée avec succès");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }
        private async void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Content.ToString())) return;

            var marqueModifiee = new { nom_marque = SAI_Nom.Text };
            string url = $"{apiUrl}/{txtId.Content}";

            try
            {
                var json = JsonConvert.SerializeObject(marqueModifiee);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(url, content);
                response.EnsureSuccessStatusCode();

                afficher();
                MessageBox.Show("Marque modifiée avec succès");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }

        private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Content.ToString())) return;

            string url = $"{apiUrl}/{txtId.Content}";

            try
            {
                HttpResponseMessage response = await client.DeleteAsync(url);
                response.EnsureSuccessStatusCode();

                afficher();
                effacer();
                MessageBox.Show("Marque supprimée avec succès");
            }
            catch (Exception ex)
            {
                semaphore.Release(); // Libère l'accès
            }
        }

        private void dgMarques_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Vérifie si une ligne est sélectionnée
            if (dgMarques.SelectedItem is Marque marqueSelectionnee)
            {
                MessageBox.Show($"Marque sélectionnée : {marqueSelectionnee.nom_marque}");
            }
        }
    }

    public class Marque
    {
        public int id { get; set; }
        public string nom_marque { get; set; }
    }
}
