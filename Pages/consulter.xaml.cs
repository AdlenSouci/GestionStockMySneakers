using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls; 
using System.Windows.Media.Imaging;

namespace GestionStockMySneakers.Pages
{

    public partial class consulter : Page
    {
        public consulter()
        {
            InitializeComponent();
            ChargerImagesFTP();
        }

        private void ChargerImagesFTP()
        {
            string urlServeur = "ftp.kuph3194.odns.fr/resources/img/";
            string nomUtilisateur = "sneakers@ifcsio2022.fr";
            string motDePasse = "5NRN7lbrz3Zt";

            try
            {
                FtpWebRequest requeteFTP = (FtpWebRequest)WebRequest.Create(urlServeur);
                requeteFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                requeteFTP.Credentials = new NetworkCredential(nomUtilisateur, motDePasse);
                requeteFTP.EnableSsl = true;


                using (FtpWebResponse reponseFTP = (FtpWebResponse)requeteFTP.GetResponse())
                using (Stream flux = reponseFTP.GetResponseStream())
                using (StreamReader lecteur = new StreamReader(flux))
                {
                    string ligne;
                    while ((ligne = lecteur.ReadLine()) != null)
                    {
                        // Vérifier si le fichier est une image
                        if (ligne.EndsWith(".jpg") || ligne.EndsWith(".webp") || ligne.EndsWith(".jpeg") || ligne.EndsWith(".png") || ligne.EndsWith(".gif"))
                        {
                            string urlImage = urlServeur + ligne;

                            // Afficher l'image
                            AfficherImage(urlImage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite : " + ex.Message);
            }
        }




        private void AfficherImage(string urlImage)
        {
            try
            {
                if (urlImage != null)
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(urlImage);
                    image.EndInit();

                    Image img = new Image();
                    img.Source = image;

                    // Ajoutez l'image à votre interface utilisateur
                    // par exemple, si vous avez un conteneur nommé "grilleImages":
                    grilleImages.Children.Add(img);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement de l'image : " + ex.Message);
            }
        }

    }
}
