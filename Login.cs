using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GestionStockMySneakers
{
    public class Login
    {

        public bool logIn(string username, string password)
        {
            const string expectedUsername = "username";
            const string expectedPassword = "password";

            return (username == expectedUsername && password == expectedPassword);
        }


    }


}
