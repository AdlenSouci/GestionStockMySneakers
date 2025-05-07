using GestionStockMySneakers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GestionStockMySneakers.Pages
{
    public partial class Avis : Page
    {
        private ObservableCollection<Models.Avis> avis = new ObservableCollection<Models.Avis>();

        public Avis()
        {
            InitializeComponent();
            afficher();
        }

        private async void afficher()
        {
            pbLoading.Visibility = Visibility.Visible;
            dgAvis.Visibility = Visibility.Collapsed;

            try
            {
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/avis");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Désérialiser en List<Models.Avis> d'abord
                var listAvis = JsonConvert.DeserializeObject<List<Models.Avis>>(responseBody);
                avis = new ObservableCollection<Models.Avis>(listAvis ?? new List<Models.Avis>());

                dgAvis.ItemsSource = avis;
                lblAvis.Content = $"Avis ({avis.Count})"; // Afficher le nombre d'avis
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show("Erreur de connexion : " + httpEx.Message);
            }
            catch (JsonException jsonEx)
            {
                MessageBox.Show("Erreur de traitement des données : " + jsonEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur inattendue : " + ex.Message);
            }
            finally
            {
                pbLoading.Visibility = Visibility.Collapsed;
                dgAvis.Visibility = Visibility.Visible;
            }
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (dgAvis.SelectedItem != null)
            {
                dgAvis.SelectedItem = null;
            }
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            // Effacer les champs pour ajouter un nouvel avis
            effacer();
        }

        private async void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            // Vérifier que tous les champs sont remplis
            if (string.IsNullOrEmpty(txtUserId.Text) || string.IsNullOrEmpty(txtArticleId.Text) ||
                string.IsNullOrEmpty(txtContenu.Text) || string.IsNullOrEmpty(txtNote.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.");
                return;
            }

            var avisData = new
            {
                user_id = txtUserId.Text, 
                article_id = txtArticleId.Text,
                contenu = txtContenu.Text,
                note = int.Parse(txtNote.Text),
                created_at = DateTime.Now 
            };

            try
            {
                HttpResponseMessage response;
                string json = JsonConvert.SerializeObject(avisData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Ajout
                response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/avis", content);
                response.EnsureSuccessStatusCode(); // Vérifiez si la réponse est réussie

                var newAvis = JsonConvert.DeserializeObject<Models.Avis>(await response.Content.ReadAsStringAsync());
                if (newAvis != null)
                    avis.Add(newAvis);

                MessageBox.Show("Avis ajouté avec succès !");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }

        private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (dgAvis.SelectedItem is Models.Avis avisSelectionne)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Voulez-vous vraiment supprimer cet avis ?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        HttpResponseMessage response = await ApiClient.Client.DeleteAsync(ApiClient.apiUrl + $"/avis/{avisSelectionne.id}");
                        response.EnsureSuccessStatusCode(); // Vérifiez si la réponse est réussie

                        avis.Remove(avisSelectionne);
                        MessageBox.Show("Avis supprimé avec succès !");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur : " + ex.Message);
                    }
                }
            }
        }

        private void effacer()
        {
            txtUserId.Clear();
            txtArticleId.Clear();
            txtContenu.Clear();
            txtNote.Clear();
            txtId.Clear(); // Utilisez Clear() pour le TextBox txtId
        }

        private void btnEnvoyerReponse_Click(object sender, RoutedEventArgs e)
        {
            // Logique pour envoyer une réponse à l'avis
        }
    }
}
