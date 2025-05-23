# GestionStockMySneakers - Application de Gestion WPF

Application de bureau C# WPF conçue pour gérer les données (articles, commandes, utilisateurs, etc.) du site web `my-sneakers-shop.fr`. Ce guide explique comment installer et lancer l'application.

## Prérequis

Pour utiliser cette application, vous avez besoin de :

1.  **Visual Studio** :
    *   Installez [Visual Studio](https://visualstudio.microsoft.com/fr/vs/) (la version Community gratuite suffit).
    *   **Important :** Pendant l'installation via le "Visual Studio Installer", assurez-vous de cocher la case **"Développement desktop .NET"** pour inclure les outils nécessaires pour WPF.

2.  **XAMPP** (Pour travailler en local) :
    *   Si vous voulez connecter l'application à une base de données sur votre propre ordinateur (mode local), installez [XAMPP](https://www.apachefriends.org/fr/index.html).
    *   XAMPP fournit un serveur de base de données MySQL facile à utiliser.

3.  **Git** (Si vous utilisez `git clone`) :
    *   **Vous DEVEZ installer [Git](https://git-scm.com/downloads) pour pouvoir utiliser la commande `git clone`.** Si vous préférez télécharger le projet en ZIP depuis GitHub, l'installation de Git n'est pas nécessaire.

4.  **(Pour le Mode Local) Projet Web `my-sneakers-shop` (Laravel) :**
    *   Pour que la connexion locale fonctionne, vous devez avoir au préalable installé et configuré le projet web `my-sneakers-shop` (le projet Laravel) sur votre machine, y compris sa base de données locale.

## Installation et Lancement

1.  **Préparer un Dossier pour le Projet :**
    *   **Ouvrez l'Explorateur de fichiers** de Windows.
    *   **Naviguez jusqu'à l'endroit où vous voulez stocker vos projets** (par exemple, votre dossier `Documents`).
    *   **Créez un nouveau dossier** (ex: `Projets_CSharp`).
    *   **Entrez dans ce nouveau dossier**.

2.  **Récupérer le Code Source :**
    *   **Méthode A : Avec `git clone` (Nécessite Git installé)**
        *   Ouvrez l'**Invite de commandes (`cmd`)** *dans le dossier (`Projets_CSharp`)* créé précédemment (astuce: tapez `cmd` dans la barre d'adresse de l'Explorateur et appuyez sur Entrée).
        *   Dans la fenêtre `cmd`, tapez :
            ```bash
            git clone https://github.com/AdlenSouci/GestionStockMySneakers.git
            ```
    *   **Méthode B : Télécharger le ZIP**
        *   Allez sur [https://github.com/AdlenSouci/GestionStockMySneakers](https://github.com/AdlenSouci/GestionStockMySneakers).
        *   Cliquez `Code` -> `Download ZIP`.
        *   Décompressez le fichier ZIP dans votre dossier `Projets_CSharp`.

3.  **Ouvrir dans Visual Studio :**
    *   Allez dans le dossier `GestionStockMySneakers` (qui se trouve maintenant dans `Projets_CSharp`).
    *   Double-cliquez sur le fichier se terminant par `.sln` (le fichier de Solution). Visual Studio va s'ouvrir.

4.  **(Si vous travaillez en Local) Démarrer les Services Locaux :**
    *   Lancez le **"Panneau de Contrôle XAMPP"**.
    *   Cliquez sur **"Start"** pour **Apache**.
    *   Cliquez sur **"Start"** pour **MySQL**.
    *   Assurez-vous également que votre serveur de développement **Laravel** (pour l'API locale, si l'application l'utilise) est démarré (généralement avec `php artisan serve`).

5.  **Compiler et Lancer :**
    *   Dans Visual Studio, attendez que les dépendances se chargent (cela peut prendre un moment la première fois).
    *   Appuyez sur `F5` ou cliquez sur le bouton vert "Démarrer" en haut.
    *   L'application devrait se lancer.

## Connexion à la Base de Données (Local vs Distant)

*   L'application peut se connecter soit à une base de données **locale** (via XAMPP), soit à une base de données **distante**.
*   Un **paramètre ou un switch** est disponible **dans l'application** (dans une section "Paramètres") pour choisir entre la connexion **locale** et **distante**.

*   **Mode Distant :**
    *   Ce mode se connecte à la base de données du site web seulement heberger.
    *   **Important :** La connexion distante n'est possible **que si le site web est effectivement hébergé et accessible en ligne.** Si le site n'est pas déployé, ce mode ne fonctionnera pas.

*   **Mode Local :**
    *   Ce mode se connecte à la base de données que vous avez configurée sur votre machine lors de l'installation du **projet web `my-sneakers-shop` (le projet Laravel)**.
    *   Pour que cela fonctionne :
        1.  Le serveur **MySQL de XAMPP doit être démarré**.
        2.  La **base de données locale** du projet web doit exister.
        3.  Les **informations de connexion** (nom de la base, utilisateur, mot de passe) que vous configurez dans l'application WPF (via la page "Paramètres" pour le mode local) **doivent être strictement identiques** à celles définies dans le fichier `.env` de votre projet Laravel local.

*   Par défaut, l'application est généralement configurée pour tenter une connexion locale au démarrage.

## ⚠️ Connexion à l'application

 **Important :** Pour vous connecter depuis l'écran de connexion de l'application, vous devez obligatoirement utiliser **l'identifiant `admin`** créé automatiquement par le **seeder `SeederAdmin`** dans le **projet Laravel (client léger)**.

- Cet identifiant `admin` est inséré dans la base de données lors de l'exécution du seeder Laravel.
- Si vous ne parvenez pas à vous connecter, vérifiez que ce seeder a bien été exécuté (via `php artisan db:seed --class=SeederAdmin`) et que l'utilisateur `admin` est bien présent dans la base de données cible (locale ou distante).
- Sans cet utilisateur `admin`, l'accès à l'application de gestion sera bloqué.

---

## Utilisation

*   Naviguez entre les différentes sections (Articles, Familles, Marques, Commandes, etc.) via le menu principal ou les éventuels onglets pour voir et gérer les données.
*   Utilisez les boutons prévus (Ajouter, Modifier, Supprimer, Consulter) pour interagir avec les informations.

---
