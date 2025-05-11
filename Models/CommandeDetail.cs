using System;
using Newtonsoft.Json; // Assure-toi d'avoir ce using

namespace GestionStockMySneakers.Models
{
    public class CommandeDetail
    {
        [JsonIgnore]
        public int id { get; set; }

        [JsonIgnore]
        public int id_commande { get; set; }

        public int id_article { get; set; }
        public string taille { get; set; }
        public int quantite { get; set; }
        public decimal prix_ht { get; set; }
        public decimal prix_ttc { get; set; }
        public decimal montant_tva { get; set; }
        public decimal remise { get; set; }

        [JsonIgnore]
        public DateTime created_at { get; set; }

        [JsonIgnore]
        public DateTime updated_at { get; set; }
    }
}
