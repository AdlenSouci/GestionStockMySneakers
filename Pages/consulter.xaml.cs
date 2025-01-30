using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GestionStockMySneakers.Pages
{
    public class Article
    {
        public string img { get; set; }
        public string marque { get; set; }
        public string nom_famille { get; set; }
        public string modele { get; set; }
        public string description { get; set; }
        public string couleur { get; set; }
        public string prix_public { get; set; }
        public string prix_achat { get; set; }
    }

    public partial class consulter : Page
    {
        // Objets nécessaires pour SQL
        const string _dsn = "server=localhost;port=3306;database=kuph3194_sneakers;username=kuph3194_adlen;password=5NRN7lbrz3Zt";
        private MySqlConnection _connexion = new MySqlConnection(_dsn);
        private MySqlCommand _command;

        public consulter()
        {
            InitializeComponent();
            afficher();
        }

        private void afficher()
        {
            try
            {
                _command = new MySqlCommand("SELECT * FROM articles;", _connexion);
                _connexion.Open();
                MySqlDataReader reader = _command.ExecuteReader();

                List<Article> articles = new List<Article>();
                while (reader.Read())
                {
                    articles.Add(new Article
                    {
                        img = reader["img"].ToString(),
                        marque = reader["marque"].ToString(),
                        nom_famille = reader["nom_famille"].ToString(),
                        modele = reader["modele"].ToString(),
                        description = reader["description"].ToString(),
                        couleur = reader["couleur"].ToString(),
                        prix_public = reader["prix_public"].ToString(),
                        prix_achat = reader["prix_achat"].ToString()
                    });
                }

                icArticles.ItemsSource = articles;  

                _connexion.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
