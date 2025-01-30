using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;
using GestionStockMySneakers;
using System;


namespace GestionStockMySneakers.Pages
{
    public partial class Famille : Page
    {
        // Objets nécessaires pour SQL
        const string _dsn = "Server=my-sneakers-shop.fr;Port=3306;Database=kera6497_my-sneakers;username=kera6497_adlen;password=wrJY?5o.KZrZ;SslMode=none;";
        private MySqlConnection _connexion = new MySqlConnection(_dsn);
        private MySqlCommand _command;
        private MySqlDataAdapter _adapter;

        private DataTable _dt;  // Ne fait pas partie de MySql.Data (objet .NET)
        public Famille()
        {
            InitializeComponent();
            afficher();
        }
        private void afficher()
        {
            try
            {
                _adapter = new MySqlDataAdapter("SELECT * FROM familles;", _connexion); // Instancie un adapter
                _dt = new DataTable();  // Instancie une DataTable
                _adapter.Fill(_dt);  
                dgFamilles.ItemsSource = _dt.DefaultView;  
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void effacer()
        {
            txtId.Content = "";
            SAI_NomFamille.Text = "";
            SAI_IdParent.Text = "";
          
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            string _sql = "INSERT INTO familles(nom_famille, id_parent) VALUES(@NomFamille, @IdParent)";
            try
            {
                _command = new MySqlCommand(_sql, _connexion);
                _command.Parameters.AddWithValue("@NomFamille", SAI_NomFamille.Text);
                _command.Parameters.AddWithValue("@IdParent", string.IsNullOrEmpty(SAI_IdParent.Text) ? (object)DBNull.Value : int.Parse(SAI_IdParent.Text));

                _connexion.Open();
                _command.ExecuteNonQuery();

                afficher();
                effacer();

                MessageBox.Show("Famille enregistrée avec succès", "Nouvelle famille", MessageBoxButton.OK, MessageBoxImage.Information);
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
            string _sql = "UPDATE familles SET nom_famille = @NomFamille, id_parent = @IdParent WHERE id = @Id";
            try
            {
                _command = new MySqlCommand(_sql, _connexion);
                _command.Parameters.AddWithValue("@Id", txtId.Content);
                _command.Parameters.AddWithValue("@NomFamille", SAI_NomFamille.Text);
                _command.Parameters.AddWithValue("@IdParent", string.IsNullOrEmpty(SAI_IdParent.Text) ? (object)DBNull.Value : int.Parse(SAI_IdParent.Text));

                _connexion.Open();
                _command.ExecuteNonQuery();

                afficher();

                MessageBox.Show("Famille modifiée avec succès", "Modifier Famille", MessageBoxButton.OK, MessageBoxImage.Information);
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
            string _sql = "DELETE FROM familles WHERE id = @Id;";
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

        private void dgFamilles_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if ((DataRowView)dgFamilles.SelectedItem != null)
            {
                DataRowView _drv = (DataRowView)dgFamilles.SelectedItem;

                txtId.Content = _drv.Row["id"].ToString();
                SAI_NomFamille.Text = _drv.Row["nom_famille"].ToString();
                SAI_IdParent.Text = _drv.Row["id_parent"].ToString();
            }
        }


    }
}
