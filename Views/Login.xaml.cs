using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows;

namespace GestionStockMySneakers.Views
{
    public partial class Login : Window
    {
        private static readonly HttpClient client = new HttpClient();
        private const string apiUrl = "http://127.0.0.1:8000/api/login"; // Assurez-vous que c'est bien cette URL

        public Login()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text; // Récupère l'email entré dans le champ
            string password = passwordBox.Password; // Récupère le mot de passe

            // Vérifie que les champs ne sont pas vides
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Veuillez remplir tous les champs.");
                return;
            }

            // Appel à la méthode LogInAsync pour tenter la connexion
            string? token = await LogInAsync(email, password);

            // Si le token est null, affichage d'un message d'erreur
            if (token == null)
            {
                MessageBox.Show("Échec de la connexion. Vérifiez vos identifiants.");
            }
            else
            {
                // Connexion réussie, ouvrir la fenêtre principale
                MainWindow mainWindow = new MainWindow(); // Remplacez par le nom de votre fenêtre principale
                mainWindow.Show(); // Ouvre la nouvelle fenêtre
                this.Close(); // Ferme la fenêtre de connexion
            }
        }

        // Méthode de connexion à l'API Laravel
        public static async Task<string?> LogInAsync(string email, string password)
        {
            try
            {
                var loginData = new
                {
                    email = email,
                    password = password
                };

                var jsonData = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Envoie la requête POST à l'API Laravel
                var response = await client.PostAsync(apiUrl, content); // Assurez-vous que apiUrl est correct

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);
                    return responseObject?.token ?? null;
                }
                else
                {
                    MessageBox.Show("Échec de la connexion. Vérifiez vos identifiants.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite lors de la tentative de connexion : " + ex.Message);
                return null;
            }
        }

        // Méthode pour afficher/masquer le mot de passe
        private void btnShowPassword_Click(object sender, RoutedEventArgs e)
        {
            // Logique pour afficher ou masquer le mot de passe
            if (passwordBox.Visibility == Visibility.Visible)
            {
                passwordBox.Visibility = Visibility.Collapsed;
                // Vous pouvez ajouter un TextBox temporaire pour afficher le mot de passe ici si nécessaire
            }
            else
            {
                passwordBox.Visibility = Visibility.Visible;
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Permet de déplacer la fenêtre en cliquant et en faisant glisser
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
