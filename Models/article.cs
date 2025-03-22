using System;

namespace GestionStockMySneakers.Models
{
    public class Article
    {
        public int id { get; set; }
        public string? nom_marque { get; set; }
        public string? nom_famille { get; set; }
        public string? modele { get; set; }
        public string? description { get; set; }
        public string? nom_couleur { get; set; }
        public decimal prix_public { get; set; }
        public decimal prix_achat { get; set; }
        public string? img { get; set; }
        public int id_famille { get; set; }
        public int id_marque { get; set; }
    }
}
