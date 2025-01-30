using MySql.Data.MySqlClient;

namespace GestionStockMySneakers
{
    public class Data
    {
        private string _dsn = "Server=my-sneakers-shop.fr;Port=3306;Database=kera6497_my-sneakers;username=kera6497_adlen;password=789-AA__s;SslMode=none;";
        private MySqlConnection _connexion;

        public MySqlConnection Connexion()
        {
            _connexion = new MySqlConnection(_dsn);
            return _connexion;
        }

    }
}
