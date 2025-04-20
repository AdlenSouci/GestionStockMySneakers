# GestionStockMySneakers - Application de Gestion WPF

Application de bureau C# WPF conçue pour gérer les données (articles, commandes, utilisateurs, etc.) du site web `my-sneakers-shop.fr`. Ce guide explique comment récupérer et ouvrir le code source pour **consultation et analyse**.

**IMPORTANT : Ce projet est fourni ici uniquement pour consulter le code source. Il n'est pas destiné à être compilé ou exécuté.**

## Prérequis pour Consulter le Code

1.  **Visual Studio** :
    *   Installez [Visual Studio](https://visualstudio.microsoft.com/fr/vs/) (la version Community gratuite suffit).
    *   **Important :** Lors de l'installation via le "Visual Studio Installer", assurez-vous de cocher la case **"Développement desktop .NET"** pour avoir les outils nécessaires pour WPF (coloration syntaxique C#/XAML, explorateur de solutions, etc.).

2.  **Git** (Si vous utilisez `git clone`) :
    *   **Vous DEVEZ installer [Git](https://git-scm.com/downloads) pour pouvoir utiliser la commande `git clone`.** Si vous préférez télécharger le projet en ZIP depuis GitHub, l'installation de Git n'est pas nécessaire.

## Récupération et Ouverture du Code Source

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
    *   Double-cliquez sur le fichier se terminant par `.sln` (le fichier de Solution).
    *   Visual Studio va charger la structure du projet, vous permettant de naviguer dans les fichiers.

**NOTE : À partir de là, l'objectif est d'explorer le code. N'essayez pas de compiler ou de lancer l'application (pas de F5 / bouton Démarrer).**


**En résumé :** Ce README vous guide pour ouvrir le code dans Visual Studio. Utilisez l'Explorateur de solutions pour naviguer.
