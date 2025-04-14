using System;

namespace GestionStockMySneakers.Models
{
    public class CommandeDetail
    {
        public int id { get; set; }                      // Clé primaire
        public int id_commande { get; set; }             // Référence vers CommandeEntete
        public int id_article { get; set; }              // Référence vers Article
        public string taille { get; set; }               // Taille de l'article
        public int quantite { get; set; }                // Quantité
        public decimal prix_ht { get; set; }             // Prix HT unitaire
        public decimal prix_ttc { get; set; }            // Prix TTC unitaire
        public decimal montant_tva { get; set; }         // Montant TVA unitaire
        public decimal remise { get; set; }              // Remise appliquée
        public DateTime created_at { get; set; }         // Horodatage création
        public DateTime updated_at { get; set; }         // Horodatage modification
    }
}
