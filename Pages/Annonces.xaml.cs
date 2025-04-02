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
    public partial class Annonces : Page
    {

        private ObservableCollection<Models.Annonce> annonces = new ObservableCollection<Models.Annonce>();

        public Annonces()
        {
            InitializeComponent();
            afficher(); // Display existing annonces
        }

        private async void afficher()
        {
            // Afficher le spinner
            pbLoading.Visibility = Visibility.Visible;
            dgAnnonces.Visibility = Visibility.Collapsed;

            try
            {
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/annonce");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                annonces = JsonConvert.DeserializeObject<ObservableCollection<Models.Annonce>>(responseBody) ?? new ObservableCollection<Models.Annonce>();

                dgAnnonces.ItemsSource = annonces;
                lblAnnonces.Content = $"Annonces ({annonces.Count})"; // Afficher le nombre d'annonces
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
            finally
            {
                // Masquer le spinner et afficher les données
                pbLoading.Visibility = Visibility.Collapsed;
                dgAnnonces.Visibility = Visibility.Visible;
            }
        }

        private void effacer()
        {
            dgAnnonces.SelectedItem = null;
        }

        private async void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            effacer();
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (dgAnnonces.SelectedItem != null)
            {
                dgAnnonces.SelectedItem = null;
            }
        }

        private async void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {

            // Vérifier que tous les champs sont remplis
            if (string.IsNullOrEmpty(txtH1.Text) || string.IsNullOrEmpty(txtH3.Text) ||
                string.IsNullOrEmpty(txtTexte.Text) || string.IsNullOrEmpty(txtImageURL.Text) || string.IsNullOrEmpty(cmbStatut.SelectedValue.ToString()))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.");
                return;
            }

            var annonce = new
            {
                h1 = txtH1.Text,
                h3 = txtH3.Text,
                texte = txtTexte.Text,
                imageURL = txtImageURL.Text,
                statut = Enum.Parse<StatutAnnonce>((string)cmbStatut.SelectedValue),
            };

            try
            {
                HttpResponseMessage response;
                string json = JsonConvert.SerializeObject(annonce);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                if (null == txtId.Content)
                {
                    // Ajout
                    response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/annonce", content);
                    // Récupérer l'annonce ajoutée
                    string a = await response.Content.ReadAsStringAsync();
                    var newAnnonce = JsonConvert.DeserializeObject<Annonce>(a);

                    // Ajouter l'annonce directement au DataGrid
                    if (null != newAnnonce)
                        annonces.Add(newAnnonce);

                    MessageBox.Show("Annonce ajoutée avec succès !");
                }
                else
                {
                    // Mise à jour
                    int annonceId = int.Parse(txtId.Content.ToString());
                    response = await ApiClient.Client.PutAsync(ApiClient.apiUrl + $"/annonce/{annonceId}", content);

                    // Trouver et modifier l'annonce dans la liste existante
                    var updatedAnnonce = annonces.FirstOrDefault(a => a.id == annonceId);
                    if (updatedAnnonce != null)
                    {
                        updatedAnnonce.h1 = txtH1.Text;
                        updatedAnnonce.h3 = txtH3.Text;
                        updatedAnnonce.texte = txtTexte.Text;
                        updatedAnnonce.imageURL = txtImageURL.Text;
                        updatedAnnonce.statut = Enum.Parse<StatutAnnonce>((string)cmbStatut.SelectedValue);

                        //updatedAnnonce.id_couleur = ((Models.Couleur)cmbCouleur.SelectedItem).id;
                    }
                    dgAnnonces.Items.Refresh();
                    MessageBox.Show("Annonce mise à jour avec succès !");
                }
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }

        private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (dgAnnonces.SelectedItem is Annonce annonceSelectionnee)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Voulez-vous vraiment supprimer cette annonce ?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        HttpResponseMessage response = await ApiClient.Client.DeleteAsync(ApiClient.apiUrl + "/annonce/" + annonceSelectionnee.id);
                        response.EnsureSuccessStatusCode();

                        // Supprimer l'annonce de la liste locale
                        annonces.Remove(annonceSelectionnee);
                        //MessageBox.Show("Annonce supprimée avec succès !");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur : " + ex.Message);
                    }
                }
            }
        }
    }
}
