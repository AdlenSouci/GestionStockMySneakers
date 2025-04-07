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
- Ajout et suivi des commandes des clients.

### 4.4 Gestion du Stock
- Suivi des entrées et sorties du stock.
- Mise à jour des quantités disponibles.

### 4.5 Gestion des Utilisateurs
- Seul l'administrateur peut se connecter.
- Seul l'administrateur peut ajouter, modifier ou supprimer des éléments.

## 5. Communication avec l'API Laravel
- L'application WPF envoie des requêtes aux routes API Laravel.
- Laravel gère les traitements et met à jour la base de données.
- Aucune connexion directe à la base de données depuis l'application.

## 6. Conclusion
Ce projet vise à simplifier la gestion des stocks et commandes à l'aide d'une application locale connectée à un backend Laravel via API.

---
