using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_GMP
{
    /// <summary>
    /// Interface de l'API avec la classe Entry afin de l'utiliser comme une boîte noire et de masquer certains éléments
    /// </summary>
    public class API_Key
    {
        /// <summary>
        /// Nom de la clé
        /// </summary>
        private string title;
        internal string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// Identifiant pour l'URL
        /// </summary>
        private string username;
        internal string Username
        {
            get { return username; }
            set { username = value; }
        }

        /// <summary>
        /// Mot de passe pour l'URL
        /// </summary>
        private string password;
        internal string Password
        {
            get { return password; }
            set { password = value; }
        }

        /// <summary>
        /// Constructeur de la classe API_Key
        /// </summary>
        /// <param name="inTitle">Nom de la clé</param>
        /// <param name="inUsername">Identifiant correspondant à la clé</param>
        /// <param name="inPassword">Mot de passe correspondant à la clé</param>
        internal API_Key(string inTitle, string inUsername, string inPassword)
        {
            Title = inTitle;
            Username = inUsername;
            Password = inPassword;
        }

        /// <summary>
        /// Convertit l'instance de API_Key en string
        /// </summary>
        /// <returns>Chaîne représentant l'instance de la classe courante</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Clé ");
            sb.Append(title);
            sb.Append(" | Identifiant : ");
            sb.Append(username);
            sb.Append(" | Mot de passe : ");
            sb.Append(password);

            return sb.ToString();
        }
    }
}
