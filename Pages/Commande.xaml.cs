using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GestionStockMySneakers.Pages
{
    /// <summary>
    /// Logique d'interaction pour Commande.xaml
    /// </summary>
    public partial class Commande : Page
    {
        Data d = new Data();
        private MySqlConnection _connexion;
        private MySqlDataAdapter _adapter;
        private DataTable _dt;
        public Commande()
        {
            _connexion = d.Connexion();
            InitializeComponent();
            afficher();
        }
        private void afficher()
        {
            try
            {
                _adapter = new MySqlDataAdapter("SELECT * FROM commandes_entetes;", _connexion);
                _dt = new DataTable();
                _adapter.Fill(_dt);
                dgCommandes.ItemsSource = _dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
