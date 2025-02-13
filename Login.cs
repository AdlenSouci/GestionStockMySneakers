using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;

namespace GestionStockMySneakers
{
    public class LoginService
    {
        private string connectionString = "Server=localhost;Port=3306;Database=kera6497_my-sneakers;username=kera6497_adlen;password=789-AA__s;SslMode=none;";

        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Convertit chaque byte en hexadécimal
                }
                return builder.ToString();
            }
        }

        public bool logIn(string email, string password)
        {
            bool isAuthenticated = false;
            string hashedPassword = HashPassword(password);

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE email = @Email AND password = @Password";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", hashedPassword);

                        // Debug: Afficher les valeurs des paramètres
                        Console.WriteLine($"Email: {email}, Mot de passe haché: {hashedPassword}");

                        int count = Convert.ToInt32(command.ExecuteScalar());
                        isAuthenticated = count > 0; // Si count > 0, l'utilisateur est authentifié
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur de connexion : " + ex.Message);
                }
            }

            return isAuthenticated;
        }
    }
}
