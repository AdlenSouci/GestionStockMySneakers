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
    public partial class Couleurs : Page
    {

        private ObservableCollection<Models.Couleur> couleurs = new ObservableCollection<Models.Couleur>();


        public Couleurs()
        {
            InitializeComponent();
            afficher();
        }
        private async void afficher()
        {
            // Afficher le spinner
            pbLoading.Visibility = Visibility.Visible;
            dgCouleurs.Visibility = Visibility.Collapsed;

            try
            {
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/couleur");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                couleurs = JsonConvert.DeserializeObject<ObservableCollection<Models.Couleur>>(responseBody) ?? new ObservableCollection<Couleur>();

                dgCouleurs.ItemsSource = couleurs;
                lblCouleurs.Content = $"Couleurs ({couleurs.Count})"; // Afficher le nombre de couleurs
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
            finally
            {
                // Masquer le spinner et afficher les données
                pbLoading.Visibility = Visibility.Collapsed;
                dgCouleurs.Visibility = Visibility.Visible;
            }
        }

        private void effacer()
        {
            dgCouleurs.SelectedItem = null;
        }

        private void btnNettoyer_Click(object sender, RoutedEventArgs e)
        {
            effacer();
        }


        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            effacer();
        }

        private async void btnModifier_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(txtNomCouleur.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.");
                return;
            }

            var couleur = new
            {
                nom_couleur = txtNomCouleur.Text,
            };

            try
            {
                HttpResponseMessage response;
                string json = JsonConvert.SerializeObject(couleur);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                if (string.IsNullOrWhiteSpace(txtId.Content?.ToString()))
                {
                    // Ajout
                    response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/couleur", content);
                    // Récupérer la couleur ajoutée
                    var newCouleur = JsonConvert.DeserializeObject<Couleur>(await response.Content.ReadAsStringAsync());

                    // Ajouter la couleur directement au DataGrid
                    if (null != newCouleur)
                        couleurs.Add(newCouleur);

                    MessageBox.Show("Couleur ajoutée avec succès !");
                }
                else
                {
                    // Mise à jour
                    int couleurId = int.Parse(txtId.Content.ToString());
                    response = await ApiClient.Client.PutAsync(ApiClient.apiUrl + $"/couleur/{couleurId}", content);

                    // Trouver et modifier la couleur dans la liste existante
                    var updatedCouleur = couleurs.FirstOrDefault(a => a.id == couleurId);
                    if (updatedCouleur != null)
                    {
                        updatedCouleur.nom_couleur = txtNomCouleur.Text;
                    }
                    dgCouleurs.Items.Refresh();

                    MessageBox.Show("Couleur mise à jour avec succès !");
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
            {
                if (dgCouleurs.SelectedItem is Couleur couleurSelectionnee)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Voulez-vous vraiment supprimer cette couleur ?",
                        "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            HttpResponseMessage response = await ApiClient.Client.DeleteAsync(ApiClient.apiUrl + $"/couleur/{couleurSelectionnee.id}");
                            response.EnsureSuccessStatusCode();

                            // Supprimer la couleur de la liste locale
                            couleurs.Remove(couleurSelectionnee);
                            //MessageBox.Show("Couleur supprimé avec succès !");
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
}
