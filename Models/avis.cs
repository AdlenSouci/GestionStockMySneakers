using System;

namespace GestionStockMySneakers.Models
{
    public class Avis
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int article_id { get; set; }
        public string contenu { get; set; }
        public int note { get; set; }
        public DateTime created_at { get; set; }
        public string reponse { get; set; }
        public Article article { get; set; }
        public User user { get; set; }
    }

}
