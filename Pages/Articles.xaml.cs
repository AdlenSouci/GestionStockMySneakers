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
    public partial class Articles : Page
    {
       
        private List<Marque> marques = new List<Marque>();
        //private List<Models.Article> articles = new List<Article>();
        private ObservableCollection<Models.Article> articles = new ObservableCollection<Article>();

        public Articles()
        {
            InitializeComponent();
            afficher(); // Display existing articles
            LoadMarques(); // Chargement des marques
            LoadFamilles(); // Chargement des familles
            LoadCouleurs(); // Chargement des couleurs
        }

        private async void LoadMarques()
        {
            try { 
                var response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/marque");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                List<Models.Marque> marques = JsonConvert.DeserializeObject<List<Models.Marque>>(responseBody) ?? new List<Models.Marque>();

                cmbMarque.DisplayMemberPath = "nom_marque";
                cmbMarque.SelectedValuePath = "id";
                cmbMarque.ItemsSource = marques;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }

        private async void LoadFamilles()
        {
            try
            {
                var response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/famille");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                List<Models.Famille> familles = JsonConvert.DeserializeObject<List<Models.Famille>>(responseBody) ?? new List<Models.Famille>();

                cmbFamille.DisplayMemberPath = "nom_famille";
                cmbFamille.SelectedValuePath = "id";
                cmbFamille.ItemsSource = familles;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }

        private async void LoadCouleurs()
        {
            try
            {
                var response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/couleur");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                List<Models.Couleur> couleurs = JsonConvert.DeserializeObject<List<Models.Couleur>>(responseBody) ?? new List<Models.Couleur>();

                cmbCouleur.DisplayMemberPath = "nom_couleur";
                cmbCouleur.SelectedValuePath = "id";
                cmbCouleur.ItemsSource = couleurs;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }

        private async void afficher()
        {
            // Afficher le spinner
            pbLoading.Visibility = Visibility.Visible;
            dgArticles.Visibility = Visibility.Collapsed;

            try
            {
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/article");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                articles = JsonConvert.DeserializeObject<ObservableCollection<Models.Article>>(responseBody) ?? new ObservableCollection<Article>();

                dgArticles.ItemsSource = articles;
                lblArticles.Content = $"Articles ({articles.Count})"; // Afficher le nombre d'articles
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
            finally
            {
                // Masquer le spinner et afficher les données
                pbLoading.Visibility = Visibility.Collapsed;
                dgArticles.Visibility = Visibility.Visible;
            }
        }

        private void effacer()
        {
            dgArticles.SelectedItem = null;
        }

        private async void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            effacer();
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (dgArticles.SelectedItem != null)
            {
                dgArticles.SelectedItem = null;
            }
        }

        private async void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {

            // Vérifier que tous les champs sont remplis
            if (string.IsNullOrEmpty(txtModele.Text) || string.IsNullOrEmpty(txtDescription.Text) ||
                cmbFamille.SelectedItem == null || cmbMarque.SelectedItem == null || cmbCouleur.SelectedItem == null)
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.");
                return;
            }

            var article = new
            {
                id_famille = ((Models.Famille)cmbFamille.SelectedItem).id,
                id_marque = ((Models.Marque)cmbMarque.SelectedItem).id,
                id_couleur = ((Models.Couleur)cmbCouleur.SelectedItem).id,
                modele = txtModele.Text,
                description = txtDescription.Text,
                prix_public = decimal.Parse(txtPrixPublic.Text.Replace('.',',')),
                prix_achat = decimal.Parse(txtPrixAchat.Text.Replace('.', ',')),
                img = "default.jpg" // Remplace par un vrai upload d'image si nécessaire
            };

            try
            {
                HttpResponseMessage response;
                string json = JsonConvert.SerializeObject(article);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                if (null==txtId.Content)
                {
                    // Ajout
                    response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/article", content);
                    // 🆕 Récupérer l'article ajouté
                    var newArticle = JsonConvert.DeserializeObject<Article>(await response.Content.ReadAsStringAsync());

                    // 🚀 Ajouter l'article directement au DataGrid
                    if (null!=newArticle)
                        articles.Add(newArticle);

                    MessageBox.Show("Article ajouté avec succès !");
                }
                else
                {
                    // Mise à jour
                    int articleId = int.Parse(txtId.Content.ToString());
                    response = await ApiClient.Client.PutAsync(ApiClient.apiUrl + $"/article/{articleId}", content);

                    // 📢 Trouver et modifier l'article dans la liste existante
                    var updatedArticle = articles.FirstOrDefault(a => a.id == articleId);
                    if (updatedArticle != null)
                    {
                        updatedArticle.modele = txtModele.Text;
                        updatedArticle.description = txtDescription.Text;
                        updatedArticle.prix_public = decimal.Parse(txtPrixPublic.Text.Replace('.', ','));
                        updatedArticle.prix_achat = decimal.Parse(txtPrixAchat.Text.Replace('.', ','));
                        updatedArticle.id_famille = ((Models.Famille)cmbFamille.SelectedItem).id;
                        updatedArticle.id_marque = ((Models.Marque)cmbMarque.SelectedItem).id;
                        updatedArticle.id_couleur = ((Models.Couleur)cmbCouleur.SelectedItem).id;
                    }
                    dgArticles.Items.Refresh();
                    MessageBox.Show("Article mis à jour avec succès !");
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
            if (dgArticles.SelectedItem is Article articleSelectionne)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Voulez-vous vraiment supprimer cet article ?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        HttpResponseMessage response = await ApiClient.Client.DeleteAsync(ApiClient.apiUrl + "/article/" + articleSelectionne.id);
                        response.EnsureSuccessStatusCode();

                        // Supprimer l'article de la liste locale
                        articles.Remove(articleSelectionne);
                        //MessageBox.Show("Article supprimé avec succès !");
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
