using GestionStockMySneakers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace GestionStockMySneakers.Pages
{
    public partial class Familles : Page
    {

        private ObservableCollection<Models.Famille> familles = new ObservableCollection<Models.Famille>();


        public Familles()
        {
            InitializeComponent();
            afficher();
        }
        private async void afficher()
        {
            // Afficher le spinner
            pbLoading.Visibility = Visibility.Visible;
            dgFamilles.Visibility = Visibility.Collapsed;

            try
            {
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/famille");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                familles = JsonConvert.DeserializeObject<ObservableCollection<Models.Famille>>(responseBody) ?? new ObservableCollection<Famille>();

                dgFamilles.ItemsSource = familles;
                lblFamilles.Content = $"Familles ({familles.Count})"; // Afficher le nombre de familles
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
            finally
            {
                // Masquer le spinner et afficher les données
                pbLoading.Visibility = Visibility.Collapsed;
                dgFamilles.Visibility = Visibility.Visible;
            }
        }

        private void effacer()
        {
            dgFamilles.SelectedItem = null;
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            effacer();
        }

        private async void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {

            // Vérifier que tous les champs sont remplis
            if (string.IsNullOrEmpty(txtNomFamille.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.");
                return;
            }

            var famille = new
            {
                nom_famille = txtNomFamille.Text,
                id_parent = txtIdParent.Text,
            };

            try
            {
                HttpResponseMessage response;
                string json = JsonConvert.SerializeObject(famille);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                if (null == txtId.Content)
                {
                    // Ajout
                    response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/famille", content);
                    // Récupérer la famille ajoutée
                    var newFamille = JsonConvert.DeserializeObject<Famille>(await response.Content.ReadAsStringAsync());

                    // Ajouter la famille directement au DataGrid
                    if (null != newFamille)
                        familles.Add(newFamille);

                    MessageBox.Show("Famille ajoutée avec succès !");
                }
                else
                {
                    // Mise à jour
                    int familleId = int.Parse(txtId.Content.ToString());
                    response = await ApiClient.Client.PutAsync(ApiClient.apiUrl + $"/famille/{familleId}", content);

                    // Trouver et modifier la famille dans la liste existante
                    var updatedFamille = familles.FirstOrDefault(a => a.id == familleId);
                    if (updatedFamille != null)
                    {
                        updatedFamille.nom_famille = txtNomFamille.Text;
                        if (!string.IsNullOrWhiteSpace(txtIdParent.Text))
                            updatedFamille.id_parent = int.Parse(txtIdParent.Text);
                    }
                    dgFamilles.Items.Refresh();

                    MessageBox.Show("Famille mise à jour avec succès !");
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
                if (dgFamilles.SelectedItem is Famille familleSelectionnee)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Voulez-vous vraiment supprimer cette famille ?",
                        "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            HttpResponseMessage response = await ApiClient.Client.DeleteAsync(ApiClient.apiUrl + $"/famille/{familleSelectionnee.id}");
                            response.EnsureSuccessStatusCode();

                            // Supprimer la famille de la liste locale
                            familles.Remove(familleSelectionnee);
                            //MessageBox.Show("Famille supprimée avec succès !");
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
