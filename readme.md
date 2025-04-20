# GestionStockMySneakers - Application de Gestion WPF

Application de bureau pour gérer les données (articles, commandes, utilisateurs, etc.) du site web `my-sneakers-shop.fr`.

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

## Installation et Lancement

1.  **Préparer un Dossier pour le Projet :**
    *   **Ouvrez l'Explorateur de fichiers** de Windows.
    *   **Naviguez jusqu'à l'endroit où vous voulez stocker vos projets** (par exemple, votre dossier `Documents`, ou directement sur le disque `C:\`).
    *   **Créez un nouveau dossier** dédié à vos projets C#, par exemple, nommez-le `Projets_CSharp`.
    *   **Entrez dans ce nouveau dossier** (`Projets_CSharp`). C'est *ici* que nous allons télécharger le code.

2.  **Récupérer le Code Source :**
    *   **Méthode A : Avec `git clone` (Nécessite Git installé)**
        *   Ouvrez l'**Invite de commandes (`cmd`)** *dans le dossier `Projets_CSharp`* que vous venez de créer (astuce: tapez `cmd` dans la barre d'adresse de l'Explorateur et appuyez sur Entrée).
        *   Dans la fenêtre `cmd`, tapez la commande suivante et appuyez sur Entrée :
            ```bash
            git clone https://github.com/AdlenSouci/GestionStockMySneakers.git
            ```
    *   **Méthode B : Télécharger le ZIP (Ne nécessite pas Git)**
        *   Allez sur la page GitHub du projet : [https://github.com/AdlenSouci/GestionStockMySneakers](https://github.com/AdlenSouci/GestionStockMySneakers)
        *   Cliquez sur le bouton vert `Code`, puis sur `Download ZIP`.
        *   Enregistrez le fichier ZIP, puis **décompressez-le** dans votre dossier `Projets_CSharp`. Assurez-vous que cela crée un dossier `GestionStockMySneakers` à l'intérieur.

3.  **Ouvrir dans Visual Studio :**
    *   Quel que soit la méthode de récupération, allez dans le dossier `GestionStockMySneakers` (qui se trouve maintenant dans `Projets_CSharp`).
    *   Double-cliquez sur le fichier qui se termine par `.sln` (le fichier de Solution). Visual Studio va s'ouvrir.

4.  **(Si vous travaillez en Local) Démarrer XAMPP :**
    *   Lancez le **"Panneau de Contrôle XAMPP"**.
    *   Cliquez sur **"Start"** pour **Apache**.
    *   Cliquez sur **"Start"** pour **MySQL**.
    *   *L'application a besoin que ces deux services soient démarrés pour fonctionner en mode local.*

5.  **Compiler et Lancer :**
    *   Dans Visual Studio, attendez que les dépendances se chargent (cela peut prendre un moment la première fois).
    *   Appuyez sur `F5` ou cliquez sur le bouton vert "Démarrer" en haut.
    *   L'application devrait se lancer.

## Connexion à la Base de Données (Local vs Distant)

*   L'application peut se connecter soit à une base de données **locale** (via XAMPP), soit à une base de données **distante** (celle du site web, par exemple).
*   Un **paramètre ou un switch** est disponible **dans l'application** (probablement dans une section "Paramètres" ou "Configuration") pour choisir entre la connexion **locale** et **distante**.
*   Par défaut, elle est probablement configurée pour essayer de se connecter en local (XAMPP). Assurez-vous donc que XAMPP (Apache et MySQL) est démarré si vous utilisez ce mode.

## Utilisation

*   Naviguez entre les différentes sections (Articles, Familles, Marques, Commandes, etc.) via le menu principal ou les éventuels onglets pour voir et gérer les données.
*   Utilisez les boutons prévus (Ajouter, Modifier, Supprimer, Consulter) pour interagir avec les informations.

---
