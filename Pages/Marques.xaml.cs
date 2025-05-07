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
        private ObservableCollection<Models.Marque> marques = new ObservableCollection<Models.Marque>();

        public Marques()
        {
            InitializeComponent();
            afficher(); // Appelle la fonction pour récupérer et afficher les marques dès que la page est ouverte
        }

        private async void afficher()
        {
            pbLoading.Visibility = Visibility.Visible;
            dgMarques.Visibility = Visibility.Collapsed;

            try
            {
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/marque");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                marques = JsonConvert.DeserializeObject<ObservableCollection<Models.Marque>>(responseBody) ?? new ObservableCollection<Marque>();
                dgMarques.ItemsSource = marques;
                lblMarques.Content = $"Marques ({marques.Count})";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
            finally
            {
                pbLoading.Visibility = Visibility.Collapsed;
                dgMarques.Visibility = Visibility.Visible;
            }
        }

        private void effacer()
        {
            dgMarques.SelectedItem = null;
            txtNomMarque.Clear(); // Efface le champ de texte
        }

        private async void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNomMarque.Text))
            {
                MessageBox.Show("Veuillez entrer un nom pour la nouvelle marque.");
                return;
            }

            var marqueAAjouter = new
            {
                nom_marque = txtNomMarque.Text.Trim()
            };

            try
            {
                string json = JsonConvert.SerializeObject(marqueAAjouter);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                MessageBoxResult confirmResult = MessageBox.Show("Êtes-vous sûr de vouloir ajouter cette marque ?", "Confirmation", MessageBoxButton.YesNo);
                if (confirmResult == MessageBoxResult.Yes)
                {
                    HttpResponseMessage response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/marque", content);
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var newMarque = JsonConvert.DeserializeObject<Marque>(responseBody);

                    if (newMarque != null)
                    {
                        marques.Add(newMarque);
                        lblMarques.Content = $"Marques ({marques.Count})";
                        MessageBox.Show($"Marque '{newMarque.nom_marque}' ajoutée avec succès !");
                    }
                    else
                    {
                        MessageBox.Show("Marque ajoutée, mais impossible de récupérer les détails depuis l'API.");
                        afficher();
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Erreur réseau ou API lors de l'ajout : {httpEx.Message} ({(int?)httpEx.StatusCode})");
            }
            catch (JsonException jsonEx)
            {
                MessageBox.Show($"Erreur de format JSON lors de l'ajout : {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur inattendue lors de l'ajout : {ex.Message}");
            }
        }

        private async void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNomMarque.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.");
                return;
            }

            var marque = new
            {
                nom_marque = txtNomMarque.Text,
            };

            try
            {
                HttpResponseMessage response;
                string json = JsonConvert.SerializeObject(marque);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                if (string.IsNullOrWhiteSpace(txtId.Content?.ToString()))
                {
                    MessageBox.Show("Pour ajouter une nouvelle marque, veuillez utiliser le bouton 'Nouveau'.");
                }
                else
                {
                    int marqueId = int.Parse(txtId.Content.ToString());
                    MessageBoxResult result = MessageBox.Show("Êtes-vous sûr de vouloir modifier cette marque ?", "Confirmation", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        response = await ApiClient.Client.PutAsync(ApiClient.apiUrl + $"/marque/{marqueId}", content);
                        response.EnsureSuccessStatusCode();

                        var updatedMarque = marques.FirstOrDefault(m => m.id == marqueId);
                        if (updatedMarque != null)
                        {
                            updatedMarque.nom_marque = txtNomMarque.Text; // Met à jour la marque dans la collection observable
                            MessageBox.Show("Marque modifiée avec succès !");
                            lblMarques.Content = $"Marques ({marques.Count})";
                        }
                        effacer();
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Erreur réseau ou API lors de la modification : {httpEx.Message} ({(int?)httpEx.StatusCode})");
            }
            catch (JsonException jsonEx)
            {
                MessageBox.Show($"Erreur de format JSON lors de la modification : {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur inattendue lors de la modification : {ex.Message}");
            }
        }

        private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (dgMarques.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner une marque à supprimer.");
                return;
            }

            var marqueASupprimer = (Models.Marque)dgMarques.SelectedItem;
            MessageBoxResult result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer la marque '{marqueASupprimer.nom_marque}' ?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    HttpResponseMessage response = await ApiClient.Client.DeleteAsync(ApiClient.apiUrl + $"/marque/{marqueASupprimer.id}");
                    response.EnsureSuccessStatusCode();

                    marques.Remove(marqueASupprimer); // Supprime la marque de la collection observable
                    lblMarques.Content = $"Marques ({marques.Count})";
                    MessageBox.Show("Marque supprimée avec succès !");
                    effacer();
                }
                catch (HttpRequestException httpEx)
                {
                    MessageBox.Show($"Erreur réseau ou API lors de la suppression : {httpEx.Message} ({(int?)httpEx.StatusCode})");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur inattendue lors de la suppression : {ex.Message}");
                }
            }
        }

        private void dgMarques_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgMarques.SelectedItem is Models.Marque selectedMarque)
            {
                txtId.Content = selectedMarque.id.ToString();
                txtNomMarque.Text = selectedMarque.nom_marque;
            }
        }

        private void btnNettoyer_Click(object sender, RoutedEventArgs e)
        {
            effacer();
        }
    }
}
