using System;

namespace GestionStockMySneakers.Models
{
    public class Famille
    {
        public int id { get; set; }
        public int? id_parent { get; set; }
        public string? nom_famille { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
