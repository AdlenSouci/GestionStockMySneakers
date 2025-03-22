using System;

namespace GestionStockMySneakers.Models
{
    public class Couleur
    {
        public int id { get; set; }
        public string? nom_couleur { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
