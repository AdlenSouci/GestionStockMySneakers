using GestionStockMySneakers.Models; // Assure-toi que ce using est correct ET que CommandeEntete a id_commande (potentiellement avec [JsonProperty("id")])
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
using System.Diagnostics; // Pour Debug.WriteLine

namespace GestionStockMySneakers.Pages
{
    public partial class Commande : Page
    {
        // Collections pour les données
        private ObservableCollection<CommandeEntete> commandes = new ObservableCollection<CommandeEntete>();
        private ObservableCollection<CommandeDetail> detailsTemporaires = new ObservableCollection<CommandeDetail>();

        // Constructeur
        public Commande()
        {
            InitializeComponent();
            dgCommandeDetails.ItemsSource = detailsTemporaires; // Lier la grille des détails temporaires
            btnSupprimerCommande.IsEnabled = false; // Désactiver le bouton supprimer au démarrage
            afficher(); // Charger les commandes initiales
        }

        // --- CHARGEMENT INITIAL DES COMMANDES ---
        private async void afficher()
        {
            // Gérer l'état de l'interface pendant le chargement
            pbLoading.Visibility = Visibility.Visible;
            dgCommandes.Visibility = Visibility.Collapsed;
            btnAjouter.IsEnabled = false;
            btnSupprimerCommande.IsEnabled = false;
            btnEffacerFormulaire.IsEnabled = false;

            try
            {
                Debug.WriteLine("--- Début Affichage Commandes ---");
                // Appel API pour récupérer les commandes
                HttpResponseMessage response = await ApiClient.Client.GetAsync(ApiClient.apiUrl + "/commandes");
                response.EnsureSuccessStatusCode(); // Vérifie si la requête a réussi (status 2xx)
                string responseBody = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Réponse API GET /commandes: {response.StatusCode}");

                // Désérialiser la réponse JSON en objets C#
                // Important: Le modèle CommandeEntete DOIT pouvoir mapper l'ID (via nom ou JsonProperty)
                commandes = JsonConvert.DeserializeObject<ObservableCollection<CommandeEntete>>(responseBody)
                            ?? new ObservableCollection<CommandeEntete>(); // Si null, créer une collection vide

                // Trier par ID (id_commande) décroissant pour voir les plus récentes
                var commandesTriees = commandes.OrderByDescending(c => c.id_commande).ToList();
                // Mettre à jour la source de données de la grille principale
                dgCommandes.ItemsSource = new ObservableCollection<CommandeEntete>(commandesTriees);

                // Mettre à jour le titre avec le nombre de commandes
                lblCommandes.Content = $"Gestion des Commandes ({commandesTriees.Count})";
                Debug.WriteLine($"Affichage terminé. {commandesTriees.Count} commandes chargées.");
            }
            // Gestion des différentes erreurs possibles
            catch (HttpRequestException httpEx) { Debug.WriteLine($"ERREUR HTTP Affichage: {httpEx.Message}"); MessageBox.Show($"Erreur connexion commandes: {httpEx.Message}", "Erreur Réseau", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (JsonException jsonEx) { Debug.WriteLine($"ERREUR JSON Affichage: {jsonEx.Message}"); MessageBox.Show($"Erreur lecture données commandes: {jsonEx.Message}", "Erreur Données", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (Exception ex) { Debug.WriteLine($"ERREUR Inattendue Affichage: {ex.ToString()}"); MessageBox.Show("Erreur inattendue affichage commandes: " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally
            {
                // Assurer que l'interface est réactivée même en cas d'erreur
                pbLoading.Visibility = Visibility.Collapsed;
                dgCommandes.Visibility = Visibility.Visible;
                btnAjouter.IsEnabled = true;
                btnEffacerFormulaire.IsEnabled = true;
                // Activer Supprimer seulement si un item est sélectionné
                btnSupprimerCommande.IsEnabled = dgCommandes.SelectedItem != null;
                Debug.WriteLine("--- Fin Affichage Commandes (finally) ---");
            }
        }

        // --- GESTION DE LA SÉLECTION DANS LA GRILLE PRINCIPALE ---
        private void dgCommandes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Vérifier si un élément est sélectionné
            if (dgCommandes.SelectedItem is CommandeEntete selectedCommande)
            {
                // Afficher les détails de la commande sélectionnée dans le formulaire
                Debug.WriteLine($"Sélection changée: Commande ID {selectedCommande.id_commande} sélectionnée.");
                txtIdUser.Text = selectedCommande.id_user.ToString();
                txtIdNumCommande.Text = selectedCommande.id_num_commande.ToString();
                txtTotalHT.Text = selectedCommande.total_ht.ToString("N2", CultureInfo.InvariantCulture);
                txtTotalTTC.Text = selectedCommande.total_ttc.ToString("N2", CultureInfo.InvariantCulture);
                txtTotalTVA.Text = selectedCommande.total_tva.ToString("N2", CultureInfo.InvariantCulture);
                txtTotalRemise.Text = selectedCommande.total_remise.ToString("N2", CultureInfo.InvariantCulture);

                // Vider la liste des détails temporaires (pour nouvelle saisie ou affichage clair)
                detailsTemporaires.Clear();
                ViderChampsDetail();
                CalculerEtAfficherTotaux(); // Remet les totaux du formulaire à 0

                // Activer/Désactiver les boutons pertinents
                btnSupprimerCommande.IsEnabled = true; // On peut supprimer la commande sélectionnée
                btnAjouter.IsEnabled = false; // On ne crée pas pendant qu'on regarde/supprime une existante
                btnEffacerFormulaire.IsEnabled = true; // On peut toujours effacer le formulaire
            }
            else // Aucun élément sélectionné
            {
                Debug.WriteLine("Sélection changée: Aucune commande sélectionnée.");
                // Réinitialiser l'état des boutons
                btnSupprimerCommande.IsEnabled = false; // Pas de commande à supprimer
                btnAjouter.IsEnabled = true; // Prêt à créer une nouvelle commande
                // Optionnel: Vider le formulaire si on désélectionne
                // ViderFormulaireComplet();
            }
        }

        // --- SUPPRESSION D'UNE COMMANDE ---
        private async void btnSupprimerCommande_Click(object sender, RoutedEventArgs e)
        {
            // Vérifier qu'une commande est bien sélectionnée
            if (dgCommandes.SelectedItem is CommandeEntete selectedCommande)
            {
                Debug.WriteLine($"Clic sur Supprimer pour commande ID: {selectedCommande.id_commande}");
                // Demander confirmation
                MessageBoxResult confirmation = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer la commande N° {selectedCommande.id_num_commande} (ID interne: {selectedCommande.id_commande}) ?\nCette action est irréversible.",
                    "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirmation == MessageBoxResult.Yes)
                {
                    Debug.WriteLine("Confirmation OUI pour la suppression.");
                    // Gérer l'état de l'interface pendant la suppression
                    pbLoading.Visibility = Visibility.Visible;
                    btnSupprimerCommande.IsEnabled = false;
                    btnAjouter.IsEnabled = false;
                    btnEffacerFormulaire.IsEnabled = false;
                    dgCommandes.IsEnabled = false; // Empêcher la sélection pendant l'appel

                    try
                    {
                        // Construire l'URL de l'API DELETE avec l'ID correct (id_commande)
                        string url = $"{ApiClient.apiUrl}/commandes/{selectedCommande.id_commande}";
                        Debug.WriteLine($"--- APPEL API DELETE : {url} ---");
                        // Envoyer la requête DELETE
                        HttpResponseMessage response = await ApiClient.Client.DeleteAsync(url);
                        Debug.WriteLine($"--- FIN APPEL API DELETE --- Statut: {response.StatusCode}");

                        // Gérer la réponse de l'API
                        if (response.IsSuccessStatusCode) // Status 2xx (ex: 200 OK, 204 No Content)
                        {
                            Debug.WriteLine($"Suppression réussie (API). Message: {await response.Content.ReadAsStringAsync()}");
                            MessageBox.Show("Commande supprimée avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                            // Réinitialiser l'interface après succès
                            ViderFormulaireComplet(); // Vide le formulaire et désélectionne
                            afficher(); // Recharge la liste mise à jour
                        }
                        else // Erreur retournée par l'API (ex: 404 Not Found, 500 Server Error)
                        {
                            string errorContent = await response.Content.ReadAsStringAsync();
                            Debug.WriteLine($"Échec suppression (API). Statut: {response.StatusCode}, Contenu: {errorContent}");
                            // Afficher l'erreur à l'utilisateur
                            MessageBox.Show($"Erreur lors de la suppression côté serveur : {response.StatusCode}\n{errorContent}", "Erreur API", MessageBoxButton.OK, MessageBoxImage.Error);
                            // Réactiver l'interface pour permettre une autre action (ex: réessayer, effacer)
                            btnSupprimerCommande.IsEnabled = true; // Réactiver car l'item est toujours sélectionné
                            btnAjouter.IsEnabled = false; // Toujours désactivé si item sélectionné
                            btnEffacerFormulaire.IsEnabled = true;
                            dgCommandes.IsEnabled = true;
                        }
                    }
                    // Gestion des erreurs de connexion ou autres erreurs C#
                    catch (HttpRequestException httpEx) { Debug.WriteLine($"ERREUR HttpRequestException Suppression: {httpEx.ToString()}"); MessageBox.Show("Erreur connexion API suppression: " + httpEx.Message, "Erreur Réseau"); btnSupprimerCommande.IsEnabled = true; btnAjouter.IsEnabled = false; btnEffacerFormulaire.IsEnabled = true; dgCommandes.IsEnabled = true; }
                    catch (Exception ex) { Debug.WriteLine($"ERREUR Inattendue Suppression: {ex.ToString()}"); MessageBox.Show("Erreur inattendue suppression: " + ex.Message, "Erreur"); btnSupprimerCommande.IsEnabled = true; btnAjouter.IsEnabled = false; btnEffacerFormulaire.IsEnabled = true; dgCommandes.IsEnabled = true; }
                    finally
                    {
                        // Assurer que la barre de progression est cachée
                        pbLoading.Visibility = Visibility.Collapsed;
                        Debug.WriteLine("--- Fin Suppression Commande (finally) ---");
                    }
                }
                else { Debug.WriteLine("Confirmation NON pour la suppression."); } // L'utilisateur a cliqué Non
            }
            else { Debug.WriteLine("Clic sur Supprimer mais aucune commande sélectionnée."); MessageBox.Show("Sélectionnez une commande avant de supprimer.", "Sélection requise", MessageBoxButton.OK, MessageBoxImage.Warning); } // Sécurité si le bouton était mal activé
        }


        // --- AJOUT D'UN ARTICLE À LA LISTE TEMPORAIRE ---
        private void btnAjouterDetail_Click(object sender, RoutedEventArgs e)
        {
            // 1. Valider les entrées utilisateur pour le détail
            if (!int.TryParse(txtDetailIdArticle.Text, out int idArticle) || idArticle <= 0) { MessageBox.Show("ID Article invalide."); return; }
            if (string.IsNullOrWhiteSpace(txtDetailTaille.Text)) { MessageBox.Show("Taille requise."); return; }
            if (!int.TryParse(txtDetailQuantite.Text, out int quantite) || quantite <= 0) { MessageBox.Show("Quantité invalide."); return; }
            // Valider les champs numériques (HT, TVA, Remise) - TTC n'est plus saisi
            if (!decimal.TryParse(txtDetailPrixHT.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal prixHT) || prixHT < 0 ||
                !decimal.TryParse(txtDetailMontantTVA.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal montantTVA) || montantTVA < 0 ||
                !decimal.TryParse(txtDetailRemise.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal remise) || remise < 0)
            {
                MessageBox.Show("Valeurs numériques (Prix HT, Montant TVA, Remise) invalides ou négatives. Utilisez '.' pour les décimales.", "Erreur Format", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Calculer le Prix TTC à partir de HT et TVA saisis
            decimal prixTTC = prixHT + montantTVA;

            // 3. Créer un nouvel objet CommandeDetail avec les données validées et calculées
            CommandeDetail nouveauDetail = new CommandeDetail
            {
                id_article = idArticle,
                taille = txtDetailTaille.Text,
                quantite = quantite,
                prix_ht = prixHT,
                prix_ttc = prixTTC, // Utilisation du TTC calculé
                montant_tva = montantTVA,
                remise = remise
            };

            // 4. Ajouter le détail à la liste temporaire (qui est liée à dgCommandeDetails)
            detailsTemporaires.Add(nouveauDetail);
            Debug.WriteLine($"Article ajouté (localement): ID {idArticle}, Qte {quantite}, HT={prixHT:N2}, TVA={montantTVA:N2}, TTC(calc)={prixTTC:N2}");

            // 5. Recalculer les totaux globaux de la commande en cours
            CalculerEtAfficherTotaux();

            // 6. Vider les champs de saisie pour préparer l'ajout suivant
            ViderChampsDetail();
        }

        // --- SUPPRESSION D'UN ARTICLE DE LA LISTE TEMPORAIRE ---
        private void btnSupprimerDetail_Click(object sender, RoutedEventArgs e)
        {
            // Vérifier si un détail est sélectionné dans la grille des détails temporaires
            if (dgCommandeDetails.SelectedItem is CommandeDetail selectedDetail)
            {
                // Supprimer l'élément de la collection ObservableCollection
                detailsTemporaires.Remove(selectedDetail);
                Debug.WriteLine($"Article supprimé (localement): ID {selectedDetail.id_article}");
                // Recalculer les totaux après suppression
                CalculerEtAfficherTotaux();
            }
            else { MessageBox.Show("Sélectionnez un article dans la liste des détails à supprimer.", "Sélection Requise", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        // --- CALCUL ET AFFICHAGE DES TOTAUX DE LA COMMANDE EN COURS ---
        private void CalculerEtAfficherTotaux()
        {
            // Sommer les valeurs des articles dans la liste temporaire
            decimal totalHT = detailsTemporaires.Sum(d => d.prix_ht * d.quantite);
            decimal totalTTC = detailsTemporaires.Sum(d => d.prix_ttc * d.quantite); // Somme les TTC (maintenant cohérents car calculés)
            decimal totalTVA = detailsTemporaires.Sum(d => d.montant_tva * d.quantite);
            decimal totalRemise = detailsTemporaires.Sum(d => d.remise * d.quantite);

            // Mettre à jour les champs d'affichage de l'en-tête (en lecture seule)
            txtTotalHT.Text = totalHT.ToString("N2", CultureInfo.InvariantCulture);
            txtTotalTTC.Text = totalTTC.ToString("N2", CultureInfo.InvariantCulture);
            txtTotalTVA.Text = totalTVA.ToString("N2", CultureInfo.InvariantCulture);
            txtTotalRemise.Text = totalRemise.ToString("N2", CultureInfo.InvariantCulture);
            Debug.WriteLine($"Totaux recalculés: HT={totalHT:N2}, TTC={totalTTC:N2}, TVA={totalTVA:N2}");
        }

        // --- AJOUT DE LA COMMANDE COMPLÈTE (EN-TÊTE + DÉTAILS) ---
        private async void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Clic sur Créer Commande.");
            // Gérer l'état de l'interface pendant l'ajout
            pbLoading.Visibility = Visibility.Visible;
            btnAjouter.IsEnabled = false;
            btnSupprimerCommande.IsEnabled = false;
            btnEffacerFormulaire.IsEnabled = false;
            dgCommandes.IsEnabled = false;

            try
            {
                // Valider les informations minimales (ID utilisateur et présence de détails)
                if (!int.TryParse(txtIdUser.Text, out int idUser) || idUser <= 0)
                {
                    MessageBox.Show("ID Utilisateur invalide.");
                    // Réactiver l'interface en cas d'erreur de validation
                    pbLoading.Visibility = Visibility.Collapsed; btnAjouter.IsEnabled = true; btnEffacerFormulaire.IsEnabled = true; dgCommandes.IsEnabled = true; return;
                }
                if (detailsTemporaires.Count == 0)
                {
                    MessageBox.Show("Commande vide. Ajoutez des articles.");
                    // Réactiver l'interface en cas d'erreur de validation
                    pbLoading.Visibility = Visibility.Collapsed; btnAjouter.IsEnabled = true; btnEffacerFormulaire.IsEnabled = true; dgCommandes.IsEnabled = true; return;
                }

                // Générer un numéro de commande (si non géré par l'API)
                int idNumCommande = (int)(DateTime.Now.Ticks % int.MaxValue);

                // Créer l'objet CommandeEntete complet avec les totaux calculés et les détails
                CommandeEntete nouvelleCommande = new CommandeEntete()
                {
                    // id_commande n'est pas défini ici, il sera généré par la BDD lors de l'insertion
                    id_user = idUser,
                    id_num_commande = idNumCommande,
                    total_ht = decimal.Parse(txtTotalHT.Text, CultureInfo.InvariantCulture), // Lire les totaux affichés
                    total_ttc = decimal.Parse(txtTotalTTC.Text, CultureInfo.InvariantCulture),
                    total_tva = decimal.Parse(txtTotalTVA.Text, CultureInfo.InvariantCulture),
                    total_remise = decimal.Parse(txtTotalRemise.Text, CultureInfo.InvariantCulture),
                    details = new List<CommandeDetail>(detailsTemporaires) // Convertir l'ObservableCollection en List pour l'envoi
                };

                // Sérialiser l'objet en JSON
                string jsonCommande = JsonConvert.SerializeObject(nouvelleCommande, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Debug.WriteLine("--- DÉBUT APPEL API POST /commandes ---\n" + jsonCommande);
                StringContent content = new StringContent(jsonCommande, Encoding.UTF8, "application/json");

                // Envoyer la requête POST à l'API
                HttpResponseMessage response = await ApiClient.Client.PostAsync(ApiClient.apiUrl + "/commandes", content);
                Debug.WriteLine($"--- FIN APPEL API POST --- Statut: {response.StatusCode}");

                // Gérer la réponse de l'API
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Ajout réussi (API). Réponse: {await response.Content.ReadAsStringAsync()}");
                    MessageBox.Show("Commande ajoutée avec succès !");
                    // Réinitialiser l'interface après succès
                    ViderFormulaireComplet();
                    afficher(); // Recharger la liste
                }
                else // Erreur retournée par l'API
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Échec ajout (API). Statut: {response.StatusCode}, Contenu: {errorContent}");
                    MessageBox.Show($"Erreur ajout serveur : {response.StatusCode}\n{errorContent}", "Erreur API");
                    // Réactiver l'interface pour permettre correction/nouvelle tentative
                    btnAjouter.IsEnabled = true;
                    btnEffacerFormulaire.IsEnabled = true;
                    dgCommandes.IsEnabled = true;
                }
            }
            // Gestion des erreurs C# (format, connexion, etc.)
            catch (FormatException formatEx) { Debug.WriteLine($"ERREUR FormatException Ajout: {formatEx.ToString()}"); MessageBox.Show("Erreur format totaux: " + formatEx.Message); btnAjouter.IsEnabled = true; btnEffacerFormulaire.IsEnabled = true; dgCommandes.IsEnabled = true; }
            catch (HttpRequestException httpEx) { Debug.WriteLine($"ERREUR HttpRequestException Ajout: {httpEx.ToString()}"); MessageBox.Show("Erreur connexion API ajout: " + httpEx.Message, "Erreur Réseau"); btnAjouter.IsEnabled = true; btnEffacerFormulaire.IsEnabled = true; dgCommandes.IsEnabled = true; }
            catch (Exception ex) { Debug.WriteLine($"ERREUR Inattendue Ajout: {ex.ToString()}"); MessageBox.Show("Erreur inattendue ajout: " + ex.Message, "Erreur"); btnAjouter.IsEnabled = true; btnEffacerFormulaire.IsEnabled = true; dgCommandes.IsEnabled = true; }
            finally
            {
                // Assurer que la barre de progression est cachée
                pbLoading.Visibility = Visibility.Collapsed;
                Debug.WriteLine("--- Fin Ajout Commande (finally) ---");
            }
        }


        // --- MÉTHODES UTILITAIRES ---

        // Vider les champs de saisie pour UN détail d'article
        private void ViderChampsDetail()
        {
            txtDetailIdArticle.Clear();
            txtDetailTaille.Clear();
            txtDetailQuantite.Clear();
            txtDetailPrixHT.Clear();
            txtDetailMontantTVA.Clear();
            txtDetailRemise.Clear();
            // tbDetailPrixTTCCalcule.Text = "--.--"; // Réinitialiser si vous affichez le TTC calculé
            txtDetailIdArticle.Focus(); // Mettre le focus sur le premier champ
        }

        // Vider TOUT le formulaire (en-tête + détails temporaires) ET réinitialiser les états
        private void ViderFormulaireComplet()
        {
            Debug.WriteLine("Vidage complet du formulaire.");
            // Vider champs en-tête
            txtIdUser.Clear();
            txtIdNumCommande.Clear();
            txtTotalHT.Clear();
            txtTotalTTC.Clear();
            txtTotalTVA.Clear();
            txtTotalRemise.Clear();

            // Vider la liste temporaire et les champs de saisie de détail
            detailsTemporaires.Clear();
            ViderChampsDetail();

            // Réinitialiser la sélection de la grille et l'état des boutons
            dgCommandes.SelectedItem = null; // Important: déclenche SelectionChanged qui gère les états
            btnSupprimerCommande.IsEnabled = false; // Assurer par sécurité
            btnAjouter.IsEnabled = true;
            btnEffacerFormulaire.IsEnabled = true;
            dgCommandes.IsEnabled = true;

            txtIdUser.Focus(); // Prêt pour une nouvelle saisie
        }

        // Bouton pour effacer manuellement le formulaire
        private void btnEffacerFormulaire_Click(object sender, RoutedEventArgs e)
        {
            ViderFormulaireComplet(); // Appelle la méthode de vidage complet
            MessageBox.Show("Formulaire effacé.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        // --- GESTION DE LA DÉSÉLECTION PAR CLIC EXTÉRIEUR ---
        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Si on clique en dehors de la grille et qu'un item est sélectionné
            if (e.OriginalSource is DependencyObject source && !(e.Source is Button)) // Ignorer les clics sur les boutons
            {
                var dataGrid = FindVisualParent<DataGrid>(source);
                if (dataGrid != dgCommandes && dgCommandes.SelectedItem != null)
                {
                    dgCommandes.SelectedItem = null; // Désélectionner
                    Debug.WriteLine("Clic en dehors de la grille -> Désélection.");
                    // L'événement SelectionChanged gérera la mise à jour de l'état des boutons
                    // ViderFormulaireComplet(); // Décommentez si vous voulez aussi vider le formulaire
                }
            }
        }

        // Helper récursif pour trouver un parent visuel d'un type donné
        private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = System.Windows.Media.VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null; // Pas de parent
            T parent = parentObject as T; // Essayer de caster le parent
            return parent ?? FindVisualParent<T>(parentObject); // Retourner si trouvé, sinon chercher plus haut
        }
    }
}
