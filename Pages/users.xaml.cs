using GestionStockMySneakers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GestionStockMySneakers.Pages
{
    public partial class Users : Page
    {
        private ObservableCollection<GestionStockMySneakers.Models.Users> users = new ObservableCollection<GestionStockMySneakers.Models.Users>();

        public Users()
        {
            InitializeComponent();
            afficher();
        }

        private async void afficher()
        {
            // Afficher le spinner
            pbLoading.Visibility = Visibility.Visible;
            dgUsers.Visibility = Visibility.Collapsed;

            try
            {
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/users");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                users = JsonConvert.DeserializeObject<ObservableCollection<GestionStockMySneakers.Models.Users>>(responseBody) ?? new ObservableCollection<GestionStockMySneakers.Models.Users>();

                dgUsers.ItemsSource = users;
                lblUsers.Content = $"Users ({users.Count})"; 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
            finally
            {
                // Masquer le spinner et afficher les données
                pbLoading.Visibility = Visibility.Collapsed;
                dgUsers.Visibility = Visibility.Visible;
            }
        }

        private void effacer()
        {
            dgUsers.SelectedItem = null;
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            effacer();
        }

        private async void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNom.Text) || string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtPassword.Password))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.");
                return;
            }

            var user = new
            {
                name = txtNom.Text,
                email = txtEmail.Text,
                adresse_livraison = txtAdresse.Text,
                password = txtPassword.Password,
                is_admin = false
            };

            try
            {
                HttpResponseMessage response;
                string json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                if (string.IsNullOrWhiteSpace(txtId.Content?.ToString()))
                {
                    // Ajout
                    response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/users", content);

                    var newUser = JsonConvert.DeserializeObject<GestionStockMySneakers.Models.Users>(await response.Content.ReadAsStringAsync());

                    if (newUser != null)
                        users.Add(newUser);

                    MessageBox.Show("Utilisateur ajouté avec succès !");
                }
                else
                {
                    int userId = int.Parse(txtId.Content.ToString());
                    response = await ApiClient.Client.PutAsync(ApiClient.apiUrl + $"/users/{userId}", content);

                    var updatedUser = users.FirstOrDefault(u => u.user_id == userId);
                    if (updatedUser != null)
                    {
                        updatedUser.name = txtNom.Text;
                        updatedUser.email = txtEmail.Text;
                        updatedUser.adresse_livraison = txtAdresse.Text;
                        // Note: Ne pas mettre à jour le mot de passe ici pour des raisons de sécurité
                    }
                    dgUsers.Items.Refresh();

                    MessageBox.Show("Utilisateur mis à jour avec succès !");
                }
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }

        private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is GestionStockMySneakers.Models.Users userSelectionne)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Voulez-vous vraiment supprimer cet utilisateur ?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        HttpResponseMessage response = await ApiClient.Client.DeleteAsync(ApiClient.apiUrl + $"/users/{userSelectionne.user_id}");
                        response.EnsureSuccessStatusCode();

                        // Supprimer l'utilisateur de la liste locale
                        users.Remove(userSelectionne);
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
