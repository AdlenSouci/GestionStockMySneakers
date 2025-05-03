using GestionStockMySneakers.Models;
using Newtonsoft.Json; // Permet de travailler avec le format JSON
using System;
using System.Collections.ObjectModel; // Utilisé pour stocker une liste d'objets observable
using System.Linq; // Fournit des fonctionnalités pour manipuler des collections
using System.Net.Http; // Utilisé pour envoyer des requêtes HTTP
using System.Text; // Utilisé pour encoder du texte
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GestionStockMySneakers.Pages
{
    public partial class Marques : Page
    {
        // Liste observable qui contient les marques.
        // Cela permet d'afficher dynamiquement les données dans l'interface utilisateur.
        private ObservableCollection<Models.Marque> marques = new ObservableCollection<Models.Marque>();

        public Marques()
        {
            InitializeComponent();
            afficher(); // Appelle la fonction pour récupérer et afficher les marques dès que la page est ouverte
        }

        private async void afficher()
        {
            // Affiche un indicateur de chargement pendant la récupération des données
            pbLoading.Visibility = Visibility.Visible;
            dgMarques.Visibility = Visibility.Collapsed;

            try
            {
                // Effectue une requête HTTP GET pour récupérer la liste des marques depuis l'API
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/marque");
                response.EnsureSuccessStatusCode(); // Vérifie si la requête a réussi

                string responseBody = await response.Content.ReadAsStringAsync(); // Lit la réponse sous forme de texte JSON

                // Convertit le JSON reçu en une liste d'objets Marque
                marques = JsonConvert.DeserializeObject<ObservableCollection<Models.Marque>>(responseBody) ?? new ObservableCollection<Marque>();

                // Affiche les marques dans le DataGrid
                dgMarques.ItemsSource = marques;

                // Met à jour l'affichage du nombre total de marques
                lblMarques.Content = $"Marques ({marques.Count})";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message); // Affiche une erreur si la requête échoue
            }
            finally
            {
                // Cache l'indicateur de chargement et affiche les données
                pbLoading.Visibility = Visibility.Collapsed;
                dgMarques.Visibility = Visibility.Visible;
            }
        }

        // Efface la sélection actuelle dans la liste des marques
        private void effacer()
        {
            dgMarques.SelectedItem = null;
            // Grâce au Binding OneWay, vider SelectedItem videra aussi les champs liés.
            // Si les bindings étaient TwoWay ou si le vidage ne fonctionnait pas,
            // il faudrait vider explicitement :
            // txtId.Content = null;
            // txtNomMarque.Text = "";
        }

        // *** SEULE CETTE MÉTHODE EST MODIFIÉE ***
        // Gère le clic sur le bouton "Nouveau" pour AJOUTER une marque
        private async void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validation du champ Nom Marque
            if (string.IsNullOrWhiteSpace(txtNomMarque.Text))
            {
                MessageBox.Show("Veuillez entrer un nom pour la nouvelle marque.");
                return;
            }

            // 2. Création de l'objet marque à envoyer
            var marqueAAjouter = new
            {
                nom_marque = txtNomMarque.Text.Trim() // .Trim() enlève les espaces avant/après
            };

            // 3. Appel API POST pour ajouter la marque
            try
            {
                string json = JsonConvert.SerializeObject(marqueAAjouter);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Envoi de la requête POST
                HttpResponseMessage response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/marque", content);
                response.EnsureSuccessStatusCode(); // Lève une exception si le statut HTTP n'est pas 2xx

                // 4. Récupérer la marque ajoutée depuis la réponse de l'API
                var responseBody = await response.Content.ReadAsStringAsync();
                var newMarque = JsonConvert.DeserializeObject<Marque>(responseBody);

                // 5. Mettre à jour l'interface utilisateur
                if (newMarque != null)
                {
                    marques.Add(newMarque); // Ajoute la nouvelle marque à la collection observable
                    lblMarques.Content = $"Marques ({marques.Count})"; // Met à jour le compteur
                    MessageBox.Show($"Marque '{newMarque.nom_marque}' ajoutée avec succès !");
                    effacer(); // Appelle la fonction pour vider le formulaire après l'ajout réussi
                }
                else
                {
                    // Si l'API retourne une réponse vide ou mal formée après un succès HTTP 2xx
                    MessageBox.Show("Marque ajoutée, mais impossible de récupérer les détails depuis l'API.");
                    afficher(); // Optionnel: recharger toute la liste pour être sûr
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Gère les erreurs réseau ou les réponses API non-2xx
                MessageBox.Show($"Erreur réseau ou API lors de l'ajout : {httpEx.Message} ({(int?)httpEx.StatusCode})");
            }
            catch (JsonException jsonEx)
            {
                // Gère les erreurs si la réponse de l'API n'est pas du JSON valide
                MessageBox.Show($"Erreur de format JSON lors de l'ajout : {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                // Gère toute autre erreur inattendue
                MessageBox.Show($"Erreur inattendue lors de l'ajout : {ex.Message}");
            }
        }
      



        private async void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Vérifie si le champ du nom de la marque est vide
            if (string.IsNullOrEmpty(txtNomMarque.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.");
                return;
            }

            // Crée un objet contenant les données à envoyer
            var marque = new
            {
                nom_marque = txtNomMarque.Text,
            };

            try
            {
                HttpResponseMessage response;
                string json = JsonConvert.SerializeObject(marque); // Convertit l'objet en JSON
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Vérifie si un ID est présent : si non, c'est un ajout, sinon c'est une modification
                // NOTE: Selon la nouvelle logique, ce 'if' ne devrait plus gérer l'ajout.
                // Il ne traitera que la partie 'else' (modification) car l'ajout est géré par btnAjouter_Click.
                if (string.IsNullOrWhiteSpace(txtId.Content?.ToString()))
                {
              
                    MessageBox.Show("Pour ajouter une nouvelle marque, veuillez utiliser le bouton 'Nouveau'.");
                }
                else
                {
                    // Mise à jour d'une marque existante
                    int marqueId = int.Parse(txtId.Content.ToString());
                    response = await ApiClient.Client.PutAsync(ApiClient.apiUrl + $"/marque/{marqueId}", content);

                    // Recherche la marque modifiée dans la liste et met à jour son nom
                    var updatedMarque = marques.FirstOrDefault(a => a.id == marqueId);
                    if (updatedMarque != null)
                    {
                        updatedMarque.nom_marque = txtNomMarque.Text;
                        // Mettre à jour updated_at si nécessaire et si l'API le renvoie
                    }
                    // dgMarques.Items.Refresh(); // Normalement pas nécessaire avec ObservableCollection + PropertyChanged (si implémenté dans Marque)
                    // Si Marque n'implémente pas INotifyPropertyChanged, Refresh peut être utile.
                    // Ou mettre à jour l'objet directement comme fait ci-dessus suffit visuellement.

                    MessageBox.Show("Marque mise à jour avec succès !");
                    response.EnsureSuccessStatusCode(); // Vérifie si la requête PUT a réussi
                }
                // L'appel EnsureSuccessStatusCode était mal placé ici dans l'original,
                // il devrait être après chaque requête (POST ou PUT). Corrigé dans les nouvelles logiques.
                // response.EnsureSuccessStatusCode();
            }
            catch (FormatException) // Pour int.Parse
            {
                MessageBox.Show("L'ID de la marque sélectionnée est invalide.");
            }
            catch (HttpRequestException httpEx) // Pour les erreurs API/Réseau
            {
                MessageBox.Show($"Erreur API/Réseau : {httpEx.Message} ({(int?)httpEx.StatusCode})");
            }
            catch (Exception ex) // Pour toute autre erreur
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }


        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Vérifie si la source du clic n'est pas un élément de la grille ou un bouton
            if (e.OriginalSource is Grid || e.OriginalSource is Border || e.OriginalSource is Page)
            {
                if (dgMarques.SelectedItem != null)
                {
                    effacer(); // Utilise la fonction existante pour désélectionner et vider
                }
            }
        }

        private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            // Vérifie si une marque est sélectionnée
            if (dgMarques.SelectedItem is Marque marqueSelectionnee)
            {
                // Affiche une boîte de confirmation avant la suppression
                MessageBoxResult result = MessageBox.Show(
                    $"Voulez-vous vraiment supprimer la marque '{marqueSelectionnee.nom_marque}' ?", // Message plus précis
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Envoie une requête DELETE à l'API pour supprimer la marque
                        HttpResponseMessage response = await ApiClient.Client.DeleteAsync(ApiClient.apiUrl + $"/marque/{marqueSelectionnee.id}");
                        response.EnsureSuccessStatusCode(); // Vérifie si la requête a réussi

                        // Supprime la marque de la liste locale
                        marques.Remove(marqueSelectionnee);
                        lblMarques.Content = $"Marques ({marques.Count})"; // Mettre à jour le compteur
                        effacer(); // Vider le formulaire après suppression
                        // MessageBox.Show("Marque supprimée avec succès !"); // Souvent redondant après suppression visuelle
                    }
                    catch (HttpRequestException httpEx) // Pour les erreurs API/Réseau
                    {
                        // Gérer les erreurs spécifiques (ex: suppression impossible car clé étrangère)
                        if (httpEx.StatusCode == System.Net.HttpStatusCode.Conflict || httpEx.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            MessageBox.Show($"Impossible de supprimer la marque : elle est probablement utilisée par des articles. ({httpEx.StatusCode})");
                        }
                        else
                        {
                            MessageBox.Show($"Erreur API/Réseau lors de la suppression: {httpEx.Message} ({httpEx.StatusCode})");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur lors de la suppression: " + ex.Message);
                    }
                }
            }
            else // Si rien n'est sélectionné
            {
                MessageBox.Show("Veuillez sélectionner une marque à supprimer.");
            }
        }

    }


    
}
