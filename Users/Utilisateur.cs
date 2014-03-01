using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users
{
    [Serializable]
    public class Utilisateur
    {
        private string _login;
        public string Login
        {
            get { return _login; }
            set { _login = value; }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private string _cléDeCryptage;
        public string CléDeCryptage
        {
            get { return _cléDeCryptage; }
            set { _cléDeCryptage = value; }
        }

        public Utilisateur(string inLogin, string inPassword, string inCléDeCryptage)
        {
            Login = inLogin;
            Password = inPassword;
            _cléDeCryptage = inCléDeCryptage;
        }
    }
}
