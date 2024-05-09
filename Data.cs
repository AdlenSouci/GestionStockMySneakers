using MySql.Data.MySqlClient;

namespace GestionStockMySneakers
{
    public class Data
    {
        private string _dsn = "server=localhost;port=3306;database=projet_sneakers;username=root;password=;";
        private MySqlConnection _connexion;

        public MySqlConnection Connexion()
        {
            _connexion = new MySqlConnection(_dsn);
            return _connexion;
        }

    }
}
