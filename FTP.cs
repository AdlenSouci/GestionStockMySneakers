using System;
using System.Net;
using System.Windows;

namespace GestionStockMySneakers
{
    public class FTP
    {
        private string _host;
        private NetworkCredential _credential;
        private WebClient _client;

        public FTP(string Host, string Username, string Password)
        {
            _host = Host;
            _credential = new NetworkCredential(Username, Password);
            _client = new WebClient();
            _client.Credentials = _credential;
            _client.BaseAddress = _host;
        }

        public bool Upload(string LocalFile, string RemoteFile)

        {
            try
            {
                _client.UploadFile(RemoteFile, LocalFile);
                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }

        public bool Download(string RemoteFile, string LocalFile)

        {
            try
            {
                _client.DownloadFile(RemoteFile, LocalFile);
                MessageBox.Show("Download OK");
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }
    }
}
