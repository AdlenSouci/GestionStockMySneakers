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
using System.Globalization;
using System.Windows.Media.Imaging;
using System.IO;
using System.Net;

namespace GestionStockMySneakers.Pages
{
    public partial class Articles : Page
    {
        private ObservableCollection<Models.Article> articles = new ObservableCollection<Article>();

        public Articles()
        {
            InitializeComponent();
            this.DataContext = this;
            dgArticles.ItemsSource = articles;

            afficher();
            LoadMarques();
            LoadFamilles();
            LoadCouleurs();
        }

        private async void LoadMarques()
        {
            try
            {
                var response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/marque");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                List<Models.Marque> marquesList = JsonConvert.DeserializeObject<List<Models.Marque>>(responseBody) ?? new List<Models.Marque>();
                cmbMarque.ItemsSource = marquesList;
            }
            catch (Exception ex) { MessageBox.Show("Erreur chargement marques: " + ex.Message); }
        }

        private async void LoadFamilles()
        {
            try
            {
                var response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/famille");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                List<Models.Famille> famillesList = JsonConvert.DeserializeObject<List<Models.Famille>>(responseBody) ?? new List<Models.Famille>();
                cmbFamille.ItemsSource = famillesList;
            }
            catch (Exception ex) { MessageBox.Show("Erreur chargement familles: " + ex.Message); }
        }

        private async void LoadCouleurs()
        {
            try
            {
                var response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/couleur");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                List<Models.Couleur> couleursList = JsonConvert.DeserializeObject<List<Models.Couleur>>(responseBody) ?? new List<Models.Couleur>();
                cmbCouleur.ItemsSource = couleursList;
            }
            catch (Exception ex) { MessageBox.Show("Erreur chargement couleurs: " + ex.Message); }
        }

        private async void afficher()
        {
            pbLoading.Visibility = Visibility.Visible;
            dgArticles.Visibility = Visibility.Collapsed;
            try
            {
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/article");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var articlesDepuisApi = JsonConvert.DeserializeObject<ObservableCollection<Models.Article>>(responseBody) ?? new ObservableCollection<Article>();

                articles.Clear();
                foreach (var article in articlesDepuisApi)
                {
                    if (article.tailles == null) article.tailles = new ObservableCollection<TaillesArticle>();
                    else if (!(article.tailles is ObservableCollection<TaillesArticle>)) article.tailles = new ObservableCollection<TaillesArticle>(article.tailles);
                    articles.Add(article);
                }
                lblArticles.Content = $"Articles ({articles.Count})";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur affichage articles: " + ex.Message);
                articles.Clear();
                lblArticles.Content = "Articles (0)";
            }
            finally
            {
                pbLoading.Visibility = Visibility.Collapsed;
                dgArticles.Visibility = Visibility.Visible;
            }
        }

        private void effacer()
        {
            dgArticles.SelectedItem = null;
            txtNewTaille.Clear();
            txtNewStock.Clear();
        }

        private async void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtModele.Text) || cmbFamille.SelectedItem == null || cmbMarque.SelectedItem == null)
            {
                MessageBox.Show("Remplir modèle, marque, famille.");
                return;
            }
            if (!decimal.TryParse(txtPrixPublic.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal prixPublic) ||
                !decimal.TryParse(txtPrixAchat.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal prixAchat))
            {
                MessageBox.Show("Prix invalides.");
                return;
            }

            var articleAAjouter = new
            {
                id_famille = ((Models.Famille)cmbFamille.SelectedItem).id,
                id_marque = ((Models.Marque)cmbMarque.SelectedItem).id,
                id_couleur = (cmbCouleur.SelectedItem as Models.Couleur)?.id,
                modele = txtModele.Text,
                description = txtDescription.Text,
                prix_public = prixPublic,
                prix_achat = prixAchat,
                img = string.IsNullOrWhiteSpace(txtImg.Text) ? "default.jpg" : txtImg.Text,
                tailles = new List<TaillesArticle>()
            };

            try
            {
                string json = JsonConvert.SerializeObject(articleAAjouter);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/article", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var newArticle = JsonConvert.DeserializeObject<Article>(responseBody);
                    if (newArticle != null)
                    {
                        if (newArticle.tailles == null) newArticle.tailles = new ObservableCollection<TaillesArticle>();
                        else if (!(newArticle.tailles is ObservableCollection<TaillesArticle>)) newArticle.tailles = new ObservableCollection<TaillesArticle>(newArticle.tailles);
                        articles.Add(newArticle);
                        lblArticles.Content = $"Articles ({articles.Count})";
                        MessageBox.Show("Article ajouté. Sélectionnez-le pour gérer stock.");
                        effacer();
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erreur : {errorMessage}");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erreur ajout : {response.StatusCode} - {errorMessage}");
                }
            }
            catch (Exception ex) { MessageBox.Show($"Erreur ajout : {ex.Message}"); }
        }

        private async void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (dgArticles.SelectedItem == null || txtId.Content == null || string.IsNullOrEmpty(txtId.Content.ToString()))
            {
                MessageBox.Show("Sélectionner article à modifier.");
                return;
            }
            int articleId = int.Parse(txtId.Content.ToString());

            if (string.IsNullOrWhiteSpace(txtModele.Text) || cmbFamille.SelectedItem == null || cmbMarque.SelectedItem == null)
            {
                MessageBox.Show("Remplir champs obligatoires.");
                return;
            }
            if (!decimal.TryParse(txtPrixPublic.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal prixPublic) ||
               !decimal.TryParse(txtPrixAchat.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal prixAchat))
            {
                MessageBox.Show("Prix invalides.");
                return;
            }

            var articleAModifier = dgArticles.SelectedItem as Models.Article;
            if (articleAModifier == null) return;

            var taillesASauvegarder = new List<TaillesArticle>(articleAModifier.tailles ?? new ObservableCollection<TaillesArticle>());
            foreach (var tailleEntry in taillesASauvegarder)
            {
                if (tailleEntry.taille <= 0 || tailleEntry.stock < 0)
                {
                    MessageBox.Show($"Stock invalide pour taille {tailleEntry.taille}: Taille > 0, Stock >= 0 requis.");
                    return;
                }
            }

            var articleDataToSend = new
            {
                id_famille = ((Models.Famille)cmbFamille.SelectedItem).id,
                id_marque = ((Models.Marque)cmbMarque.SelectedItem).id,
                id_couleur = (cmbCouleur.SelectedItem as Models.Couleur)?.id,
                modele = txtModele.Text,
                description = txtDescription.Text,
                prix_public = prixPublic,
                prix_achat = prixAchat,
                img = string.IsNullOrWhiteSpace(txtImg.Text) ? "default.jpg" : txtImg.Text,
                tailles = taillesASauvegarder
            };

            try
            {
                string json = JsonConvert.SerializeObject(articleDataToSend);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await ApiClient.Client.PutAsync(ApiClient.apiUrl + $"/article/{articleId}", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var updatedArticleFromApi = JsonConvert.DeserializeObject<Article>(responseBody);

                    var localArticle = articles.FirstOrDefault(a => a.id == articleId);
                    if (localArticle != null && updatedArticleFromApi != null)
                    {
                        localArticle.id_famille = updatedArticleFromApi.id_famille;
                        localArticle.nom_famille = updatedArticleFromApi.nom_famille;
                        localArticle.id_marque = updatedArticleFromApi.id_marque;
                        localArticle.nom_marque = updatedArticleFromApi.nom_marque;
                        localArticle.modele = updatedArticleFromApi.modele;
                        localArticle.description = updatedArticleFromApi.description;
                        localArticle.id_couleur = updatedArticleFromApi.id_couleur;
                        localArticle.nom_couleur = updatedArticleFromApi.nom_couleur;
                        localArticle.prix_public = updatedArticleFromApi.prix_public;
                        localArticle.prix_achat = updatedArticleFromApi.prix_achat;
                        localArticle.img = updatedArticleFromApi.img;

                        localArticle.tailles.Clear();
                        if (updatedArticleFromApi.tailles != null)
                        {
                            foreach (var taille in updatedArticleFromApi.tailles)
                            {
                                localArticle.tailles.Add(taille);
                            }
                        }
                        MessageBox.Show("Article et stock mis à jour !");
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erreur : {errorMessage}");
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erreur validation : {errorMessage}");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erreur modification : {response.StatusCode} - {errorMessage}");
                }
            }
            catch (Exception ex) { MessageBox.Show($"Erreur modification : {ex.Message}"); }
        }

        private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (dgArticles.SelectedItem is Article articleSelectionne)
            {
                MessageBoxResult result = MessageBox.Show($"Supprimer '{articleSelectionne.modele}' ?", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        HttpResponseMessage response = await ApiClient.Client.DeleteAsync(ApiClient.apiUrl + "/article/" + articleSelectionne.id);
                        if (response.IsSuccessStatusCode)
                        {
                            articles.Remove(articleSelectionne);
                            lblArticles.Content = $"Articles ({articles.Count})";
                            MessageBox.Show("Article supprimé !");
                            effacer();
                        }
                        else if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            MessageBox.Show("Article non trouvé.");
                            afficher();
                        }
                        else
                        {
                            var errorMessage = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"Erreur suppression : {response.StatusCode} - {errorMessage}");
                        }
                    }
                    catch (Exception ex) { MessageBox.Show($"Erreur suppression : {ex.Message}"); }
                }
            }
            else { MessageBox.Show("Sélectionner article à supprimer."); }
        }

        private void btnAjouterTaille_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgArticles.SelectedItem is Models.Article selectedArticle))
            {
                MessageBox.Show("Sélectionnez article pour ajouter stock.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNewTaille.Text) || string.IsNullOrWhiteSpace(txtNewStock.Text))
            {
                MessageBox.Show("Entrez taille et stock.");
                return;
            }
            if (!int.TryParse(txtNewTaille.Text, out int newTaille) || newTaille <= 0)
            {
                MessageBox.Show("Taille invalide (> 0).");
                txtNewTaille.Focus(); return;
            }
            if (!int.TryParse(txtNewStock.Text, out int newStock) || newStock < 0)
            {
                MessageBox.Show("Stock invalide (>= 0).");
                txtNewStock.Focus(); return;
            }
            if (selectedArticle.tailles != null && selectedArticle.tailles.Any(t => t.taille == newTaille))
            {
                MessageBox.Show($"Taille {newTaille} existe déjà. Modifiez dans tableau.");
                return;
            }

            if (selectedArticle.tailles == null) selectedArticle.tailles = new ObservableCollection<TaillesArticle>();
            selectedArticle.tailles.Add(new TaillesArticle { taille = newTaille, stock = newStock, id = 0, article_id = selectedArticle.id });
            txtNewTaille.Clear();
            txtNewStock.Clear();
            MessageBox.Show("Taille ajoutée localement. Cliquez 'Modifier' pour sauvegarder.");
        }

        private void btnSupprimerTaille_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is TaillesArticle tailleToDelete)
            {
                if (!(dgArticles.SelectedItem is Models.Article selectedArticle) || selectedArticle.tailles == null) return;

                MessageBoxResult result = MessageBox.Show($"Supprimer stock taille {tailleToDelete.taille} ?", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    if (selectedArticle.tailles.Remove(tailleToDelete))
                    {
                        MessageBox.Show("Stock supprimé localement. Cliquez 'Modifier' pour sauvegarder.");
                    }
                }
            }
        }

        private void btnNettoyer_Click(object sender, RoutedEventArgs e)
        {
            effacer();
            MessageBox.Show("Champs nettoyés.");
        }

        private void dgArticles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgArticles.SelectedItem is Models.Article selectedArticle)
            {
                AfficherImage(selectedArticle.img);
                if (selectedArticle.tailles == null) selectedArticle.tailles = new ObservableCollection<TaillesArticle>();
                else if (!(selectedArticle.tailles is ObservableCollection<TaillesArticle>)) selectedArticle.tailles = new ObservableCollection<TaillesArticle>(selectedArticle.tailles);

                if (StockManagementBorder != null) StockManagementBorder.Visibility = Visibility.Visible;
            }
            else
            {
                ImageArticle.Source = null;
                if (StockManagementBorder != null) StockManagementBorder.Visibility = Visibility.Collapsed;
            }
            txtNewTaille.Clear();
            txtNewStock.Clear();
        }

        private void AfficherImage(string imageName)
        {
            try
            {
                var imagePath = Path.Combine(@"C:\CSharp\GestionStockMySneakers - Copie (2)\img\", imageName ?? "");
                if (!string.IsNullOrEmpty(imageName) && File.Exists(imagePath))
                {
                    using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        ImageArticle.Source = bitmap;
                    }
                }
                else { ImageArticle.Source = null; }
            }
            catch { ImageArticle.Source = null; }
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var clickedElement = e.OriginalSource as FrameworkElement;
            if (clickedElement != null)
            {
                bool isClickInsideInteractiveArea = false;
                DependencyObject current = clickedElement;
                while (current != null)
                {
                    if (current is Button || current is TextBox || current is ComboBox || current is DataGrid || current is Border || current is ScrollViewer || current is System.Windows.Controls.Primitives.ScrollBar)
                    {
                        isClickInsideInteractiveArea = true;
                        break;
                    }
                    current = System.Windows.Media.VisualTreeHelper.GetParent(current);
                }
                if (!isClickInsideInteractiveArea && dgArticles.SelectedItem != null)
                {
                    effacer();
                }
            }
        }
    }
}
