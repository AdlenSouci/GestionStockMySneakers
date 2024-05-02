using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;
using GestionStockMySneakers;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;


namespace GestionStockMySneakers.Pages
{
    public partial class CrudA : Page
    {
        // Objets nécessaires pour SQL
        const string _dsn = "server=localhost;port=3306;database=projet_sneakers;username=root;password=;";
        private MySqlConnection _connexion = new MySqlConnection(_dsn);
        private MySqlCommand _command;
        private MySqlDataAdapter _adapter;

        private DataTable _dt;
        public CrudA()
        {
            InitializeComponent();
            afficher();
        }
        private void afficher()
        {
            try
            {
                _adapter = new MySqlDataAdapter("SELECT * FROM articles;", _connexion); // Instancie un adapter
                _dt = new DataTable();  // Instancie une DataTable
                _adapter.Fill(_dt);     // Adapter remplit la DataTable
                dgArticles.ItemsSource = _dt.DefaultView;  // Transfère les données de _dt vers dgArticles
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
            string _sql = "INSERT INTO articles(marque, nom_famille, modele, description, couleur, prix_public, prix_achat, img) VALUES(@Marque, @NomFamille, @Modele, @Description, @Couleur, @PrixPublic, @PrixAchat, @Img)";
            try
            {
                _command = new MySqlCommand(_sql, _connexion);
                _command.Parameters.AddWithValue("@Marque", SAI_Marque.Text);
                _command.Parameters.AddWithValue("@NomFamille", SAI_NomFamille.Text);
                _command.Parameters.AddWithValue("@Modele", SAI_Modele.Text);
                _command.Parameters.AddWithValue("@Description", SAI_Description.Text);
                _command.Parameters.AddWithValue("@Couleur", SAI_Couleur.Text);
                _command.Parameters.AddWithValue("@PrixPublic", decimal.Parse(SAI_PrixPublic.Text));
                _command.Parameters.AddWithValue("@PrixAchat", decimal.Parse(SAI_PrixAchat.Text));
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
            string _sql = "UPDATE articles SET marque = @Marque, nom_famille = @NomFamille, modele = @Modele, description = @Description, couleur = @Couleur, prix_public = @PrixPublic, prix_achat = @PrixAchat, img = @Img WHERE id = @Id";
            try
            {
                _command = new MySqlCommand(_sql, _connexion);
                _command.Parameters.AddWithValue("@Id", txtId.Content);
                _command.Parameters.AddWithValue("@Marque", SAI_Marque.Text);
                _command.Parameters.AddWithValue("@NomFamille", SAI_NomFamille.Text);
                _command.Parameters.AddWithValue("@Modele", SAI_Modele.Text);
                _command.Parameters.AddWithValue("@Description", SAI_Description.Text);
                _command.Parameters.AddWithValue("@Couleur", SAI_Couleur.Text);
                _command.Parameters.AddWithValue("@PrixPublic", decimal.Parse(SAI_PrixPublic.Text));
                _command.Parameters.AddWithValue("@PrixAchat", decimal.Parse(SAI_PrixAchat.Text));
                _command.Parameters.AddWithValue("@Img", SAI_Img.Text);

                _connexion.Open();
                _command.ExecuteNonQuery();

                afficher();

                MessageBox.Show("Article modifier avec succès", "changer Article", MessageBoxButton.OK, MessageBoxImage.Information);
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

            string _sql = "DELETE FROM Article WHERE id = @Id;";

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

        private void dgArticles_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if ((DataRowView)dgArticles.SelectedItem != null)
            {
                DataRowView _drv = (DataRowView)dgArticles.SelectedItem;

                txtId.Content = _drv.Row["id"].ToString();
                SAI_Marque.Text = _drv.Row["marque"].ToString();
                SAI_NomFamille.Text = _drv.Row["nom_famille"].ToString();
                SAI_Modele.Text = _drv.Row["modele"].ToString();
                SAI_Description.Text = _drv.Row["description"].ToString();
                SAI_Couleur.Text = _drv.Row["couleur"].ToString();
                SAI_PrixPublic.Text = _drv.Row["prix_public"].ToString();
                SAI_PrixAchat.Text = _drv.Row["prix_achat"].ToString();
                SAI_Img.Text = _drv.Row["img"].ToString();

               
            }
        }
    }
}
