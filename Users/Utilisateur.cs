using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users
{
    /// <summary>
    /// Classe réprésentant un utilisateur du programme
    /// </summary>
    [Serializable]
    public class Utilisateur
    {
        /// <summary>
        /// Login de l'utilisateur
        /// </summary>
        private string _login;
        public string Login
        {
            get { return _login; }
            set { _login = value; }
        }

        /// <summary>
        /// Mot de passe de l'utilisateur
        /// </summary>
        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        /// <summary>
        /// Clé de cryptage de l'utilisateur
        /// </summary>
        private string _cléDeCryptage;
        public string CléDeCryptage
        {
            get { return _cléDeCryptage; }
            set { _cléDeCryptage = value; }
        }

        /// <summary>
        /// Constructeur de la classe Utilisateur
        /// </summary>
        /// <param name="inLogin">Login du nouvel utilisateur</param>
        /// <param name="inPassword">Mot de passe du nouvel utilisateur</param>
        /// <param name="inCléDeCryptage">Clé de cryptage du nouvel utilisateur</param>
        public Utilisateur(string inLogin, string inPassword, string inCléDeCryptage)
        {
            Login = inLogin;
            Password = inPassword;
            _cléDeCryptage = inCléDeCryptage;
        }
    }
}
