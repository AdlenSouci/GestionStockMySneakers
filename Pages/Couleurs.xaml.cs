using GestionStockMySneakers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data; // Assurez-vous que l'import est là si vous l'utilisez

namespace GestionStockMySneakers.Pages
{
    public partial class Couleurs : Page
    {
        private ObservableCollection<Models.Couleur> couleurs = new ObservableCollection<Models.Couleur>();

        public Couleurs()
        {
            InitializeComponent();
            // Lier le DataGrid à la collection
            dgCouleurs.ItemsSource = couleurs;
            afficher();
        }

        private async void afficher()
        {
            pbLoading.Visibility = Visibility.Visible;
            dgCouleurs.Visibility = Visibility.Collapsed;

            try
            {
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/couleur");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Désérialiser dans la collection existante ou une nouvelle si null
                var couleursDepuisApi = JsonConvert.DeserializeObject<ObservableCollection<Models.Couleur>>(responseBody) ?? new ObservableCollection<Couleur>();

                // Effacer l'ancienne collection et ajouter les nouvelles couleurs
                couleurs.Clear();
                foreach (var couleur in couleursDepuisApi)
                {
                    couleurs.Add(couleur);
                }

                lblCouleurs.Content = $"Couleurs ({couleurs.Count})";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur affichage couleurs: " + ex.Message);
                couleurs.Clear();
                lblCouleurs.Content = "Couleurs (0)";
            }
            finally
            {
                pbLoading.Visibility = Visibility.Collapsed;
                dgCouleurs.Visibility = Visibility.Visible;
            }
        }

        private void effacer()
        {
            dgCouleurs.SelectedItem = null;
            // Effacer les champs de détails spécifiques aux couleurs
            // txtId est un Label, son contenu est effacé par binding
            txtNomCouleur.Clear();
            // txtIdParent n'existe pas pour les couleurs
        }

        private void btnNettoyer_Click(object sender, RoutedEventArgs e)
        {
            effacer();
            MessageBox.Show("Champs nettoyés.");
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            // En mode "Ajouter", on nettoie simplement les champs pour la saisie d'une nouvelle couleur.
            // La logique d'ajout réelle est déclenchée par le bouton "Modifier" si l'ID est vide.
            effacer();
        }

        private async void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer le nom de la couleur du champ de texte
            string nomCouleur = txtNomCouleur.Text.Trim();

            if (string.IsNullOrWhiteSpace(nomCouleur))
            {
                MessageBox.Show("Veuillez entrer un nom pour la couleur.");
                return;
            }

            // Créer l'objet à envoyer à l'API
            var couleurData = new
            {
                nom_couleur = nomCouleur,
            };

            HttpResponseMessage response;
            string json = JsonConvert.SerializeObject(couleurData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Déterminer si c'est un ajout ou une modification basé sur l'ID
            if (string.IsNullOrWhiteSpace(txtId.Content?.ToString()))
            {
                // --- Logique d'AJOUT ---
                MessageBoxResult confirmResult = MessageBox.Show("Êtes-vous sûr de vouloir ajouter cette couleur ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirmResult == MessageBoxResult.Yes)
                {
                    try
                    {
                        response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/couleur", content);

                        if (!response.IsSuccessStatusCode)
                        {
                            string errorContent = await response.Content.ReadAsStringAsync();
                            try
                            {
                                var errorObj = JsonConvert.DeserializeObject<dynamic>(errorContent);
                                // Gérer spécifiquement l'erreur 409 Conflict pour nom unique
                                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                                {
                                    MessageBox.Show($"Cette couleur existe déjà.");
                                }
                                else
                                {
                                    MessageBox.Show($"Erreur lors de l'ajout : {errorObj?.message ?? errorContent} ({(int)response.StatusCode})");
                                }
                            }
                            catch
                            {
                                MessageBox.Show($"Erreur lors de l'ajout : {errorContent} ({(int)response.StatusCode})");
                            }
                            return; // Sortir si l'ajout a échoué
                        }

                        // Si l'ajout a réussi
                        var newCouleur = JsonConvert.DeserializeObject<Couleur>(await response.Content.ReadAsStringAsync());

                        if (newCouleur != null)
                        {
                            couleurs.Add(newCouleur);
                            lblCouleurs.Content = $"Couleurs ({couleurs.Count})";
                            MessageBox.Show($"Couleur '{newCouleur.nom_couleur}' ajoutée avec succès !");
                            effacer(); // Nettoyer les champs après succès
                        }
                        else
                        {
                            MessageBox.Show("Couleur ajoutée, mais impossible de récupérer les détails depuis l'API.");
                            afficher(); // Recharger la liste pour synchronisation
                        }
                    }
                    catch (HttpRequestException httpEx)
                    {
                        MessageBox.Show($"Erreur réseau ou API lors de l'ajout : {httpEx.Message} ({(int?)httpEx.StatusCode})");
                    }
                    catch (JsonException jsonEx)
                    {
                        MessageBox.Show($"Erreur de format JSON lors de la réponse API (ajout) : {jsonEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur inattendue lors de l'ajout : {ex.Message}");
                    }
                }
            }
            else
            {
                // --- Logique de MODIFICATION ---
                int couleurId;
                if (!int.TryParse(txtId.Content.ToString(), out couleurId))
                {
                    MessageBox.Show("ID de couleur invalide pour la modification.");
                    return;
                }

                Models.Couleur couleurSelectionnee = dgCouleurs.SelectedItem as Models.Couleur;
                if (couleurSelectionnee == null || couleurSelectionnee.id != couleurId)
                {
                    // S'assurer que l'ID dans le champ correspond bien à l'élément sélectionné
                    MessageBox.Show("Erreur interne: La couleur sélectionnée ne correspond pas à l'ID affiché.");
                    return;
                }


                MessageBoxResult confirmResult = MessageBox.Show($"Êtes-vous sûr de vouloir modifier la couleur '{couleurSelectionnee.nom_couleur}' ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirmResult == MessageBoxResult.Yes)
                {
                    try
                    {
                        response = await ApiClient.Client.PutAsync(ApiClient.apiUrl + $"/couleur/{couleurId}", content);

                        if (!response.IsSuccessStatusCode)
                        {
                            string errorContent = await response.Content.ReadAsStringAsync();
                            try
                            {
                                var errorObj = JsonConvert.DeserializeObject<dynamic>(errorContent);
                                // Gérer spécifiquement l'erreur 409 Conflict pour nom unique
                                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                                {
                                    MessageBox.Show($"Cette couleur existe déjà.");
                                }
                                else
                                {
                                    MessageBox.Show($"Erreur lors de la modification : {errorObj?.message ?? errorContent} ({(int)response.StatusCode})");
                                }
                            }
                            catch
                            {
                                MessageBox.Show($"Erreur lors de la modification : {errorContent} ({(int)response.StatusCode})");
                            }
                            return; // Sortir si la modification a échoué
                        }

                        // Si la modification a réussi
                        var updatedCouleurFromApi = JsonConvert.DeserializeObject<Couleur>(await response.Content.ReadAsStringAsync());
                        // Trouver et modifier la couleur dans la liste existante pour rafraîchir l'UI
                        var couleurDansListe = couleurs.FirstOrDefault(c => c.id == couleurId);
                        if (couleurDansListe != null && updatedCouleurFromApi != null)
                        {
                            // Mettre à jour les propriétés de l'objet local
                            couleurDansListe.nom_couleur = updatedCouleurFromApi.nom_couleur;
                            // Forcer le rafraîchissement du DataGrid si nécessaire (ObservableCollection devrait suffire)
                            // dgCouleurs.Items.Refresh();
                        }
                        MessageBox.Show("Couleur mise à jour avec succès !");
                        // Ne pas effacer les champs si la modification est réussie, l'élément reste sélectionné
                    }
                    catch (HttpRequestException httpEx)
                    {
                        MessageBox.Show($"Erreur réseau ou API lors de la modification : {httpEx.Message} ({(int?)httpEx.StatusCode})");
                    }
                    catch (JsonException jsonEx)
                    {
                        MessageBox.Show($"Erreur de format JSON lors de la réponse API (modification) : {jsonEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur inattendue lors de la modification : {ex.Message}");
                    }
                }
            }
            // Assurez-vous que la méthode EnsureSuccessStatusCode() n'est pas appelée si vous gérez les codes d'erreur manuellement
            // response.EnsureSuccessStatusCode(); // Cette ligne devrait être retirée ou placée dans un bloc success
        }

        private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (dgCouleurs.SelectedItem is Couleur couleurSelectionnee)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Voulez-vous vraiment supprimer la couleur '{couleurSelectionnee.nom_couleur}' ?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        HttpResponseMessage response = await ApiClient.Client.DeleteAsync(ApiClient.apiUrl + $"/couleur/{couleurSelectionnee.id}");

                        if (!response.IsSuccessStatusCode)
                        {
                            string errorContent = await response.Content.ReadAsStringAsync();
                            try
                            {
                                var errorObj = JsonConvert.DeserializeObject<dynamic>(errorContent);
                                MessageBox.Show($"Erreur lors de la suppression : {errorObj?.message ?? errorContent} ({(int)response.StatusCode})");
                            }
                            catch
                            {
                                MessageBox.Show($"Erreur lors de la suppression : {errorContent} ({(int)response.StatusCode})");
                            }
                            return; // Sortir si la suppression a échoué
                        }


                        // Supprimer la couleur de la liste locale
                        if (couleurs.Remove(couleurSelectionnee))
                        {
                            lblCouleurs.Content = $"Couleurs ({couleurs.Count})";
                            MessageBox.Show("Couleur supprimée avec succès !");
                            effacer(); // Nettoyer les champs après suppression réussie
                        }
                        else
                        {
                            MessageBox.Show("Couleur supprimée sur le serveur, mais pas trouvée dans la liste locale.");
                            afficher(); // Recharger la liste pour synchronisation
                        }

                    }
                    catch (HttpRequestException httpEx)
                    {
                        // Gérer spécifiquement les erreurs de conflit si la couleur est utilisée ailleurs (ex: articles)
                        if (httpEx.StatusCode == System.Net.HttpStatusCode.Conflict || httpEx.StatusCode == System.Net.HttpStatusCode.BadRequest) // L'API Famille renvoyait BadRequest, l'API Couleur actuelle 409
                        {
                            MessageBox.Show($"Impossible de supprimer la couleur : elle est peut-être utilisée par des articles. ({(int?)httpEx.StatusCode})");
                        }
                        else
                        {
                            MessageBox.Show($"Erreur réseau ou API lors de la suppression : {httpEx.Message} ({(int?)httpEx.StatusCode})");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur inattendue lors de la suppression : " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une couleur à supprimer.");
            }
        }

        private void dgCouleurs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCouleurs.SelectedItem is Models.Couleur selectedCouleur)
            {
                txtId.Content = selectedCouleur.id.ToString();
                txtNomCouleur.Text = selectedCouleur.nom_couleur;
                // Pas de champ parent pour les couleurs
            }
            else
            {
                // Si la sélection est nulle (après effacer ou suppression)
                txtId.Content = string.Empty;
                txtNomCouleur.Clear();
            }
        }

        

    }
}
