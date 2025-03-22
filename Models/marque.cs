using System;

namespace GestionStockMySneakers.Models
{
    public class Marque
    {
        public int id { get; set; }
        public string? nom_marque { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
