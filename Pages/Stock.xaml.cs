using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace GestionStockMySneakers.Pages
{
    public partial class Stock : Page
    {
        Data d = new Data();

        private MySqlConnection _connexion;
        private MySqlCommand _command;
        private MySqlDataAdapter _adapter;

        private DataTable _dt;

        public Stock()
        {
            _connexion = d.Connexion();
            InitializeComponent();
            afficher();
        }

        private void afficher()
        {
            try
            {
                _adapter = new MySqlDataAdapter("SELECT * FROM tailles_articles;", _connexion);
                _dt = new DataTable();
                _adapter.Fill(_dt);
                dgTaillesArticles.ItemsSource = _dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void effacer()
        {
            txtId.Content = "";
            SAI_ArticleId.Text = "";
            SAI_Taille.Text = "";
            SAI_Stock.Text = "";
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            string _sql = "INSERT INTO tailles_articles (article_id, taille, stock) VALUES (@ArticleId, @Taille, @Stock)";
            try
            {
                _command = new MySqlCommand(_sql, _connexion);
                _command.Parameters.AddWithValue("@ArticleId", int.Parse(SAI_ArticleId.Text));
                _command.Parameters.AddWithValue("@Taille", int.Parse(SAI_Taille.Text));
                _command.Parameters.AddWithValue("@Stock", int.Parse(SAI_Stock.Text));

                _connexion.Open();
                _command.ExecuteNonQuery();

                afficher();
                effacer();

                MessageBox.Show("Taille enregistrée avec succès", "Nouvelle taille", MessageBoxButton.OK, MessageBoxImage.Information);
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
            try
            {
                // Récupérer l'ancienne valeur depuis la base de données
                string sqlSelect = "SELECT taille, stock FROM tailles_articles WHERE id = @Id";
                _command = new MySqlCommand(sqlSelect, _connexion);
                _command.Parameters.AddWithValue("@Id", txtId.Content);

                _connexion.Open();
                MySqlDataReader reader = _command.ExecuteReader();
                string ancienneTaille = "";
                string ancienStock = "";

                if (reader.Read())
                {
                    ancienneTaille = reader["taille"].ToString();
                    ancienStock = reader["stock"].ToString();
                }
                reader.Close();
                _connexion.Close();

                // Requête de mise à jour
                string _sql = "UPDATE tailles_articles SET article_id = @ArticleId, taille = @Taille, stock = @Stock WHERE id = @Id";
                _command = new MySqlCommand(_sql, _connexion);
                _command.Parameters.AddWithValue("@Id", txtId.Content);
                _command.Parameters.AddWithValue("@ArticleId", int.Parse(SAI_ArticleId.Text));
                _command.Parameters.AddWithValue("@Taille", int.Parse(SAI_Taille.Text));
                _command.Parameters.AddWithValue("@Stock", int.Parse(SAI_Stock.Text));

                _connexion.Open();
                _command.ExecuteNonQuery();

                afficher();

                // Vérifier ce qui a été modifié
                string message = "Aucune modification effectuée";
                if (ancienneTaille != SAI_Taille.Text && ancienStock != SAI_Stock.Text)
                {
                    message = "Taille et stock modifiés avec succès";
                }
                else if (ancienneTaille != SAI_Taille.Text)
                {
                    message = "Taille modifiée avec succès";
                }
                else if (ancienStock != SAI_Stock.Text)
                {
                    message = "Stock modifié avec succès";
                }

                MessageBox.Show(message, "Modification", MessageBoxButton.OK, MessageBoxImage.Information);
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
            string _sql = "DELETE FROM tailles_articles WHERE id = @Id";
            try
            {
                _command = new MySqlCommand(_sql, _connexion);
                _command.Parameters.AddWithValue("@Id", txtId.Content);
                _connexion.Open();
                _command.ExecuteNonQuery();

                afficher();
                effacer();
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

        private void dgTaillesArticles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTaillesArticles.SelectedItem != null)
            {
                DataRowView _drv = (DataRowView)dgTaillesArticles.SelectedItem;

                txtId.Content = _drv.Row["id"].ToString();
                SAI_ArticleId.Text = _drv.Row["article_id"].ToString();
                SAI_Taille.Text = _drv.Row["taille"].ToString();
                SAI_Stock.Text = _drv.Row["stock"].ToString();
            }
        }
    }
}
