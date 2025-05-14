using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionStockMySneakers.Models
{
    public class TaillesArticle 
    {
        public int id { get; set; }
        [JsonProperty("article_id")]
        public int article_id { get; set; }
        public int taille { get; set; }
        public int stock { get; set; }
    }
}
