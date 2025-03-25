using System;

namespace GestionStockMySneakers.Models
{
    public class Users
    {
        public int user_id { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? adresse_livraison { get; set; }
        public string? password { get; set; }
     
        public DateTime? email_verified_at { get; set; }

        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool is_admin { get; set; }

    }
}
