using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace GestionStockMySneakers.Pages
{
    /// <summary>
    /// Logique d'interaction pour Marques.xaml
    /// </summary>
    public partial class Marques : Page
    { // Objets nécessaires pour SQL
        const string _dsn = //"server=109.234.165.229;port=3306;database=kuph3194_sneakers;username=kuph3194_sneakers;password=Adl@nSouci";

        "server=localhost;port=3306;database=projet_sneakers;username=root;password=;";
        private MySqlConnection _connexion = new MySqlConnection(_dsn);
        private MySqlCommand _command;
        private MySqlDataAdapter _adapter;

        private DataTable _dt;  // Ne fait pas partie de MySql.Data (objet .NET)
        public Marques()
        {
            InitializeComponent();
            afficher();
        }

     

        private void afficher()
        {
            try
            {
                _adapter = new MySqlDataAdapter("SELECT * FROM marques;", _connexion);
                _dt = new DataTable();
                _adapter.Fill(_dt);
                dgMarques.ItemsSource = _dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void effacer()
        {
            txtId.Content = "";
            SAI_Nom.Text = "";
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            string _sql = "INSERT INTO marques(nom) VALUES(@Nom)";
            try
            {
                _command = new MySqlCommand(_sql, _connexion);
                _command.Parameters.AddWithValue("@Nom", SAI_Nom.Text);

                _connexion.Open();
                _command.ExecuteNonQuery();

                afficher();
                effacer();

                MessageBox.Show("Marque enregistrée avec succès", "Nouvelle marque", MessageBoxButton.OK, MessageBoxImage.Information);
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
            string _sql = "UPDATE marques SET nom = @Nom WHERE id = @Id";
            try
            {
                _command = new MySqlCommand(_sql, _connexion);
                _command.Parameters.AddWithValue("@Id", txtId.Content);
                _command.Parameters.AddWithValue("@Nom", SAI_Nom.Text);

                _connexion.Open();
                _command.ExecuteNonQuery();

                afficher();

                MessageBox.Show("Marque modifiée avec succès", "Modifier marque", MessageBoxButton.OK, MessageBoxImage.Information);
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
            string _sql = "DELETE FROM marques WHERE id = @Id;";
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

        private void dgMarques_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((DataRowView)dgMarques.SelectedItem != null)
            {
                DataRowView _drv = (DataRowView)dgMarques.SelectedItem;

                txtId.Content = _drv.Row["id"].ToString();
                SAI_Nom.Text = _drv.Row["nom"].ToString();
            }
        }
    }
}

