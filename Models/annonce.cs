using System;

namespace GestionStockMySneakers.Models
{
    public class Annonce
    {
        public int id { get; set; }
        public string? h1 { get; set; }
        public string? h3 { get; set; }
        public string? texte { get; set; }
        public string? imageURL { get; set; }
        public string? statut { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
