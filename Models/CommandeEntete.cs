using System;
using System.Collections.Generic;

namespace GestionStockMySneakers.Models
{
    public class CommandeEntete
    {
        public int id { get; set; }                     // Clé primaire
        public string date { get; set; }                // Date de la commande
        public int id_user { get; set; }                // ID de l'utilisateur
        public int id_num_commande { get; set; }        // Numéro de commande
        public decimal total_ht { get; set; }           // Total HT
        public decimal total_ttc { get; set; }          // Total TTC
        public decimal total_tva { get; set; }          // TVA
        public decimal total_remise { get; set; }       // Remise totale
        public DateTime created_at { get; set; }        // Horodatage création
        public DateTime updated_at { get; set; }        // Horodatage modification

        // Optionnel : liste des détails liés à cette commande
        public List<CommandeDetail> details { get; set; }
    }
}
