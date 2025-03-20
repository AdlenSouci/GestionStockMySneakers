using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows;
using System.Configuration; // Nécessaire pour lire App.config

namespace GestionStockMySneakers
{
    public class Login
    {
        
        private static readonly HttpClient client = new HttpClient();

        private static readonly string apiUrl = ConfigurationManager.AppSettings["api_url"] + "/login"; 

        public static async Task<string?> LogInAsync(string email, string password)
        {
            using (HttpClient client = new HttpClient()) // Client créé à chaque requête
            {
                try
                {
                    var loginData = new
                    {
                        email = email,
                        password = password
                    };

                    var jsonData = JsonConvert.SerializeObject(loginData);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(apiUrl, content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);
                        return responseObject?.token ?? null;
                    }
                    else
                    {
                        MessageBox.Show($"Échec de la connexion : {response.StatusCode}\nMessage: {responseString}");
                        return null;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Une erreur s'est produite lors de la tentative de connexion : " + ex.Message);
                    return null;
                }
            }
        }

    }
}
