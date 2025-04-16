using System;
using System.Collections.Generic;
using Newtonsoft.Json; // Assure-toi d'avoir ce using

namespace GestionStockMySneakers.Models
{
    public class CommandeEntete
    {
        [JsonProperty("id")] // <-- Indique que le champ JSON "id" doit mapper vers cette propriété
        public int id_commande { get; set; }

        public int id_user { get; set; }
        public int id_num_commande { get; set; } // Tu le génères en C#
        public decimal total_ht { get; set; }
        public decimal total_ttc { get; set; }
        public decimal total_tva { get; set; }
        public decimal total_remise { get; set; }

        [JsonIgnore] // Géré par Laravel
        public DateTime created_at { get; set; }

        [JsonIgnore] // Géré par Laravel
        public DateTime updated_at { get; set; }

        // Laisser les détails être sérialisés
        public List<CommandeDetail> details { get; set; }
    }
}
