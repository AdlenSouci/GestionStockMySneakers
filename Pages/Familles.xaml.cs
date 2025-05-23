using GestionStockMySneakers.Models;
using MySqlX.XDevAPI;
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
            pbLoading.Visibility = Visibility.Visible;
            dgFamilles.Visibility = Visibility.Collapsed;

            try
            {
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/famille");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                familles = JsonConvert.DeserializeObject<ObservableCollection<Models.Famille>>(responseBody) ?? new ObservableCollection<Famille>();
                dgFamilles.ItemsSource = familles;
                lblFamilles.Content = $"Familles ({familles.Count})";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
            finally
            {
                pbLoading.Visibility = Visibility.Collapsed;
                dgFamilles.Visibility = Visibility.Visible;
            }
        }

        private void effacer()
        {
            dgFamilles.SelectedItem = null;
            txtNomFamille.Clear();
            txtIdParent.Clear();
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

            MessageBoxResult confirmResult = MessageBox.Show("Êtes-vous sûr de vouloir ajouter cette famille ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmResult == MessageBoxResult.Yes)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(familleAAjouter);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    string token = Settings.Default.UserToken;

                    if (string.IsNullOrEmpty(token))
                        throw new Exception("Token non disponible. Veuillez vous reconnecter.");
                    ApiClient.Client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/famille", content);
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var newFamille = JsonConvert.DeserializeObject<Famille>(responseBody);

                    if (newFamille != null)
                    {
                        familles.Add(newFamille);
                        lblFamilles.Content = $"Familles ({familles.Count})";
                        MessageBox.Show($"Famille '{newFamille.nom_famille}' ajoutée avec succès !");
                        effacer();
                    }
                    else
                    {
                        MessageBox.Show("Famille ajoutée, mais impossible de récupérer les détails depuis l'API.");
                        afficher();
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
        }

        private async void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (dgFamilles.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner une famille à modifier.");
                return;
            }

            Models.Famille familleSelectionnee = dgFamilles.SelectedItem as Models.Famille;
            if (familleSelectionnee == null)
            {
                MessageBox.Show("L'élément sélectionné n'est pas une famille valide.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNomFamille.Text))
            {
                MessageBox.Show("Le nom de la famille ne peut pas être vide.");
                return;
            }

            int? idParentUpdate = null;
            if (!string.IsNullOrWhiteSpace(txtIdParent.Text))
            {
                if (!int.TryParse(txtIdParent.Text, out int parsedIdParentUpdate))
                {
                    MessageBox.Show("L'ID Parent doit être un nombre entier valide ou laissé vide.");
                    return;
                }
                idParentUpdate = parsedIdParentUpdate;
            }

            if (idParentUpdate.HasValue && idParentUpdate.Value == familleSelectionnee.id)
            {
                MessageBox.Show("Une famille ne peut pas être son propre parent.");
                return;
            }

            var familleAModifier = new
            {
                nom_famille = txtNomFamille.Text.Trim(),
                id_parent = idParentUpdate
            };

            MessageBoxResult confirmResult = MessageBox.Show($"Êtes-vous sûr de vouloir modifier la famille '{familleSelectionnee.nom_famille}' ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmResult == MessageBoxResult.Yes)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(familleAModifier, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    string token = Settings.Default.UserToken;

                    if (string.IsNullOrEmpty(token))
                        throw new Exception("Token non disponible. Veuillez vous reconnecter.");
                    ApiClient.Client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage response = await ApiClient.Client.PutAsync(ApiClient.apiUrl + $"/famille/{familleSelectionnee.id}", content);

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
                    var familleDansListe = familles.FirstOrDefault(f => f.id == familleSelectionnee.id);
                    if (familleDansListe != null && updatedFamilleFromApi != null)
                    {
                        familleDansListe.nom_famille = updatedFamilleFromApi.nom_famille;
                        familleDansListe.id_parent = updatedFamilleFromApi.id_parent;
                        dgFamilles.Items.Refresh();
                    }
                    MessageBox.Show("Famille modifiée avec succès !");
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
        }

        private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (dgFamilles.SelectedItem is Famille familleASupprimer)
            {
                MessageBoxResult result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer la famille '{familleASupprimer.nom_famille}' ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        string token = Settings.Default.UserToken;

                        if (string.IsNullOrEmpty(token))
                            throw new Exception("Token non disponible. Veuillez vous reconnecter.");
                        ApiClient.Client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        HttpResponseMessage response = await ApiClient.Client.DeleteAsync(ApiClient.apiUrl + $"/famille/{familleASupprimer.id}");
                        response.EnsureSuccessStatusCode();

                        familles.Remove(familleASupprimer);
                        lblFamilles.Content = $"Familles ({familles.Count})";
                        MessageBox.Show("Famille supprimée avec succès !");
                        effacer();
                    }
                    catch (HttpRequestException httpEx)
                    {
                        if (httpEx.StatusCode == System.Net.HttpStatusCode.Conflict || httpEx.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            MessageBox.Show($"Impossible de supprimer la famille : elle est peut-être utilisée par des articles ou est parente d'autres familles. ({httpEx.StatusCode})");
                        }
                        else
                        {
                            MessageBox.Show($"Erreur API/Réseau lors de la suppression : {httpEx.Message} ({httpEx.StatusCode})");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur lors de la suppression : " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une famille à supprimer.");
            }
        }

        private void dgFamilles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgFamilles.SelectedItem is Models.Famille selectedFamille)
            {
                txtId.Content = selectedFamille.id.ToString();
                txtNomFamille.Text = selectedFamille.nom_famille;
                txtIdParent.Text = selectedFamille.id_parent?.ToString() ?? "";
            }
        }

        private void btnNettoyer_Click(object sender, RoutedEventArgs e)
        {
            effacer();
            MessageBox.Show("Champs nettoyés.");
        }

    }
}
