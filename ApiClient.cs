using System;
using System.Configuration;
using System.Net.Http;

namespace GestionStockMySneakers
{
    public static class ApiClient
    {
        public static readonly string apiUrl = ConfigurationManager.AppSettings["api_url"]
            ?? throw new ArgumentNullException(nameof(apiUrl), "L'URL de l'API est introuvable dans app.config.");
        private static readonly HttpClient _client;

        static ApiClient()
        {
            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromSeconds(30); // Par exemple, 30 secondes de timeout
        }

        public static HttpClient Client => _client;
    }
}
