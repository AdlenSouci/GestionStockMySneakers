using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace GestionStockMySneakers.Pages
{
    public partial class consulter : Page
    {
        public consulter()
        {
            InitializeComponent();

            List<string> files = GetFileList("ftp://ftp.kuph3194.odns.fr/resources/img/", "sneakers@ifcsio2022.fr", "5NRN7lbrz3Zt");
            listBox.ItemsSource = files;
        }

        public List<string> GetFileList(string server, string user, string password)
        {
            List<string> files = new List<string>();
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(server);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(user, password);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        // Download the file and save it locally
                        string localFilePath = DownloadFile(server + "/" + line, user, password, line);
                        files.Add(localFilePath);

                        line = reader.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return files;
        }

        public string DownloadFile(string serverFilePath, string user, string password, string fileName)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(serverFilePath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(user, password);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (FileStream writer = new FileStream(fileName, FileMode.Create))
                {
                    responseStream.CopyTo(writer);
                }

                return Path.GetFullPath(fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

    }
}
