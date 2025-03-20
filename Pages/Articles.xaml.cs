using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;

namespace GestionStockMySneakers.Pages
{
    public partial class Articles : Page
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string apiUrl = ConfigurationManager.AppSettings["api_url"] + "/article"; 

        public Articles()
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
                var articles = JsonConvert.DeserializeObject<List<Article>>(responseBody);

                dgArticles.ItemsSource = articles;
                lblArticles.Content = $"Articles ({articles.Count})"; 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }

        private void effacer()
        {
            txtId.Content = "";
            SAI_Marque.Text = "";
            SAI_NomFamille.Text = "";
            SAI_Modele.Text = "";
            SAI_Description.Text = "";
            SAI_Couleur.Text = "";
            SAI_PrixPublic.Text = "";
            SAI_PrixAchat.Text = "";
            SAI_Img.Text = "";
        }

        private void dgArticles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgArticles.SelectedItem is Article articleSelectionne)
            {
                txtId.Content = articleSelectionne.id;
                SAI_Marque.Text = articleSelectionne.nom_marque;
                SAI_NomFamille.Text = articleSelectionne.nom_famille;
                SAI_Modele.Text = articleSelectionne.modele;
                SAI_Description.Text = articleSelectionne.description;
                SAI_Couleur.Text = articleSelectionne.nom_couleur;
                SAI_PrixPublic.Text = articleSelectionne.prix_public.ToString();
                SAI_PrixAchat.Text = articleSelectionne.prix_achat.ToString();
                SAI_Img.Text = articleSelectionne.img;

                // Afficher l'image
                AfficherImage(articleSelectionne.img);
            }
        }

        private void AfficherImage(string imageName)
        {
            try
            {
                var imagePath = @"C:\\CSharp\\GestionStockMySneakers\\img\\" + imageName;

                if (File.Exists(imagePath))
                {
                    var uriSource = new Uri(imagePath);
                    ImageArticle.Source = new BitmapImage(uriSource);
                }
                else
                {
                    MessageBox.Show("L'image n'existe pas sur le chemin spécifié.", "Erreur d'image", MessageBoxButton.OK, MessageBoxImage.Error);
                    ImageArticle.Source = null; // Clear image if not found
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement de l'image : " + ex.Message, "Erreur d'image", MessageBoxButton.OK, MessageBoxImage.Error);
                ImageArticle.Source = null; // Clear image on error
            }
        }

        //private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    // Vérifiez si le clic est en dehors du DataGrid
        //    if (dgArticles.IsFocused == false)
        //    {
        //        effacer(); // Réinitialiser les champs
        //        // Réactiver le bouton Ajouter
        //        btnAjouter.IsEnabled = true;
        //    }
        //}
    }

    public class Article
    {
        public int id { get; set; }
        public string nom_marque { get; set; }
        public string nom_famille { get; set; }
        public string modele { get; set; }
        public string description { get; set; }
        public string nom_couleur { get; set; }
        public decimal prix_public { get; set; }
        public decimal prix_achat { get; set; }
        public string img { get; set; }
    }
}
