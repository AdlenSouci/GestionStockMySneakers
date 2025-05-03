using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace GestionStockMySneakers.Pages
{
    public partial class Parametres : Page
    {
        private string configPath = @"C:\CSharp\GestionStockMySneakers - Copie (2)\App.config"; // Chemin correct

        public Parametres()
        {
            InitializeComponent();
            LoadCurrentUrl(); // Charge l'URL actuelle au démarrage
        }

        private void ToggleCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateConfig("https://my-sneakers-shop.fr/api");
        }

        private void ToggleCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateConfig("http://127.0.0.1:8000/api");
        }

        private void UpdateConfig(string newUrl)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                if (!File.Exists(configPath))
                {
                    MessageBox.Show("⚠ Le fichier App.config est introuvable ! Vérifie son emplacement.");
                    return;
                }

                xmlDoc.Load(configPath);
                XmlNode node = xmlDoc.SelectSingleNode("//configuration/appSettings/add[@key='api_url']");

                if (node != null)
                {
                    node.Attributes["value"].Value = newUrl;
                    xmlDoc.Save(configPath);

                    urlTextBlock.Text = "URL actuelle : " + newUrl; // Met à jour l'affichage
                    MessageBox.Show("✅ Mode changé avec succès : " + newUrl);
                }
                else
                {
                    MessageBox.Show("⚠ Clé 'api_url' introuvable dans App.config !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Erreur lors de la modification du fichier : " + ex.Message);
            }
        }

        private void LoadCurrentUrl()
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configPath);
                XmlNode node = xmlDoc.SelectSingleNode("//configuration/appSettings/add[@key='api_url']");

                if (node != null)
                {
                    string currentUrl = node.Attributes["value"].Value;
                    urlTextBlock.Text = "URL actuelle : " + currentUrl;
                    toggleCheckBox.IsChecked = currentUrl.Contains("my-sneakers-shop.fr"); // Vérifie si distant
                }
            }
            catch (Exception)
            {
                urlTextBlock.Text = "❌ Impossible de charger l'URL.";
            }
        }

        private void ReloadConfig_Click(object sender, RoutedEventArgs e)
        {
            LoadCurrentUrl();
            MessageBox.Show("🔄 Configuration rechargée !");
        }
    }
}
