using System;
using System.Collections.Generic;
using Newtonsoft.Json; // Assure-toi d'avoir ce using

namespace GestionStockMySneakers.Models
{
    public class CommandeEntete
    {
        [JsonProperty("id")]
        public int id_commande { get; set; }
        public string name { get; set; }
        public int id_user { get; set; }
        public int id_num_commande { get; set; }
        public decimal total_ht { get; set; }
        public decimal total_ttc { get; set; }
        public decimal total_tva { get; set; }
        //public decimal total_remise { get; set; }

        

        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }

        [JsonProperty("updated_at")]
        public DateTime updated_at { get; set; }


        public List<CommandeDetail> details { get; set; }
    }
}
