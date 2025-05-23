using GestionStockMySneakers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GestionStockMySneakers.Pages
{
    public partial class Avis : Page
    {
        private ObservableCollection<Models.Avis> avis = new ObservableCollection<Models.Avis>();

        public Avis()
        {
            InitializeComponent();
            afficher();
        }

        private async void afficher()
        {
            pbLoading.Visibility = Visibility.Visible;
            dgAvis.Visibility = Visibility.Collapsed;

            try
            {

                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/avis");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                // Désérialiser en List<Models.Avis> d'abord

                //----------------- CETTE LIGNE POSE PROBLEME ---------------------
                //var listAvis = JsonConvert.DeserializeObject<List<Models.Avis>>(responseBody);
                
                avis = JsonConvert.DeserializeObject<ObservableCollection<Models.Avis>>(responseBody) ?? new ObservableCollection<Models.Avis>();


                //avis = new ObservableCollection<Models.Avis>(listAvis ?? new List<Models.Avis>());

                dgAvis.ItemsSource = avis;
                lblAvis.Content = $"Avis ({avis.Count})"; // Afficher le nombre d'avis
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show("Erreur de connexion : " + httpEx.Message);
            }
            catch (JsonException jsonEx)
            {
                MessageBox.Show("Erreur de traitement des données : " + jsonEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur inattendue : " + ex.Message);
            }
            finally
            {
                pbLoading.Visibility = Visibility.Collapsed;
                dgAvis.Visibility = Visibility.Visible;
            }
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (dgAvis.SelectedItem != null)
            {
                dgAvis.SelectedItem = null;
            }
        }

        private async void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            // Logique d'AJOUT d'un nouvel avis
            // Lire les champs pour l'ajout (User ID, Article ID, Contenu, Note)
            string userIdText = txtUserId.Text.Trim();
            string articleIdText = txtArticleId.Text.Trim();
            string contenu = txtContenu.Text.Trim();
            string noteText = txtNote.Text.Trim();

            // Valider les champs nécessaires pour l'ajout
            if (string.IsNullOrWhiteSpace(userIdText) || string.IsNullOrWhiteSpace(articleIdText) ||
                string.IsNullOrWhiteSpace(contenu) || string.IsNullOrWhiteSpace(noteText))
            {
                MessageBox.Show("Veuillez remplir User ID, Article ID, Contenu et Note pour ajouter un avis.");
                return;
            }

            // Valider les formats pour l'ajout
            if (!int.TryParse(userIdText, out int userId))
            {
                MessageBox.Show("User ID doit être un nombre entier valide.");
                txtUserId.Focus(); return;
            }
            if (!int.TryParse(articleIdText, out int articleId))
            {
                MessageBox.Show("Article ID doit être un nombre entier valide.");
                txtArticleId.Focus(); return;
            }
            if (!int.TryParse(noteText, out int note) || note < 1 || note > 5)
            {
                MessageBox.Show("Note doit être un nombre entier entre 1 et 5.");
                txtNote.Focus(); return;
            }


            // Créer l'objet avis à envoyer à l'API
            var avisData = new
            {
                user_id = userId,
                article_id = articleId,
                contenu = contenu,
                note = note,
            };

            MessageBoxResult confirmResult = MessageBox.Show("Êtes-vous sûr de vouloir ajouter cet avis ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmResult == MessageBoxResult.Yes)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(avisData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    string token = Settings.Default.UserToken;

                    if (string.IsNullOrEmpty(token))
                        throw new Exception("Token non disponible. Veuillez vous reconnecter.");
                    ApiClient.Client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    // Appel API pour l'ajout
                    HttpResponseMessage response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/avis", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var errorObj = JsonConvert.DeserializeObject<dynamic>(errorContent);
                            if (response.StatusCode == HttpStatusCode.Conflict) // 409
                            {
                                MessageBox.Show($"Cet utilisateur a déjà laissé un avis pour cet article.");
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
                        return;
                    }

                    var newAvis = JsonConvert.DeserializeObject<Models.Avis>(await response.Content.ReadAsStringAsync());
                    if (newAvis != null)
                    {
                        avis.Add(newAvis); // Ajouter à la collection locale
                        lblAvis.Content = $"Avis ({avis.Count})";
                        MessageBox.Show("Avis ajouté avec succès !");
                        effacer(); // Nettoyer les champs après succès
                    }
                    else
                    {
                        MessageBox.Show("Avis ajouté, mais impossible de récupérer les détails depuis l'API.");
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
        //private async void btnModifier_Click(object sender, RoutedEventArgs e)
        //{
        //    // Vérifier que tous les champs sont remplis
        //    if (string.IsNullOrEmpty(txtUserId.Text) || string.IsNullOrEmpty(txtArticleId.Text) ||
        //        string.IsNullOrEmpty(txtContenu.Text) || string.IsNullOrEmpty(txtNote.Text))
        //    {
        //        MessageBox.Show("Veuillez remplir tous les champs obligatoires.");
        //        return;
        //    }

        //    var avisData = new
        //    {
        //        user_id = txtUserId.Text, 
        //        article_id = txtArticleId.Text,
        //        contenu = txtContenu.Text,
        //        note = int.Parse(txtNote.Text),
        //        created_at = DateTime.Now 
        //    };

        //    try
        //    {
        //        HttpResponseMessage response;
        //        string json = JsonConvert.SerializeObject(avisData);
        //        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //        // Ajout
        //        response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/avis", content);
        //        response.EnsureSuccessStatusCode(); // Vérifiez si la réponse est réussie

        //        var newAvis = JsonConvert.DeserializeObject<Models.Avis>(await response.Content.ReadAsStringAsync());
        //        if (newAvis != null)
        //            avis.Add(newAvis);

        //        MessageBox.Show("Avis ajouté avec succès !");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Erreur : " + ex.Message);
        //    }
        //}

        private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (dgAvis.SelectedItem is Models.Avis avisSelectionne)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Voulez-vous vraiment supprimer cet avis ?",
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
                        HttpResponseMessage response = await ApiClient.Client.DeleteAsync(ApiClient.apiUrl + $"/avis/{avisSelectionne.id}");
                        response.EnsureSuccessStatusCode(); // Vérifiez si la réponse est réussie

                        avis.Remove(avisSelectionne);
                        MessageBox.Show("Avis supprimé avec succès !");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur : " + ex.Message);
                    }
                }
            }
        }

        private void effacer()
        {
            txtUserId.Clear();
            txtArticleId.Clear();
            txtContenu.Clear();
            txtNote.Clear();
            txtId.Clear(); 
        }

        private async void btnEnvoyerReponse_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text) || string.IsNullOrWhiteSpace(txtReponse.Text))
            {
                MessageBox.Show("Veuillez sélectionner un avis et saisir une réponse.");
                return;
            }

            var reponseData = new
            {
                reponse = txtReponse.Text
            };

            try
            {
                string json = JsonConvert.SerializeObject(reponseData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                string token = Settings.Default.UserToken;

                if (string.IsNullOrEmpty(token))
                    throw new Exception("Token non disponible. Veuillez vous reconnecter.");
                ApiClient.Client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await ApiClient.Client.PutAsync(ApiClient.apiUrl + "/avis/" + txtId.Text + "/repondre", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Réponse envoyée avec succès.");
                afficher(); // Rafraîchir la liste
                txtReponse.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'envoi de la réponse : " + ex.Message);
            }
        }
        private void btnNettoyer_Click(object sender, RoutedEventArgs e)
        {
            effacer();
        }


    }
}
