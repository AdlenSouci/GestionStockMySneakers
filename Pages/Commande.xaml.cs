using GestionStockMySneakers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // Nécessaire pour ObservableCollection
using System.Globalization;
using System.Linq; // Nécessaire pour Sum()
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace GestionStockMySneakers.Pages
{
    public partial class Commande : Page
    {
        private ObservableCollection<CommandeEntete> commandes = new ObservableCollection<CommandeEntete>();
        // Collection pour stocker les détails de la commande EN COURS DE CRÉATION
        private ObservableCollection<CommandeDetail> detailsTemporaires = new ObservableCollection<CommandeDetail>();

        public Commande()
        {
            InitializeComponent();
            // Lier le DataGrid des détails à la collection temporaire
            dgCommandeDetails.ItemsSource = detailsTemporaires;
            afficher(); // Charger les commandes existantes
        }

        // --- MÉTHODES POUR LA LISTE DES COMMANDES EXISTANTES ---

        private async void afficher()
        {
            // ... (ton code existant pour afficher dgCommandes est bon) ...
            pbLoading.Visibility = Visibility.Visible;
            dgCommandes.Visibility = Visibility.Collapsed;
            btnAjouter.IsEnabled = false; // Désactiver pendant le chargement

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
                MessageBox.Show("Erreur lors de l'affichage des commandes : " + ex.Message);
            }
            finally
            {
                pbLoading.Visibility = Visibility.Collapsed;
                dgCommandes.Visibility = Visibility.Visible;
                btnAjouter.IsEnabled = true; // Réactiver
            }
        }

        private void dgCommandes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Quand on sélectionne une commande existante, on affiche ses infos
            // Pour l'instant, on ne charge pas ses détails dans le formulaire d'ajout
            // On vide plutôt le formulaire d'ajout pour éviter la confusion
            if (dgCommandes.SelectedItem is CommandeEntete selectedCommande)
            {
                // Afficher l'en-tête sélectionné dans les champs (qui sont ReadOnly)
                txtIdUser.Text = selectedCommande.id_user.ToString();
                txtIdNumCommande.Text = selectedCommande.id_num_commande.ToString();
                txtTotalHT.Text = selectedCommande.total_ht.ToString("N2", CultureInfo.InvariantCulture);
                txtTotalTTC.Text = selectedCommande.total_ttc.ToString("N2", CultureInfo.InvariantCulture);
                txtTotalTVA.Text = selectedCommande.total_tva.ToString("N2", CultureInfo.InvariantCulture);
                txtTotalRemise.Text = selectedCommande.total_remise.ToString("N2", CultureInfo.InvariantCulture);

                // Vider la section des détails de la commande en cours
                detailsTemporaires.Clear();
                ViderChampsDetail(); // Vider les champs de saisie de détail
                CalculerEtAfficherTotaux(); // Recalculer les totaux (qui seront à 0)

                // Désactiver l'ajout de détails si on regarde une ancienne commande? Ou permettre de copier? Pour l'instant on bloque pas.
            }
        }

        // --- MÉTHODES POUR LE FORMULAIRE D'AJOUT ---

        // Bouton pour ajouter UN ARTICLE à la liste temporaire
        private void btnAjouterDetail_Click(object sender, RoutedEventArgs e)
        {
            // 1. Lire et Valider les entrées pour le détail
            if (!int.TryParse(txtDetailIdArticle.Text, out int idArticle) || idArticle <= 0)
            {
                MessageBox.Show("Veuillez entrer un ID Article valide (nombre entier positif).");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtDetailTaille.Text))
            {
                MessageBox.Show("Veuillez entrer une Taille.");
                return;
            }
            if (!int.TryParse(txtDetailQuantite.Text, out int quantite) || quantite <= 0)
            {
                MessageBox.Show("Veuillez entrer une Quantité valide (nombre entier positif).");
                return;
            }
            // Valider les champs numériques (prix, tva, remise)
            if (!decimal.TryParse(txtDetailPrixHT.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal prixHT) || prixHT < 0 ||
                !decimal.TryParse(txtDetailPrixTTC.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal prixTTC) || prixTTC < 0 ||
                !decimal.TryParse(txtDetailMontantTVA.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal montantTVA) || montantTVA < 0 ||
                !decimal.TryParse(txtDetailRemise.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal remise) || remise < 0)
            {
                MessageBox.Show("Veuillez entrer des valeurs numériques valides et positives pour les prix, TVA et remise. Utilisez '.' comme séparateur décimal.");
                return;
            }

            // 2. Créer l'objet CommandeDetail
            CommandeDetail nouveauDetail = new CommandeDetail
            {
                id_article = idArticle,
                taille = txtDetailTaille.Text,
                quantite = quantite,
                prix_ht = prixHT,
                prix_ttc = prixTTC,
                montant_tva = montantTVA,
                remise = remise
                // id, id_commande, created_at, updated_at ne sont pas définis ici
            };

            // 3. Ajouter à la collection (et donc au DataGrid dgCommandeDetails)
            detailsTemporaires.Add(nouveauDetail);

            // 4. Recalculer et afficher les totaux globaux
            CalculerEtAfficherTotaux();

            // 5. Vider les champs de saisie du détail pour le prochain article
            ViderChampsDetail();
        }

        // Bouton pour supprimer l'article sélectionné dans dgCommandeDetails
        private void btnSupprimerDetail_Click(object sender, RoutedEventArgs e)
        {
            if (dgCommandeDetails.SelectedItem is CommandeDetail selectedDetail)
            {
                detailsTemporaires.Remove(selectedDetail);
                // Recalculer les totaux après suppression
                CalculerEtAfficherTotaux();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un article dans la liste des détails à supprimer.");
            }
        }

        // Calculer les totaux à partir de la liste detailsTemporaires et les afficher
        private void CalculerEtAfficherTotaux()
        {
            decimal totalHT = detailsTemporaires.Sum(d => d.prix_ht * d.quantite);
            decimal totalTTC = detailsTemporaires.Sum(d => d.prix_ttc * d.quantite);
            // Attention: la TVA et la Remise dépendent de si les champs détail sont unitaires ou totaux ligne
            // Ici on suppose qu'ils sont unitaires (comme les prix)
            decimal totalTVA = detailsTemporaires.Sum(d => d.montant_tva * d.quantite);
            decimal totalRemise = detailsTemporaires.Sum(d => d.remise * d.quantite);

            // Afficher dans les TextBox (ReadOnly) de l'en-tête
            txtTotalHT.Text = totalHT.ToString("N2", CultureInfo.InvariantCulture);
            txtTotalTTC.Text = totalTTC.ToString("N2", CultureInfo.InvariantCulture);
            txtTotalTVA.Text = totalTVA.ToString("N2", CultureInfo.InvariantCulture);
            txtTotalRemise.Text = totalRemise.ToString("N2", CultureInfo.InvariantCulture);
        }


        // --- BOUTON PRINCIPAL POUR AJOUTER LA COMMANDE ---
        private async void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            pbLoading.Visibility = Visibility.Visible;
            btnAjouter.IsEnabled = false;

            try
            {
                // 1. Valider l'en-tête (juste l'ID utilisateur pour l'instant)
                if (!int.TryParse(txtIdUser.Text, out int idUser) || idUser <= 0)
                {
                    MessageBox.Show("Veuillez entrer un ID Utilisateur valide.");
                    return; // Sortie anticipée
                }

                // 2. Valider qu'il y a des détails !
                if (detailsTemporaires.Count == 0)
                {
                    MessageBox.Show("Impossible d'ajouter une commande vide. Veuillez ajouter au moins un article.");
                    return; // Sortie anticipée
                }

                // 3. Générer le numéro de commande (ou laisser l'API le faire)
                int idNumCommande = (int)(DateTime.Now.Ticks % int.MaxValue);
                // txtIdNumCommande.Text = idNumCommande.ToString(); // Affiché automatiquement par le binding si besoin

                // 4. Créer l'objet CommandeEntete COMPLET
                // Utiliser les totaux calculés directement
                CommandeEntete nouvelleCommande = new CommandeEntete()
                {
                    id_user = idUser,
                    id_num_commande = idNumCommande,
                    total_ht = decimal.Parse(txtTotalHT.Text, CultureInfo.InvariantCulture), // Lire depuis le champ calculé
                    total_ttc = decimal.Parse(txtTotalTTC.Text, CultureInfo.InvariantCulture),
                    total_tva = decimal.Parse(txtTotalTVA.Text, CultureInfo.InvariantCulture),
                    total_remise = decimal.Parse(txtTotalRemise.Text, CultureInfo.InvariantCulture),

                    // Assigner la VRAIE liste des détails
                    details = new List<CommandeDetail>(detailsTemporaires) // Convertir ObservableCollection en List pour l'envoi
                };

                // (Optionnel mais recommandé) Ajouter les [JsonIgnore] sur id, created_at, updated_at
                // dans les modèles CommandeEntete et CommandeDetail pour ne pas les envoyer.

                // 5. Sérialiser et Envoyer
                string jsonCommande = JsonConvert.SerializeObject(nouvelleCommande, Formatting.Indented);
                System.Diagnostics.Debug.WriteLine("--- DÉBUT APPEL API ---");
                System.Diagnostics.Debug.WriteLine("JSON envoyé : \n" + jsonCommande);
                StringContent content = new StringContent(jsonCommande, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/commandes", content);

                System.Diagnostics.Debug.WriteLine($"--- FIN APPEL API --- Statut: {response.StatusCode}");

                // 6. Gérer la réponse
                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("API a répondu Succès.");
                    MessageBox.Show("Commande ajoutée avec succès !");
                    ViderFormulaireComplet(); // Vider tout le formulaire
                    afficher(); // Rafraîchir la liste des commandes existantes
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"API a répondu Erreur. Contenu: {errorContent}");
                    MessageBox.Show($"Erreur lors de l'ajout : {response.StatusCode}\n{errorContent}");
                }
            }
            catch (FormatException formatEx) // Erreur de parsing avant l'API
            {
                System.Diagnostics.Debug.WriteLine($"ERREUR FormatException: {formatEx.ToString()}");
                MessageBox.Show("Erreur de format dans les champs numériques : " + formatEx.Message);
            }
            catch (HttpRequestException httpEx) // Erreur de connexion/réseau
            {
                System.Diagnostics.Debug.WriteLine($"ERREUR HttpRequestException: {httpEx.ToString()}");
                MessageBox.Show("Erreur de connexion à l'API : " + httpEx.Message + "\nVérifiez l'URL, la connexion internet et les logs serveur.");
            }
            catch (Exception ex) // Autres erreurs
            {
                System.Diagnostics.Debug.WriteLine($"ERREUR INATTENDUE: {ex.ToString()}");
                MessageBox.Show("Une erreur inattendue est survenue : " + ex.Message);
            }
            finally
            {
                pbLoading.Visibility = Visibility.Collapsed;
                btnAjouter.IsEnabled = true;
            }
        }


        // --- MÉTHODES UTILITAIRES POUR VIDER LES CHAMPS ---

        // Vider les champs de saisie pour UN détail
        private void ViderChampsDetail()
        {
            txtDetailIdArticle.Clear();
            txtDetailTaille.Clear();
            txtDetailQuantite.Clear();
            txtDetailPrixHT.Clear();
            txtDetailPrixTTC.Clear();
            txtDetailMontantTVA.Clear();
            txtDetailRemise.Clear();
        }

        // Vider TOUT le formulaire (en-tête + détails temporaires)
        private void ViderFormulaireComplet()
        {
            txtIdUser.Clear();
            txtIdNumCommande.Clear();
            txtTotalHT.Clear();
            txtTotalTTC.Clear();
            txtTotalTVA.Clear();
            txtTotalRemise.Clear();

            detailsTemporaires.Clear(); // Vider la liste des détails en cours
            ViderChampsDetail(); // Vider aussi les champs de saisie de détail
                                 // Les totaux seront recalculés à 0 par CalculerEtAfficherTotaux si appelé, ou simplement vides.
        }

        // Bouton pour effacer manuellement le formulaire
        private void btnEffacerFormulaire_Click(object sender, RoutedEventArgs e)
        {
            ViderFormulaireComplet();
            dgCommandes.SelectedItem = null; // Désélectionner aussi la commande existante
            MessageBox.Show("Formulaire effacé.");
        }


        // --- GESTION DE LA DÉSÉLECTION ---
        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Si on clique en dehors du DataGrid des commandes existantes
            if (e.OriginalSource is DependencyObject source)
            {
                var dataGrid = FindVisualParent<DataGrid>(source);
                // Si on clique en dehors de dgCommandes ET qu'un item était sélectionné
                if (dataGrid != dgCommandes && dgCommandes.SelectedItem != null)
                {
                    dgCommandes.SelectedItem = null; // Désélectionner la commande existante
                                                     // ViderFormulaireComplet(); // Optionnel: vider aussi le formulaire quand on désélectionne
                }
            }
        }

        // Helper pour trouver un parent visuel (gardé de l'exemple précédent)
        private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = System.Windows.Media.VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindVisualParent<T>(parentObject);
        }
    }
}
