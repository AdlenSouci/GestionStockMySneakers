using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace GestionStockMySneakers.Pages
{
    public partial class Marques : Page
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string apiUrl = ConfigurationManager.AppSettings["api_url"] + "/marques";
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1); // Évite les appels simultanés
        private DispatcherTimer timer; // Timer pour rafraîchir automatiquement

        public Marques()
        {
            InitializeComponent();
            afficher(); // Chargement initial

            // Configuration du Timer (exécute `afficher` toutes les 10 secondes)
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10); // Rafraîchissement toutes les 10 secondes
            timer.Tick += (sender, e) => afficher();
            timer.Start();
        }

        private async void afficher()
        {
            if (!semaphore.Wait(0)) return; // Évite de lancer plusieurs requêtes en même temps
            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                // Gestion des erreurs 429 (trop de requêtes)
                if (response.StatusCode == (System.Net.HttpStatusCode)429)
                {
                    MessageBox.Show("Trop de requêtes ! Attente de 10 secondes...");
                    await Task.Delay(10000); // Attendre 10 secondes avant de réessayer
                    return;
                }

                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var marques = JsonConvert.DeserializeObject<List<Marque>>(responseBody);

                // Mise à jour de l'affichage
                dgMarques.ItemsSource = marques;
                lblMarques.Content = $"Marques ({marques.Count})";
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show("Erreur réseau : " + httpEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
            finally
            {
                semaphore.Release(); // Libère l'accès
            }
        }

        private void dgMarques_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Vérifie si une ligne est sélectionnée
            if (dgMarques.SelectedItem is Marque marqueSelectionnee)
            {
                MessageBox.Show($"Marque sélectionnée : {marqueSelectionnee.nom_marque}");
            }
        }
    }

    public class Marque
    {
        public int id { get; set; }
        public string nom_marque { get; set; }
    }
}
