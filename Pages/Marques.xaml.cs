using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.Windows.Media.Imaging;

namespace GestionStockMySneakers.Pages
{
    public partial class Marques : Page
    {
        private static readonly HttpClient client = new HttpClient();

        private static readonly string apiUrl = ConfigurationManager.AppSettings["api_url"] + "/marques";


        public Marques()
        {

            InitializeComponent();
            afficher();
        }

        private async void afficher()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var marques = JsonConvert.DeserializeObject<List<Marque>>(responseBody);

                dgMarques.ItemsSource = marques;
                lblMarques.Content = $"Marques ({marques.Count})";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
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
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }


        private void dgMarques_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgMarques.SelectedItem is Marque marqueSelectionnee)
            {
                txtId.Content = marqueSelectionnee.id;
                SAI_Nom.Text = marqueSelectionnee.nom_marque;
            }
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {

            // Vérifiez si le clic est en dehors du DataGrid
            if (dgMarques.IsFocused == false)
            {
                effacer(); // Réinitialiser les champs
                           // Réactiver le bouton Ajouter
                btnAjouter.IsEnabled = true;
            }
        }
    }
    public class Marque
    {
        public int id { get; set; }
        public string nom_marque { get; set; }
    }



}

