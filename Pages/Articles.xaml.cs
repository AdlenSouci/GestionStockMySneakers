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
using System.Globalization; // Ajout pour CultureInfo.InvariantCulture si nécessaire

namespace GestionStockMySneakers.Pages
{
    public partial class Articles : Page
    {
        private ObservableCollection<Models.Article> articles = new ObservableCollection<Article>();
        // La liste marques n'est pas utilisée globalement, on peut la déclarer localement si besoin.
        // private List<Marque> marques = new List<Marque>();

        public Articles()
        {
            InitializeComponent();
            afficher(); // Charger et afficher les articles existants
            LoadMarques(); // Charger les marques dans le ComboBox
            LoadFamilles(); // Charger les familles dans le ComboBox
            LoadCouleurs(); // Charger les couleurs dans le ComboBox
        }

        // --- Méthodes de chargement des ComboBox ---
        private async void LoadMarques()
        {
            try
            {
                var response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/marque");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                // Pas besoin de stocker dans 'marques' global si juste pour le ComboBox
                List<Models.Marque> marquesList = JsonConvert.DeserializeObject<List<Models.Marque>>(responseBody) ?? new List<Models.Marque>();

                cmbMarque.DisplayMemberPath = "nom_marque";
                cmbMarque.SelectedValuePath = "id";
                cmbMarque.ItemsSource = marquesList; // Utiliser la liste locale
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur chargement marques: " + ex.Message);
            }
        }

        private async void LoadFamilles()
        {
            try
            {
                var response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/famille");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                List<Models.Famille> famillesList = JsonConvert.DeserializeObject<List<Models.Famille>>(responseBody) ?? new List<Models.Famille>();

                cmbFamille.DisplayMemberPath = "nom_famille";
                cmbFamille.SelectedValuePath = "id";
                cmbFamille.ItemsSource = famillesList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur chargement familles: " + ex.Message);
            }
        }

        private async void LoadCouleurs()
        {
            try
            {
                var response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/couleur");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                List<Models.Couleur> couleursList = JsonConvert.DeserializeObject<List<Models.Couleur>>(responseBody) ?? new List<Models.Couleur>();

                cmbCouleur.DisplayMemberPath = "nom_couleur";
                cmbCouleur.SelectedValuePath = "id";
                cmbCouleur.ItemsSource = couleursList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur chargement couleurs: " + ex.Message);
            }
        }

        // --- Affichage initial et rafraîchissement de la grille ---
        private async void afficher()
        {
            pbLoading.Visibility = Visibility.Visible;
            dgArticles.Visibility = Visibility.Collapsed;

            try
            {
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/article");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Remplacer la collection existante pour refléter l'état actuel de l'API
                var articlesDepuisApi = JsonConvert.DeserializeObject<ObservableCollection<Models.Article>>(responseBody) ?? new ObservableCollection<Article>();
                articles = articlesDepuisApi; // Assigner la nouvelle collection
                dgArticles.ItemsSource = articles; // Mettre à jour la source de la grille
                lblArticles.Content = $"Articles ({articles.Count})";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur affichage articles: " + ex.Message);
                articles.Clear(); // Vider en cas d'erreur pour éviter incohérence
                lblArticles.Content = "Articles (0)";
            }
            finally
            {
                pbLoading.Visibility = Visibility.Collapsed;
                dgArticles.Visibility = Visibility.Visible;
            }
        }

        // --- Fonction pour désélectionner la ligne (et vider le formulaire via Binding OneWay) ---
        private void effacer()
        {
            dgArticles.SelectedItem = null;
            // Les champs texte/combo se videront grâce au Binding OneWay vers SelectedItem
        }

        // --- MODIFIÉ : btnAjouter_Click effectue l'AJOUT ---
        private async void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validation des champs
            if (string.IsNullOrWhiteSpace(txtModele.Text) ||
                string.IsNullOrWhiteSpace(txtDescription.Text) ||
                cmbFamille.SelectedItem == null ||
                cmbMarque.SelectedItem == null ||
                cmbCouleur.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtPrixPublic.Text) ||
                string.IsNullOrWhiteSpace(txtPrixAchat.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires pour ajouter un article.");
                return;
            }

            // Utiliser TryParse pour la conversion des décimaux pour plus de robustesse
            if (!decimal.TryParse(txtPrixPublic.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal prixPublic) ||
                !decimal.TryParse(txtPrixAchat.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal prixAchat))
            {
                MessageBox.Show("Veuillez entrer des valeurs numériques valides pour les prix.");
                return;
            }

            // 2. Création de l'objet article à envoyer
            var articleAAjouter = new
            {
                id_famille = ((Models.Famille)cmbFamille.SelectedItem).id,
                id_marque = ((Models.Marque)cmbMarque.SelectedItem).id,
                id_couleur = ((Models.Couleur)cmbCouleur.SelectedItem).id,
                modele = txtModele.Text,
                description = txtDescription.Text,
                prix_public = prixPublic,
                prix_achat = prixAchat,
                img = string.IsNullOrWhiteSpace(txtImg.Text) ? "default.jpg" : txtImg.Text // Utilise le texte ou "default.jpg"
            };

            // 3. Appel API POST
            try
            {
                string json = JsonConvert.SerializeObject(articleAAjouter);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/article", content);
                response.EnsureSuccessStatusCode(); // Lève une exception si le statut n'est pas 2xx

                // 4. Récupérer et ajouter le nouvel article à la liste locale
                var responseBody = await response.Content.ReadAsStringAsync();
                var newArticle = JsonConvert.DeserializeObject<Article>(responseBody);

                if (newArticle != null)
                {
                    articles.Add(newArticle); // Ajoute à l'ObservableCollection (met à jour la grille)
                    lblArticles.Content = $"Articles ({articles.Count})"; // Met à jour le compteur
                    MessageBox.Show("Article ajouté avec succès !");
                    effacer(); // Efface le formulaire (désélectionne la grille) après succès
                }
                else
                {
                    MessageBox.Show("Erreur : L'API n'a pas retourné l'article ajouté correctement.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Erreur réseau ou API lors de l'ajout : {httpEx.Message} ({(int?)httpEx.StatusCode})");
            }
            catch (JsonException jsonEx)
            {
                MessageBox.Show($"Erreur de format JSON lors de l'ajout : {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur inattendue lors de l'ajout : {ex.Message}");
            }
        }

        // --- Désélectionner si on clique en dehors de la grille ---
        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Vérifie si la source du clic n'est pas un élément de la grille ou un bouton
            if (e.OriginalSource is Grid || e.OriginalSource is Border || e.OriginalSource is Page)
            {
                if (dgArticles.SelectedItem != null)
                {
                    effacer(); // Utilise la fonction existante pour désélectionner et vider
                }
            }
        }

        // --- MODIFIÉ : btnEnregistrer_Click effectue UNIQUEMENT la MODIFICATION ---
        private async void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            // 1. Vérifier si un article est sélectionné (ID non nul)
            if (txtId.Content == null || string.IsNullOrEmpty(txtId.Content.ToString()))
            {
                MessageBox.Show("Veuillez sélectionner un article dans la liste pour le modifier.");
                return;
            }

            int articleId;
            if (!int.TryParse(txtId.Content.ToString(), out articleId))
            {
                MessageBox.Show("ID d'article invalide.");
                return;
            }

            // 2. Validation des champs (identique à l'ajout)
            if (string.IsNullOrWhiteSpace(txtModele.Text) ||
                string.IsNullOrWhiteSpace(txtDescription.Text) ||
                cmbFamille.SelectedItem == null ||
                cmbMarque.SelectedItem == null ||
                cmbCouleur.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtPrixPublic.Text) ||
                string.IsNullOrWhiteSpace(txtPrixAchat.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires pour modifier l'article.");
                return;
            }

            // Utiliser TryParse pour la conversion des décimaux
            if (!decimal.TryParse(txtPrixPublic.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal prixPublic) ||
                !decimal.TryParse(txtPrixAchat.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal prixAchat))
            {
                MessageBox.Show("Veuillez entrer des valeurs numériques valides pour les prix.");
                return;
            }

            // 3. Création de l'objet article mis à jour
            var articleAModifier = new
            {
                // L'ID est dans l'URL de la requête PUT, pas dans le corps typiquement.
                // Si votre API l'exige, ajoutez : id = articleId,
                id_famille = ((Models.Famille)cmbFamille.SelectedItem).id,
                id_marque = ((Models.Marque)cmbMarque.SelectedItem).id,
                id_couleur = ((Models.Couleur)cmbCouleur.SelectedItem).id,
                modele = txtModele.Text,
                description = txtDescription.Text,
                prix_public = prixPublic,
                prix_achat = prixAchat,
                img = string.IsNullOrWhiteSpace(txtImg.Text) ? "default.jpg" : txtImg.Text
            };

            // 4. Appel API PUT
            try
            {
                string json = JsonConvert.SerializeObject(articleAModifier);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await ApiClient.Client.PutAsync(ApiClient.apiUrl + $"/article/{articleId}", content);
                response.EnsureSuccessStatusCode(); // Vérifie le succès

                // 5. Mettre à jour l'objet dans la collection locale pour refléter les changements
                var updatedArticle = articles.FirstOrDefault(a => a.id == articleId);
                if (updatedArticle != null)
                {
                    // Mettre à jour les propriétés de l'objet dans la collection
                    updatedArticle.id_famille = articleAModifier.id_famille;
                    updatedArticle.nom_famille = ((Models.Famille)cmbFamille.SelectedItem).nom_famille; // Mettre à jour le nom aussi
                    updatedArticle.id_marque = articleAModifier.id_marque;
                    updatedArticle.nom_marque = ((Models.Marque)cmbMarque.SelectedItem).nom_marque; // Mettre à jour le nom aussi
                    updatedArticle.modele = articleAModifier.modele;
                    updatedArticle.description = articleAModifier.description;
                    updatedArticle.id_couleur = articleAModifier.id_couleur;
                    updatedArticle.nom_couleur = ((Models.Couleur)cmbCouleur.SelectedItem).nom_couleur; // Mettre à jour le nom aussi
                    updatedArticle.prix_public = articleAModifier.prix_public;
                    updatedArticle.prix_achat = articleAModifier.prix_achat;
                    updatedArticle.img = articleAModifier.img;

                    // Pour ObservableCollection, la grille devrait se mettre à jour.
                    // Si ce n'est pas le cas, dgArticles.Items.Refresh(); peut être nécessaire mais est moins performant.
                }

                MessageBox.Show("Article mis à jour avec succès !");
                // Optionnel: effacer le formulaire après la modification
                // effacer();
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Erreur réseau ou API lors de la modification : {httpEx.Message} ({(int?)httpEx.StatusCode})");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur inattendue lors de la modification : {ex.Message}");
            }
        }

        // --- MODIFIÉ : btnSupprimer_Click utilise effacer() après succès ---
        private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (dgArticles.SelectedItem is Article articleSelectionne)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Voulez-vous vraiment supprimer l'article '{articleSelectionne.modele}' ?",
                    "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        HttpResponseMessage response = await ApiClient.Client.DeleteAsync(ApiClient.apiUrl + "/article/" + articleSelectionne.id);
                        response.EnsureSuccessStatusCode(); // Vérifie succès

                        // Supprimer l'article de la liste locale (met à jour la grille)
                        articles.Remove(articleSelectionne);
                        lblArticles.Content = $"Articles ({articles.Count})"; // Met à jour le compteur
                        MessageBox.Show("Article supprimé avec succès !");
                        effacer(); // Efface le formulaire après la suppression
                    }
                    catch (HttpRequestException httpEx)
                    {
                        MessageBox.Show($"Erreur réseau ou API lors de la suppression : {httpEx.Message} ({(int?)httpEx.StatusCode})");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la suppression : {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un article à supprimer.");
            }
        }
    }
}
