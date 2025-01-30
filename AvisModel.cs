using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionStockMySneakers
{
    public class AvisModel
    {
        public int Id { get; set; }  // ID de l'avis
        public int UserId { get; set; }  // ID de l'utilisateur qui a laissé l'avis
        public int ArticleId { get; set; }  // ID de l'article
        public string Contenu { get; set; }  // Contenu de l'avis
        public int Note { get; set; }  // Note donnée par l'utilisateur
        public DateTime CreatedAt { get; set; }  // Date de création de l'avis
        public string Reponse { get; set; }  // Réponse de l'administrateur
    }
}
