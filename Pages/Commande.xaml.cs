using GestionStockMySneakers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Globalization;

namespace GestionStockMySneakers.Pages
{
    public partial class Commande : Page
    {
        private ObservableCollection<CommandeEntete> commandes = new ObservableCollection<CommandeEntete>();

        public Commande()
        {
            InitializeComponent();
            afficher();
        }

        private async void afficher()
        {
            pbLoading.Visibility = Visibility.Visible;
            dgCommandes.Visibility = Visibility.Collapsed;

            try
            {
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/commandes");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                commandes = JsonConvert.DeserializeObject<ObservableCollection<CommandeEntete>>(responseBody)
                            ?? new ObservableCollection<CommandeEntete>();

                dgCommandes.ItemsSource = commandes;
                lblCommandes.Content = $"Commandes ({commandes.Count})";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
            finally
            {
                pbLoading.Visibility = Visibility.Collapsed;
                dgCommandes.Visibility = Visibility.Visible;
            }
        }

        private async void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // S'assurer que les champs de saisie ne sont pas vides
                if (string.IsNullOrEmpty(txtIdUser.Text) || string.IsNullOrEmpty(txtTotalHT.Text) || string.IsNullOrEmpty(txtTotalTTC.Text))
                {
                    MessageBox.Show("Veuillez remplir tous les champs obligatoires.");
                    return;
                }

                int idNumCommande = (int)(DateTime.Now.Ticks % int.MaxValue);
                txtIdNumCommande.Text = idNumCommande.ToString(); // juste pour affichage si tu veux

                // Assigner les valeurs depuis les TextBox
                CommandeEntete nouvelleCommande = new CommandeEntete()
                {
                    id_user = int.Parse(txtIdUser.Text),
                    id_num_commande = idNumCommande,
                    total_ht = decimal.Parse(txtTotalHT.Text, CultureInfo.InvariantCulture),
                    total_ttc = decimal.Parse(txtTotalTTC.Text, CultureInfo.InvariantCulture),
                    total_tva = decimal.Parse(txtTotalTVA.Text, CultureInfo.InvariantCulture),
                    total_remise = decimal.Parse(txtTotalRemise.Text, CultureInfo.InvariantCulture),
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };

                // Convertir l'objet en JSON
                string jsonCommande = JsonConvert.SerializeObject(nouvelleCommande);
                StringContent content = new StringContent(jsonCommande, Encoding.UTF8, "application/json");

                // Envoi de la commande à l'API
                HttpResponseMessage response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/commandes", content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Commande ajoutée avec succès.");
                afficher(); // Rafraîchir la liste des commandes
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'ajout de la commande : " + ex.Message);
            }
        }

        private void dgCommandes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCommandes.SelectedItem is CommandeEntete selectedCommande)
            {
                // Remplir les champs de la commande sélectionnée
                txtIdUser.Text = selectedCommande.id_user.ToString();
                txtIdNumCommande.Text = selectedCommande.id_num_commande.ToString();
                txtTotalHT.Text = selectedCommande.total_ht.ToString("0.00");
                txtTotalTTC.Text = selectedCommande.total_ttc.ToString("0.00");
                txtTotalTVA.Text = selectedCommande.total_tva.ToString("0.00");
                txtTotalRemise.Text = selectedCommande.total_remise.ToString("0.00");
            }
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (dgCommandes.SelectedItem != null)
            {
                dgCommandes.SelectedItem = null; // Désélectionner la commande
            }
        }
    }
}
