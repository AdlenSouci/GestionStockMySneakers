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
        private ObservableCollection<User> users = new ObservableCollection<User>();

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
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/user");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Désérialisation
                users = JsonConvert.DeserializeObject<ObservableCollection<User>>(responseBody) ?? new ObservableCollection<User>();

                // Vérification des données désérialisées
                if (users != null && users.Count > 0)
                {
                    foreach (var user in users)
                    {
                        Console.WriteLine($"User  ID: {user.user_id}, Name: {user.name}");
                    }
                }
                else
                {
                    Console.WriteLine("Aucun utilisateur trouvé.");
                }

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

        private async void btnAjouter_Click(object sender, RoutedEventArgs e)
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
                is_admin = chkAdmin.IsChecked,
                telephone = txtTelephone.Text,
                ville = txtVille.Text,
                code_postal = txtCodePostal.Text,
            };

            try
            {
                HttpResponseMessage response;
                string json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Ajout


                // Gestion du token
                string token = Settings.Default.UserToken;
                if (string.IsNullOrEmpty(token))
                    throw new Exception("Token non disponible. Veuillez vous reconnecter.");
                ApiClient.Client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/user", content);
                response.EnsureSuccessStatusCode();

                var newUser = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());

                if (newUser != null)
                    users.Add(newUser);

                effacer();

                MessageBox.Show("Utilisateur ajouté avec succès !");


            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }

        }
        private async void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNom.Text) || string.IsNullOrEmpty(txtEmail.Text) )
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.");
                return;
            }

            User user = new()
            {
                user_id=(int)txtId.Content,
                name = txtNom.Text,
                email = txtEmail.Text,
                adresse_livraison = txtAdresse.Text,                
                is_admin = (bool)chkAdmin.IsChecked,
                telephone = txtTelephone.Text,
                ville = txtVille.Text,
                code_postal = txtCodePostal.Text,
            };
            if (!string.IsNullOrWhiteSpace(txtPassword.Password))
                user.password = txtPassword.Password;

            try
            {
                HttpResponseMessage response;
                string json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                int userId = int.Parse(txtId.Content.ToString());

                // Gestion du token
                string token = Settings.Default.UserToken;
                if (string.IsNullOrEmpty(token))
                    throw new Exception("Token non disponible. Veuillez vous reconnecter.");
                ApiClient.Client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                response = await ApiClient.Client.PutAsync(ApiClient.apiUrl + $"/user/{userId}", content);
                response.EnsureSuccessStatusCode();

                var updatedUser = users.FirstOrDefault(u => u.user_id == userId);
                if (updatedUser != null)
                {
                    updatedUser.name = txtNom.Text;
                    updatedUser.email = txtEmail.Text;
                    updatedUser.adresse_livraison = txtAdresse.Text;
                    updatedUser.code_postal = txtCodePostal.Text;
                    updatedUser.ville = txtVille.Text;
                    updatedUser.is_admin = chkAdmin.IsChecked ?? false;
                    // Note: Ne pas mettre à jour le mot de passe ici pour des raisons de sécurité
                    txtPassword.Password = string.Empty;
                }
                dgUsers.Items.Refresh();
                effacer();
                MessageBox.Show("Utilisateur mis à jour avec succès !");
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }
        private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is User userSelectionne)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Voulez-vous vraiment supprimer cet utilisateur ?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        string token = Settings.Default.UserToken;

                        if (string.IsNullOrEmpty(token))
                            throw new Exception("Token non disponible. Veuillez vous reconnecter.");
                        ApiClient.Client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        HttpResponseMessage response = await ApiClient.Client.DeleteAsync(ApiClient.apiUrl + $"/user/{userSelectionne.user_id}");
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
        private void btnNettoyer_Click(object sender, RoutedEventArgs e)
        {
            effacer();
        }

    }
}
