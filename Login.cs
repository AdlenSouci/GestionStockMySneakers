using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows;

namespace GestionStockMySneakers
{
    public class Login
    {
        private static readonly HttpClient client = new HttpClient();
        private const string apiUrl = "http://127.0.0.1:8000/api/login";

        public static async Task<string?> LogInAsync(string email, string password)
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

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);
                    return responseObject?.token ?? null;
                }
                else
                {
                    MessageBox.Show("Échec de la connexion. Vérifiez vos identifiants.");
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
