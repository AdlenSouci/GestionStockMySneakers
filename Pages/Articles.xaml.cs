using GestionStockMySneakers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GestionStockMySneakers;
using System.Collections.ObjectModel;

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
                var response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/marques");
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
                var response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/familles");
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
                var response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/couleurs");
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
            txtId.Content = "";
            cmbMarque.SelectedItem = null;
            cmbFamille.SelectedItem = null;
            SAI_Modele.Text = "";
            SAI_Description.Text = "";
            cmbCouleur.SelectedItem = null;
            SAI_PrixPublic.Text = "";
            SAI_PrixAchat.Text = "";
            SAI_Img.Text = "";
        }

        private async void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            await AjouterArticle();
        }

        private async Task AjouterArticle()
        {
            // Assurez-vous que les champs requis sont remplis
            if (cmbMarque.SelectedItem == null ||
                cmbFamille.SelectedItem == null ||
                string.IsNullOrWhiteSpace(SAI_Modele.Text) ||
                //string.IsNullOrWhiteSpace(SAI_Couleur.Text) ||
                !decimal.TryParse(SAI_PrixPublic.Text, out decimal prixPublic) ||
                !decimal.TryParse(SAI_PrixAchat.Text, out decimal prixAchat))
            {
                MessageBox.Show("Veuillez remplir tous les champs requis.");
                return;
            }

            var article = new Article
            {
                //nom_marque = cmbMarque.SelectedItem?.ToString(),
                //nom_famille = cmbFamille.SelectedItem?.ToString(),
                modele = SAI_Modele.Text,
                description = SAI_Description.Text,
                //nom_couleur = SAI_Couleur.Text,
                prix_public = prixPublic,
                prix_achat = prixAchat,
                img = SAI_Img.Text,
            };

            var json = JsonConvert.SerializeObject(article);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await ApiClient.Client.PostAsync(ApiClient.apiUrl, content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Article ajouté avec succès !");
                afficher(); // Rafraîchir la liste des articles
                effacer(); // Effacer les champs après l'ajout
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'ajout de l'article : " + ex.Message);
            }
        }

        private void dgArticles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            if (dgArticles.SelectedItem is Article selectedArticle)
            {
                txtId.Content = selectedArticle.id.ToString();
                cmbMarque.SelectedItem = selectedArticle.nom_marque;
                cmbFamille.SelectedItem = selectedArticle.nom_famille;
                SAI_Modele.Text = selectedArticle.modele;
                SAI_Description.Text = selectedArticle.description;
                SAI_Couleur.Text = selectedArticle.nom_couleur;
                SAI_PrixPublic.Text = selectedArticle.prix_public.ToString();
                SAI_PrixAchat.Text = selectedArticle.prix_achat.ToString();
                SAI_Img.Text = selectedArticle.img;

                if (!string.IsNullOrEmpty(selectedArticle.img))
                {
                    ImageArticle.Source = new BitmapImage(new Uri(selectedArticle.img, UriKind.RelativeOrAbsolute));
                }
            }
            */
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (dgArticles.SelectedItem != null)
            {
                dgArticles.SelectedItem = null;
            }
        }

        private void cmbMarque_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Vous pouvez ajouter une logique ici si nécessaire lorsque la marque est sélectionnée
        }

        private void cmbFamille_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Vous pouvez ajouter une logique ici si nécessaire lorsque la famille est sélectionnée
        }
        private void cmbCouleur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Vous pouvez ajouter une logique ici si nécessaire lorsque la famille est sélectionnée
        }

        private void btnModifier_Click(object sender, RoutedEventArgs e)
        {

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
