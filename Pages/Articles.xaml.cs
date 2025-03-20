using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GestionStockMySneakers.Pages
{
    public partial class Articles : Page
    {
        Data d = new Data();

        private MySqlConnection _connexion;
        private MySqlCommand _command;
        private MySqlDataAdapter _adapter;

        private DataTable _dt;

        public Articles()
        {
            _connexion = d.Connexion();
            InitializeComponent();
            afficher();
        }

        private void afficher()
        {
            try
            {
                _adapter = new MySqlDataAdapter("SELECT * FROM articles;", _connexion);
                _dt = new DataTable();
                _adapter.Fill(_dt);
                dgArticles.ItemsSource = _dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            // Vérifiez que tous les champs sont remplis avant d'ajouter
            if (string.IsNullOrWhiteSpace(SAI_Marque.Text))
            {
                MessageBox.Show("Le champ 'Nom de la marque' est vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(SAI_NomFamille.Text))
            {
                MessageBox.Show("Le champ 'Nom de la famille' est vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(SAI_Modele.Text))
            {
                MessageBox.Show("Le champ 'Modèle' est vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(SAI_Description.Text))
            {
                MessageBox.Show("Le champ 'Description' est vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(SAI_Couleur.Text))
            {
                MessageBox.Show("Le champ 'Couleur' est vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(SAI_PrixPublic.Text))
            {
                MessageBox.Show("Le champ 'Prix public' est vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(SAI_PrixAchat.Text))
            {
                MessageBox.Show("Le champ 'Prix d'achat' est vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(SAI_Img.Text))
            {
                MessageBox.Show("Le champ 'Image' est vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Vérifiez que les prix sont des décimaux valides
            decimal prixPublic, prixAchat;
            if (!decimal.TryParse(SAI_PrixPublic.Text, out prixPublic) || !decimal.TryParse(SAI_PrixAchat.Text, out prixAchat))
            {
                MessageBox.Show("Veuillez entrer des valeurs numériques valides pour les prix.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string _sql = "INSERT INTO articles(id_marque, nom_marque, id_famille, nom_famille, modele, description, nom_couleur, prix_public, prix_achat, img) " +
                          "VALUES((SELECT id FROM marques WHERE nom_marque = @NomMarque), @NomMarque, (SELECT id FROM familles WHERE nom_famille = @NomFamille), @NomFamille, @Modele, @Description, @NomCouleur, @PrixPublic, @PrixAchat, @Img)";
            try
            {
                _command = new MySqlCommand(_sql, _connexion);
                _command.Parameters.AddWithValue("@NomMarque", SAI_Marque.Text);
                _command.Parameters.AddWithValue("@NomFamille", SAI_NomFamille.Text);
                _command.Parameters.AddWithValue("@Modele", SAI_Modele.Text);
                _command.Parameters.AddWithValue("@Description", SAI_Description.Text);
                _command.Parameters.AddWithValue("@NomCouleur", SAI_Couleur.Text);
                _command.Parameters.AddWithValue("@PrixPublic", prixPublic);
                _command.Parameters.AddWithValue("@PrixAchat", prixAchat);
                _command.Parameters.AddWithValue("@Img", SAI_Img.Text);

                _connexion.Open();
                _command.ExecuteNonQuery();

                afficher();
                effacer();

                MessageBox.Show("Article enregistré avec succès", "Nouvel article", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _command.Dispose();
                _connexion.Close();
            }
        }

        private void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            string _sql = "UPDATE articles SET id_marque = (SELECT id FROM marques WHERE nom_marque = @NomMarque), nom_marque = @NomMarque, " +
                          "id_famille = (SELECT id FROM familles WHERE nom_famille = @NomFamille), nom_famille = @NomFamille, " +
                          "modele = @Modele, description = @Description, nom_couleur = @NomCouleur, prix_public = @PrixPublic, " +
                          "prix_achat = @PrixAchat, img = @Img WHERE id = @Id";
            try
            {
                _command = new MySqlCommand(_sql, _connexion);
                _command.Parameters.AddWithValue("@Id", txtId.Content);
                _command.Parameters.AddWithValue("@NomMarque", SAI_Marque.Text);
                _command.Parameters.AddWithValue("@NomFamille", SAI_NomFamille.Text);
                _command.Parameters.AddWithValue("@Modele", SAI_Modele.Text);
                _command.Parameters.AddWithValue("@Description", SAI_Description.Text);
                _command.Parameters.AddWithValue("@NomCouleur", SAI_Couleur.Text);

                // Vérifiez que les prix sont des décimaux valides
                decimal prixPublic, prixAchat;
                if (!decimal.TryParse(SAI_PrixPublic.Text, out prixPublic) || !decimal.TryParse(SAI_PrixAchat.Text, out prixAchat))
                {
                    MessageBox.Show("Veuillez entrer des valeurs numériques valides pour les prix.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _command.Parameters.AddWithValue("@PrixPublic", prixPublic);
                _command.Parameters.AddWithValue("@PrixAchat", prixAchat);
                _command.Parameters.AddWithValue("@Img", SAI_Img.Text);

                _connexion.Open();
                _command.ExecuteNonQuery();

                afficher();

                MessageBox.Show("Article modifié avec succès", "Modifier Article", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _command.Dispose();
                _connexion.Close();
            }
        }

        private void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            string _sql = "DELETE FROM articles WHERE id = @Id;";

            try
            {
                _command = new MySqlCommand(_sql, _connexion);
                _command.Parameters.AddWithValue("@Id", txtId.Content);
                _connexion.Open();
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _command.Dispose();
                _connexion.Close();
            }

            afficher();
            effacer();
        }

        private void dgArticles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((DataRowView)dgArticles.SelectedItem != null)
            {
                DataRowView _drv = (DataRowView)dgArticles.SelectedItem;

                txtId.Content = _drv.Row["id"].ToString();
                SAI_Marque.Text = _drv.Row["nom_marque"].ToString();
                SAI_NomFamille.Text = _drv.Row["nom_famille"].ToString();
                SAI_Modele.Text = _drv.Row["modele"].ToString();
                SAI_Description.Text = _drv.Row["description"].ToString();
                SAI_Couleur.Text = _drv.Row["nom_couleur"].ToString();
                SAI_PrixPublic.Text = _drv.Row["prix_public"].ToString();
                SAI_PrixAchat.Text = _drv.Row["prix_achat"].ToString();
                SAI_Img.Text = _drv.Row["img"].ToString();
                string imageUrl = _drv.Row["img"].ToString();

                // Télécharger et afficher l'image
                AfficherImage(imageUrl);
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

        private void SAI_Description_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Vous pouvez ajouter une logique ici si nécessaire
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Vérifiez si le clic est en dehors du DataGrid
            if (dgArticles.IsFocused == false)
            {
                effacer(); // Réinitialiser les champs
                // Réactiver le bouton Ajouter
                btnAjouter.IsEnabled = true;
            }
        }
    }
}
