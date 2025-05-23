# Cahier des Charges - GestionStockMySneakers

## 1. Présentation du Projet
**GestionStockMySneakers** est une application WPF développée en C#. Elle permet de gérer les stocks et les commandes d'un site de vente de sneakers via des routes API Laravel.

## 2. Objectifs
- Permettre la gestion des articles, marques, familles, commandes et stocks.
- Offrir une interface simple et intuitive pour effectuer des actions CRUD.
- Utiliser les routes API Laravel comme intermédiaire avec la base de données.

## 3. Technologies Utilisées
- **Frontend** : WPF (C#)
- **Backend** : Laravel (PHP)
- **Base de Données** : MySQL
- **Communication** : API REST

## 4. Fonctionnalités
### 4.1 Gestion des Articles
- Ajout, modification, suppression et consultation d'articles.

### 4.2 Gestion des Marques et Familles
- CRUD des marques et familles de sneakers.

### 4.3 Gestion des Commandes
- Ajout (même si l'intérêt d'ajouter des commandes depuis une application lourde est util uniquement pour du teste de fonctionnalié, il est possible de tester cette fonctionnalité).
- etape  :  ajouter un article de la base de donnée ensuite ajouter une commande.
- Suivi des commandes des clients.

### 4.4 Gestion du Stock
- Suivi des entrées et sorties du stock.
- Mise à jour des quantités disponibles.

### 4.5 Gestion des Utilisateurs
- Seul l'administrateur peut se connecter.
- Seul l'administrateur peut ajouter, modifier ou supprimer des éléments.

### 4.6 Configuration du Serveur
L'application permet de basculer entre un serveur local et un serveur hébergé via un fichier `app.config`. Ce fichier contient les paramètres suivants :

<configuration> <appSettings>  <add key = "api_url" value="https://my-sneakers-shop.fr/api"/>
L'utilisateur peut modifier cette valeur pour passer du serveur local à l'hébergement distant.

## 5. Communication avec l'API Laravel
- L'application WPF envoie des requêtes aux routes API Laravel.
- Laravel gère les traitements et met à jour la base de données.
- Aucune connexion directe à la base de données depuis l'application.

## 6. Interface Utilisateur (Captures d'écran)
Afin de mieux comprendre les fonctionnalités, voici les principales pages disponibles :

### 6.1 Page de Connexion
- Seul l'administrateur peut se connecter pour utiliser les fonctionnalités CRUD.
- _[Insérer une capture d'écran ici]_

### 6.2 Tableau de Bord
- Vue globale des stocks, commandes et utilisateurs.
- _[Insérer une capture d'écran ici]_

### 6.3 Gestion des Articles
- Ajout, modification et suppression des articles.
- _[Insérer une capture d'écran ici]_

### 6.4 Gestion du Stock
- Suivi des entrées et sorties du stock.
- _[Insérer une capture d'écran ici]_

## 7. Conclusion
Ce projet vise à simplifier la gestion des stocks et commandes à l'aide d'une application locale connectée à un backend Laravel via API.

---
