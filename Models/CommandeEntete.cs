using System;
using System.Collections.Generic;
using Newtonsoft.Json; // Assure-toi d'avoir ce using

namespace GestionStockMySneakers.Models
{
    public class CommandeEntete
    {
        [JsonIgnore] // Ne pas envoyer lors de la création (POST)
        public int id { get; set; }

        // [JsonIgnore] // Optionnel: Si Laravel gère toujours la date via now(), tu peux l'ignorer aussi.
        // public string date { get; set; } // Sinon, assure-toi de l'envoyer dans un format que l'API comprend si nécessaire.

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
