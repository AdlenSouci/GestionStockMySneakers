using GestionStockMySneakers.Models;
using Newtonsoft.Json; // Permet de travailler avec le format JSON
using System;
using System.Collections.ObjectModel; // Utilisé pour stocker une liste d'objets observable
using System.Linq; // Fournit des fonctionnalités pour manipuler des collections
using System.Net.Http; // Utilisé pour envoyer des requêtes HTTP
using System.Text; // Utilisé pour encoder du texte
using System.Windows;
using System.Windows.Controls;

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
        }

        // Gère le clic sur le bouton "Ajouter"
        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            effacer(); // Désélectionne l'élément en cours
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
                if (string.IsNullOrWhiteSpace(txtId.Content?.ToString()))
                {
                    // Ajout d'une nouvelle marque
                    response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/marque", content);

                    // Récupère la marque ajoutée et la convertit depuis JSON en objet Marque
                    var newMarque = JsonConvert.DeserializeObject<Marque>(await response.Content.ReadAsStringAsync());

                    // Ajoute directement la nouvelle marque à la liste affichée
                    if (newMarque != null)
                        marques.Add(newMarque);

                    MessageBox.Show("Marque ajoutée avec succès !");
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
                    }
                    dgMarques.Items.Refresh(); // Rafraîchit l'affichage

                    MessageBox.Show("Marque mise à jour avec succès !");
                }
                response.EnsureSuccessStatusCode(); // Vérifie si la requête a réussi
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }

        private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            // Vérifie si une marque est sélectionnée
            if (dgMarques.SelectedItem is Marque marqueSelectionnee)
            {
                // Affiche une boîte de confirmation avant la suppression
                MessageBoxResult result = MessageBox.Show(
                    "Voulez-vous vraiment supprimer cette marque ?",
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
