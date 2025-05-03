using GestionStockMySneakers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace GestionStockMySneakers.Pages
{
    public partial class Familles : Page
    {

        private ObservableCollection<Models.Famille> familles = new ObservableCollection<Models.Famille>();


        public Familles()
        {
            InitializeComponent();
            afficher();
        }
        private async void afficher()
        {
            // Afficher le spinner
            pbLoading.Visibility = Visibility.Visible;
            dgFamilles.Visibility = Visibility.Collapsed;

            try
            {
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/famille");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                familles = JsonConvert.DeserializeObject<ObservableCollection<Models.Famille>>(responseBody) ?? new ObservableCollection<Famille>();

                dgFamilles.ItemsSource = familles;
                lblFamilles.Content = $"Familles ({familles.Count})"; // Afficher le nombre de familles
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Erreur réseau/API affichage: {httpEx.Message}");
                familles.Clear();
                lblFamilles.Content = "Familles (0)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur affichage: " + ex.Message);
                familles.Clear();
                lblFamilles.Content = "Familles (0)";
            }
            finally
            {
                // Masquer le spinner et afficher les données
                pbLoading.Visibility = Visibility.Collapsed;
                dgFamilles.Visibility = Visibility.Visible;
            }
        }

        private void effacer()
        {
            dgFamilles.SelectedItem = null;
            // Les bindings OneWay videront les champs txtId, txtNomFamille, txtIdParent
        }


        // *** SEULE CETTE MÉTHODE EST MODIFIÉE ***
        // Gère le clic sur le bouton "Nouveau" pour AJOUTER une famille
        private async void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNomFamille.Text))
            {
                MessageBox.Show("Veuillez entrer un nom pour la nouvelle famille.");
                return;
            }

            var familleAAjouter = new
            {
                nom_famille = txtNomFamille.Text.Trim()
            };

            try
            {
                string json = JsonConvert.SerializeObject(familleAAjouter);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/famille", content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var newFamille = JsonConvert.DeserializeObject<Famille>(responseBody);

                if (newFamille != null)
                {
                    familles.Add(newFamille); // ← la collection ObservableCollection<Famille> familles
                    lblFamilles.Content = $"Familles ({familles.Count})";
                    MessageBox.Show($"Famille '{newFamille.nom_famille}' ajoutée avec succès !");
                    effacer(); // Si tu as une méthode pour nettoyer les champs
                }
                else
                {
                    MessageBox.Show("Famille ajoutée, mais impossible de récupérer les détails depuis l'API.");
                    afficher(); // Recharge la liste au cas où
                }
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Erreur réseau ou API lors de l'ajout : {httpEx.Message} ({(int?)httpEx.StatusCode})");
            }
            catch (JsonException jsonEx)
            {
                MessageBox.Show($"Erreur JSON lors de l'ajout : {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur inattendue lors de l'ajout : {ex.Message}");
            }
        }



        // --- LES MÉTHODES SUIVANTES RESTENT INCHANGÉES PAR RAPPORT À VOTRE ORIGINAL ---
        private async void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {

            // Vérifier que tous les champs sont remplis
            if (string.IsNullOrEmpty(txtNomFamille.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.");
                return;
            }

            // Traitement de id_parent pour la mise à jour
            int? idParentUpdate = null;
            if (!string.IsNullOrWhiteSpace(txtIdParent.Content.ToString()))
            {
                if (!int.TryParse(txtIdParent.Content.ToString(), out int parsedIdParentUpdate))
                {
                    MessageBox.Show("L'ID Parent doit être un nombre entier valide ou laissé vide pour la mise à jour.");
                    return;
                }
                idParentUpdate = parsedIdParentUpdate;
            }

            var famille = new
            {
                nom_famille = txtNomFamille.Text,
                id_parent = idParentUpdate, // 
            };

            try
            {
                HttpResponseMessage response;
                // Utilisation de NullValueHandling.Include pour envoyer id_parent=null si nécessaire
                string json = JsonConvert.SerializeObject(famille, Formatting.None,
                                           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // La logique originale qui gérait l'ajout et la modification est conservée ici
                // MAIS l'ajout est maintenant géré par btnAjouter_Click
                if (txtId.Content == null || string.IsNullOrEmpty(txtId.Content.ToString())) // Correction: vérifier si l'ID est null ou vide
                {
                    // -------- Bloc d'ajout original (maintenant géré par btnAjouter_Click) --------
                    // // Ajout
                    // response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/famille", content);
                    // // Récupérer la famille ajoutée
                    // var newFamille = JsonConvert.DeserializeObject<Famille>(await response.Content.ReadAsStringAsync());
                    //
                    // // Ajouter la famille directement au DataGrid
                    // if (null != newFamille)
                    //     familles.Add(newFamille);
                    //
                    // MessageBox.Show("Famille ajoutée avec succès !");
                    // -------- Fin du bloc d'ajout original --------
                    MessageBox.Show("Pour ajouter une nouvelle famille, veuillez utiliser le bouton 'Nouveau'.");

                }
                else
                {
                    // Mise à jour
                    int familleId;
                    if (!int.TryParse(txtId.Content.ToString(), out familleId))
                    {
                        MessageBox.Show("ID de famille invalide pour la mise à jour.");
                        return;
                    }

                    response = await ApiClient.Client.PutAsync(ApiClient.apiUrl + $"/famille/{familleId}", content);
                    response.EnsureSuccessStatusCode(); // Mettre EnsureSuccessStatusCode après la requête PUT

                    // Trouver et modifier la famille dans la liste existante
                    var updatedFamille = familles.FirstOrDefault(a => a.id == familleId);
                    if (updatedFamille != null)
                    {
                        updatedFamille.nom_famille = txtNomFamille.Text;
                        updatedFamille.id_parent = idParentUpdate; // Mettre à jour avec la valeur nullable
                    }
                    // dgFamilles.Items.Refresh(); // Normalement non nécessaire avec ObservableCollection si Famille notifie les changements, sinon décommenter

                    MessageBox.Show("Famille mise à jour avec succès !");
                }
                // response.EnsureSuccessStatusCode(); // Mal placé dans l'original, doit être après chaque requête spécifique (POST/PUT)
            }
            catch (FormatException formatEx) // Pour int.Parse ou int.TryParse
            {
                MessageBox.Show($"Erreur de format de nombre: {formatEx.Message}");
            }
            catch (HttpRequestException httpEx) // Pour les erreurs API/Réseau
            {
                MessageBox.Show($"Erreur API/Réseau : {httpEx.Message} ({(int?)httpEx.StatusCode})");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }

        private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            // Vérifie si une famille est sélectionnée
            if (dgFamilles.SelectedItem is Famille familleSelectionnee) // <--- Modifié: dgFamilles, Famille, familleSelectionnee
            {
                // Affiche une boîte de confirmation avant la suppression
                MessageBoxResult result = MessageBox.Show(
                    $"Voulez-vous vraiment supprimer la famille '{familleSelectionnee.nom_famille}' ?", // <--- Modifié: nom_famille
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Envoie une requête DELETE à l'API pour supprimer la famille
                        HttpResponseMessage response = await ApiClient.Client.DeleteAsync(ApiClient.apiUrl + $"/famille/{familleSelectionnee.id}"); // <--- Modifié: /famille/, familleSelectionnee.id
                        response.EnsureSuccessStatusCode(); // Vérifie si la requête a réussi
                        string responseContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(responseContent);

                        // Supprime la famille de la liste locale
                        familles.Remove(familleSelectionnee); // <--- Modifié: familles, familleSelectionnee
                        lblFamilles.Content = $"Familles ({familles.Count})"; // <--- Modifié: lblFamilles, familles.Count
                        effacer(); // Vider le formulaire après suppression
                        MessageBox.Show("Famille supprimée avec succès !"); // <--- Modifié: Message
                    }
                    catch (HttpRequestException httpEx) // Pour les erreurs API/Réseau
                    {
                        // Gérer les erreurs spécifiques (ex: suppression impossible car clé étrangère ou famille parente)
                        MessageBox.Show(httpEx.StatusCode.ToString());
                        if (httpEx.StatusCode == System.Net.HttpStatusCode.Conflict || httpEx.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            // Message adapté pour les familles
                            MessageBox.Show($"Impossible de supprimer la famille : elle est peut-être utilisée par des articles ou est parente d'autres familles. ({httpEx.StatusCode})");
                        }
                        else
                        {
                            MessageBox.Show($"Erreur API/Réseau lors de la suppression: {httpEx.Message} ({httpEx.StatusCode})");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur lors de la suppression: " + ex.Message); // Message générique
                    }
                }
            }
            else // Si rien n'est sélectionné
            {
                MessageBox.Show("Veuillez sélectionner une famille à supprimer."); // <--- Modifié: Message
            }
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Vérifie si la source du clic n'est pas un élément de la grille ou un bouton
            if (e.OriginalSource is Grid || e.OriginalSource is Border || e.OriginalSource is Page)
            {
                if (dgFamilles.SelectedItem != null)
                {
                    effacer(); // Utilise la fonction existante pour désélectionner et vider
                }
            }
        }
    }
}
