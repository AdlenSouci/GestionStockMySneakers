using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace GestionStockMySneakers.Pages
{
    public partial class Avis : Page
    {
        private int _selectedAvisId;
        private Data _dsn = new Data(); // Utilisation de _dsn au lieu de data_database

        public Avis()
        {
            InitializeComponent();
            ChargerAvis();
        }

        private void ChargerAvis()
        {
            List<AvisModel> avisList = new List<AvisModel>();

            try
            {
                using (MySqlConnection connection = _dsn.Connexion()) // Utilisation de Connexion() via _dsn
                {
                    connection.Open();
                    string query = "SELECT id, user_id, article_id, contenu, note, created_at, reponse FROM avis";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                avisList.Add(new AvisModel
                                {
                                    Id = reader.GetInt32("id"),
                                    Contenu = reader.GetString("contenu"),
                                    Reponse = reader.IsDBNull(reader.GetOrdinal("reponse")) ? "" : reader.GetString("reponse")
                                });
                            }
                        }
                    }
                }
                dgAvis.ItemsSource = avisList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des avis : {ex.Message}");
            }
        }

        private void dgAvis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgAvis.SelectedItem is AvisModel selectedAvis)
            {
                _selectedAvisId = selectedAvis.Id;
                txtReponse.Text = selectedAvis.Reponse ?? "";
            }
        }

        private void btnEnvoyerReponse_Click(object sender, RoutedEventArgs e)
        {
            if (dgAvis.SelectedItem is AvisModel selectedAvis)
            {
                if (!string.IsNullOrWhiteSpace(txtReponse.Text))
                {
                    try
                    {
                        using (MySqlConnection connection = _dsn.Connexion()) // Utilisation de _dsn
                        {
                            connection.Open();
                            string query = "UPDATE avis SET reponse = @Reponse WHERE id = @AvisId";
                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@Reponse", txtReponse.Text);
                                command.Parameters.AddWithValue("@AvisId", selectedAvis.Id);

                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Réponse enregistrée avec succès !");
                                    selectedAvis.Reponse = txtReponse.Text;
                                    dgAvis.Items.Refresh();
                                }
                                else
                                {
                                    MessageBox.Show("Aucune mise à jour effectuée.");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur : {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Veuillez entrer une réponse avant d'envoyer.");
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un avis.");
            }
        }

        private void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (dgAvis.SelectedItem is AvisModel selectedAvis)
            {
                if (MessageBox.Show("Voulez-vous vraiment supprimer cet avis ?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (MySqlConnection connection = _dsn.Connexion()) // Utilisation de _dsn
                        {
                            connection.Open();
                            string query = "DELETE FROM avis WHERE id = @AvisId";
                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@AvisId", selectedAvis.Id);
                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Avis supprimé avec succès !");
                                    ChargerAvis();
                                }
                                else
                                {
                                    MessageBox.Show("Aucune suppression effectuée.");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur : {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un avis à supprimer.");
            }
        }
    }

    public class AvisModel
    {
        public int Id { get; set; }
        public string Contenu { get; set; }
        public string Reponse { get; set; }
    }
}
