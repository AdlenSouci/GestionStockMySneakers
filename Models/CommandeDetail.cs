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
        public decimal prix_ht { get; set; }     // Garder pour l'instant car requis par la validation API
        public decimal prix_ttc { get; set; }    // Garder pour l'instant car requis par la validation API
        public decimal montant_tva { get; set; } // Garder pour l'instant car requis par la validation API
        public decimal remise { get; set; }      // Garder pour l'instant car requis par la validation API

        [JsonIgnore] // Géré par Laravel
        public DateTime created_at { get; set; }

        [JsonIgnore] // Géré par Laravel
        public DateTime updated_at { get; set; }
    }
}
