// Votre fichier Models/Article.cs

using System;
// Ajoutez cette ligne pour utiliser ObservableCollection
using System.Collections.ObjectModel;
// Ajoutez cette ligne pour que le compilateur trouve TaillesArticle
// (Elle est nécessaire si TaillesArticle est défini dans le même namespace
// mais que le fichier Article.cs ne le "voit" pas directement, ou si TaillesArticle est dans un sous-namespace)
using GestionStockMySneakers.Models;

namespace GestionStockMySneakers.Models // <- Votre namespace existant
{
    public class Article
    {
        public int id { get; set; }
        public int id_famille { get; set; }
        public string? nom_famille { get; set; }
        public int id_marque { get; set; }
        public string? nom_marque { get; set; }
        public string? modele { get; set; }
        public string? description { get; set; }
        public int id_couleur { get; set; }
        public string? nom_couleur { get; set; }
        public decimal prix_public { get; set; }
        public decimal prix_achat { get; set; }
        public string? img { get; set; }

        // AJOUTEZ CETTE PROPRIÉTÉ
        // Elle utilise ObservableCollection (necessite using System.Collections.ObjectModel;)
        // et TaillesArticle (necessite using GestionStockMySneakers.Models; si pas dans le même fichier/namespace)
        public ObservableCollection<TaillesArticle>? tailles { get; set; }

    }
}
