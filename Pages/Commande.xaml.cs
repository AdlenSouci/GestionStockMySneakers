using GestionStockMySneakers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;

namespace GestionStockMySneakers.Pages
{
    public partial class Commande : Page
    {
        private ObservableCollection<CommandeEntete> commandes = new ObservableCollection<CommandeEntete>();
        private ObservableCollection<CommandeDetail> detailsTemporaires = new ObservableCollection<CommandeDetail>();

        public Commande()
        {
            InitializeComponent();
            dgCommandeDetails.ItemsSource = detailsTemporaires;
            btnSupprimerCommande.IsEnabled = false;
            afficher();
        }

        private async void afficher()
        {
            pbLoading.Visibility = Visibility.Visible;
            dgCommandes.Visibility = Visibility.Collapsed;
            btnAjouter.IsEnabled = false;
            btnSupprimerCommande.IsEnabled = false;
            btnEffacerFormulaire.IsEnabled = false;

            try
            {
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/commandes");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                commandes = JsonConvert.DeserializeObject<ObservableCollection<CommandeEntete>>(responseBody)
                            ?? new ObservableCollection<CommandeEntete>();

                var commandesTriees = commandes.OrderByDescending(c => c.id_commande).ToList();
                dgCommandes.ItemsSource = new ObservableCollection<CommandeEntete>(commandesTriees);

                lblCommandes.Content = $"Gestion des Commandes ({commandesTriees.Count})";
            }
            catch (HttpRequestException httpEx) { MessageBox.Show($"Erreur connexion commandes: {httpEx.Message}", "Erreur Réseau", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (JsonException jsonEx) { MessageBox.Show($"Erreur lecture données commandes: {jsonEx.Message}", "Erreur Données", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (Exception ex) { MessageBox.Show("Erreur inattendue affichage commandes: " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally
            {
                pbLoading.Visibility = Visibility.Collapsed;
                dgCommandes.Visibility = Visibility.Visible;
                btnAjouter.IsEnabled = true;
                btnEffacerFormulaire.IsEnabled = true;
                btnSupprimerCommande.IsEnabled = dgCommandes.SelectedItem != null;
            }
        }

        private void dgCommandes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCommandes.SelectedItem is CommandeEntete selectedCommande)
            {
                txtIdUser.Text = selectedCommande.id_user.ToString();
                txtName.Text = selectedCommande.name;
                txtIdNumCommande.Text = selectedCommande.id_num_commande.ToString();

                txtTotalHT.Text = selectedCommande.total_ht.ToString("N2", CultureInfo.InvariantCulture);
                txtTotalTTC.Text = selectedCommande.total_ttc.ToString("N2", CultureInfo.InvariantCulture);
                txtTotalTVA.Text = selectedCommande.total_tva.ToString("N2", CultureInfo.InvariantCulture);

                detailsTemporaires.Clear();
                ViderChampsDetail();

                btnSupprimerCommande.IsEnabled = true;
                btnAjouter.IsEnabled = false;
                btnEffacerFormulaire.IsEnabled = true;
                dgCommandes.IsEnabled = true;
            }
            else
            {
                ViderFormulaireComplet();
            }
        }

        private async void btnSupprimerCommande_Click(object sender, RoutedEventArgs e)
        {
            if (dgCommandes.SelectedItem is CommandeEntete selectedCommande)
            {
                MessageBoxResult confirmation = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer la commande N° {selectedCommande.id_num_commande} (ID interne: {selectedCommande.id_commande}) ?\nCette action est irréversible.",
                    "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirmation == MessageBoxResult.Yes)
                {
                    pbLoading.Visibility = Visibility.Visible;
                    btnSupprimerCommande.IsEnabled = false;
                    btnAjouter.IsEnabled = false;
                    btnEffacerFormulaire.IsEnabled = false;
                    dgCommandes.IsEnabled = false;

                    try
                    {
                        string url = $"{ApiClient.apiUrl}/commandes/{selectedCommande.id_commande}";
                        string token = Settings.Default.UserToken;

                        if (string.IsNullOrEmpty(token))
                            throw new Exception("Token non disponible. Veuillez vous reconnecter.");

                        ApiClient.Client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                        HttpResponseMessage response = await ApiClient.Client.DeleteAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Commande supprimée avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                            ViderFormulaireComplet();
                            afficher();
                        }
                        else
                        {
                            string errorContent = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"Erreur lors de la suppression côté serveur : {response.StatusCode}\n{errorContent}", "Erreur API", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (HttpRequestException httpEx) { MessageBox.Show("Erreur connexion API suppression: " + httpEx.Message, "Erreur Réseau"); }
                    catch (Exception ex) { MessageBox.Show("Erreur inattendue suppression: " + ex.Message, "Erreur"); }
                    finally
                    {
                        pbLoading.Visibility = Visibility.Collapsed;
                        btnSupprimerCommande.IsEnabled = true;
                        btnAjouter.IsEnabled = true;
                        btnEffacerFormulaire.IsEnabled = true;
                        dgCommandes.IsEnabled = true;
                    }
                }
            }
            else
            {
                MessageBox.Show("Sélectionnez une commande avant de supprimer.", "Sélection requise", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnAjouterDetail_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtDetailIdArticle.Text, out int idArticle) || idArticle <= 0)
            {
                MessageBox.Show("ID Article invalide.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDetailTaille.Text))
            {
                MessageBox.Show("Taille requise.");
                return;
            }

            if (!int.TryParse(txtDetailQuantite.Text, out int quantite) || quantite <= 0)
            {
                MessageBox.Show("Quantité invalide.");
                return;
            }

            if (!decimal.TryParse(txtDetailPrixHT.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal prixHT) || prixHT < 0 ||
                !decimal.TryParse(txtDetailMontantTVA.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal montantTVA) || montantTVA < 0)
            {
                MessageBox.Show("Valeurs numériques (Prix HT, Montant TVA) invalides ou négatives. Utilisez '.' pour les décimales.", "Erreur Format", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal prixTTC = prixHT + montantTVA;
            tbDetailPrixTTCCalcule.Text = prixTTC.ToString("N2", CultureInfo.InvariantCulture);

            CommandeDetail nouveauDetail = new CommandeDetail
            {
                id_article = idArticle,
                taille = txtDetailTaille.Text,
                quantite = quantite,
                prix_ht = prixHT,
                prix_ttc = prixTTC,
                montant_tva = montantTVA
            };

            detailsTemporaires.Add(nouveauDetail);
            CalculerEtAfficherTotaux();
            ViderChampsDetail();
        }

        private void btnSupprimerDetail_Click(object sender, RoutedEventArgs e)
        {
            if (dgCommandeDetails.SelectedItem is CommandeDetail selectedDetail)
            {
                detailsTemporaires.Remove(selectedDetail);
                CalculerEtAfficherTotaux();
            }
            else
            {
                MessageBox.Show("Sélectionnez un article dans la liste des détails à supprimer.", "Sélection Requise", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CalculerEtAfficherTotaux()
        {
            decimal totalHT = detailsTemporaires.Sum(d => d.prix_ht * d.quantite);
            decimal totalTTC = detailsTemporaires.Sum(d => d.prix_ttc * d.quantite);
            decimal totalTVA = detailsTemporaires.Sum(d => d.montant_tva * d.quantite);

            txtTotalHT.Text = totalHT.ToString("N2", CultureInfo.InvariantCulture);
            txtTotalTTC.Text = totalTTC.ToString("N2", CultureInfo.InvariantCulture);
            txtTotalTVA.Text = totalTVA.ToString("N2", CultureInfo.InvariantCulture);
        }

        private async void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            pbLoading.Visibility = Visibility.Visible;
            btnAjouter.IsEnabled = false;
            btnSupprimerCommande.IsEnabled = false;
            btnEffacerFormulaire.IsEnabled = false;
            dgCommandes.IsEnabled = false;

            try
            {
                if (!int.TryParse(txtIdUser.Text, out int idUser) || idUser <= 0)
                {
                    MessageBox.Show("ID Utilisateur invalide.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Nom de l'utilisateur requis.");
                    return;
                }

                if (detailsTemporaires.Count == 0)
                {
                    MessageBox.Show("Commande vide. Ajoutez des articles.");
                    return;
                }

                int idNumCommande = (int)(DateTime.Now.Ticks % int.MaxValue);
                string userName = txtName.Text;

                CommandeEntete nouvelleCommande = new CommandeEntete()
                {
                    id_user = idUser,
                    name = userName,
                    id_num_commande = idNumCommande,
                    total_ht = decimal.Parse(txtTotalHT.Text, CultureInfo.InvariantCulture),
                    total_ttc = decimal.Parse(txtTotalTTC.Text, CultureInfo.InvariantCulture),
                    total_tva = decimal.Parse(txtTotalTVA.Text, CultureInfo.InvariantCulture),
                    details = new List<CommandeDetail>(detailsTemporaires)
                };

                string jsonCommande = JsonConvert.SerializeObject(nouvelleCommande, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                StringContent content = new StringContent(jsonCommande, Encoding.UTF8, "application/json");

                string token = Settings.Default.UserToken;

                if (string.IsNullOrEmpty(token))
                    throw new Exception("Token non disponible. Veuillez vous reconnecter.");

                ApiClient.Client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/commandes", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Commande ajoutée avec succès !");
                    ViderFormulaireComplet();
                    afficher();
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erreur ajout serveur : {response.StatusCode}\n{errorContent}", "Erreur API");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur inattendue ajout: " + ex.Message, "Erreur");
            }
            finally
            {
                pbLoading.Visibility = Visibility.Collapsed;
                btnAjouter.IsEnabled = true;
                btnEffacerFormulaire.IsEnabled = true;
                dgCommandes.IsEnabled = true;
            }
        }

        private void ViderChampsDetail()
        {
            txtDetailIdArticle.Clear();
            txtDetailTaille.Clear();
            txtDetailQuantite.Clear();
            txtDetailPrixHT.Clear();
            txtDetailMontantTVA.Clear();
            tbDetailPrixTTCCalcule.Text = "--.--";
            txtDetailIdArticle.Focus();
        }

        private void ViderFormulaireComplet()
        {
            txtIdUser.Clear();
            txtName.Clear();
            txtIdNumCommande.Clear();

            txtTotalHT.Text = "0.00";
            txtTotalTTC.Text = "0.00";
            txtTotalTVA.Text = "0.00";

            detailsTemporaires.Clear();
            ViderChampsDetail();

            dgCommandes.SelectedItem = null;
            btnSupprimerCommande.IsEnabled = false;
            btnAjouter.IsEnabled = true;
            btnEffacerFormulaire.IsEnabled = true;
            dgCommandes.IsEnabled = true;

            txtIdUser.Focus();
        }

        private void btnEffacerFormulaire_Click(object sender, RoutedEventArgs e)
        {
            ViderFormulaireComplet();
            MessageBox.Show("Formulaire effacé.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is DependencyObject source && !(e.Source is Button))
            {
                var dataGrid = FindVisualParent<DataGrid>(source);
                if (dataGrid != dgCommandes && dgCommandes.SelectedItem != null)
                {
                    dgCommandes.SelectedItem = null;
                }
            }
        }

        private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = System.Windows.Media.VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            return parent ?? FindVisualParent<T>(parentObject);
        }
    }
}
