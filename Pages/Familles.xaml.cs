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
          
        }


     
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
                // Vérification de l'existence de la famille avant d'ajouter
                var checkResponse = await ApiClient.Client.GetAsync(ApiClient.apiUrl + $"/famille?nom_famille={familleAAjouter.nom_famille}");
                if (checkResponse.IsSuccessStatusCode)
                {
                    var existingFamilles = JsonConvert.DeserializeObject<ObservableCollection<Models.Famille>>(await checkResponse.Content.ReadAsStringAsync());
                    if (existingFamilles != null && existingFamilles.Count > 0)
                    {
                        MessageBox.Show("Erreur : Une famille avec ce nom existe déjà.");
                        return;
                    }
                }

                // Ajout de la nouvelle famille
                string json = JsonConvert.SerializeObject(familleAAjouter);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/famille", content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var newFamille = JsonConvert.DeserializeObject<Famille>(responseBody);

                if (newFamille != null)
                {
                    familles.Add(newFamille); // Ajoute à l'ObservableCollection
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

        private async void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (dgFamilles.SelectedItem == null)
            {
                MessageBox.Show("Pour modifier une famille, veuillez sélectionner une famille existante dans la liste.");
                return;
            }

            Models.Famille familleSelectionnee = dgFamilles.SelectedItem as Models.Famille;
            if (familleSelectionnee == null) // Juste une sécurité supplémentaire
            {
                MessageBox.Show("L'élément sélectionné n'est pas une famille valide.");
                return;
            }

            int familleId = familleSelectionnee.id; // On prend l'ID directement de l'objet sélectionné

            // txtNomFamille est un TextBox, donc on lit sa propriété Text.
            // Elle devrait être remplie par le binding TwoWay avec SelectedItem.nom_famille.
            if (string.IsNullOrWhiteSpace(txtNomFamille.Text))
            {
                MessageBox.Show("Le nom de la famille ne peut pas être vide.");
                return;
            }

            int? idParentUpdate = null;
            // txtIdParent est maintenant un TextBox, on lit sa propriété Text.
            if (!string.IsNullOrWhiteSpace(txtIdParent.Text))
            {
                if (int.TryParse(txtIdParent.Text, out int parsedIdParentUpdate))
                {
                    idParentUpdate = parsedIdParentUpdate;
                }
                else
                {
                    MessageBox.Show("L'ID Parent doit être un nombre entier valide ou laissé vide.");
                    return;
                }
            }

            if (idParentUpdate.HasValue && idParentUpdate.Value == familleId)
            {
                MessageBox.Show("Une famille ne peut pas être son propre parent.");
                return;
            }

            var familleAModifier = new
            {
                nom_famille = txtNomFamille.Text.Trim(),
                id_parent = idParentUpdate
            };

            try
            {
                // ... (le reste de votre logique PUT API reste la même) ...
                string json = JsonConvert.SerializeObject(familleAModifier, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await ApiClient.Client.PutAsync(ApiClient.apiUrl + $"/famille/{familleId}", content); // Utilisez familleId récupéré

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<dynamic>(errorContent);
                        MessageBox.Show($"Erreur lors de la modification : {errorObj?.message ?? errorContent} ({(int)response.StatusCode})");
                    }
                    catch
                    {
                        MessageBox.Show($"Erreur lors de la modification : {errorContent} ({(int)response.StatusCode})");
                    }
                    return;
                }

                var updatedFamilleFromApi = JsonConvert.DeserializeObject<Famille>(await response.Content.ReadAsStringAsync());
                var familleDansListe = familles.FirstOrDefault(f => f.id == familleId);
                if (familleDansListe != null && updatedFamilleFromApi != null)
                {
                    familleDansListe.nom_famille = updatedFamilleFromApi.nom_famille;
                    familleDansListe.id_parent = updatedFamilleFromApi.id_parent;
                    dgFamilles.Items.Refresh();
                }
                MessageBox.Show("Famille mise à jour avec succès !");
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Erreur API/Réseau : {httpEx.Message} ({(int?)httpEx.StatusCode})");
            }
            catch (FormatException)
            {
                MessageBox.Show("Erreur de format de nombre pour ID Parent.");
            }
            catch (JsonException jsonEx)
            {
                MessageBox.Show($"Erreur de format JSON : {jsonEx.Message}");
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
                        MessageBox.Show("Erreur lors de la suppression: " + ex.Message); 
                    }
                }
            }
            else // Si rien n'est sélectionné
            {
                MessageBox.Show("Veuillez sélectionner une famille à supprimer."); 
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
