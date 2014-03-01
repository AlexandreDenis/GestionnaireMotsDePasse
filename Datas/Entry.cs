using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GenerationMotDePasse;

namespace Datas
{
    [Serializable]
    public class Entry : Node
    {
        private static int lenghtPassword = 8;
        public static int LenghtPassword
        {
            get { return Entry.lenghtPassword; }
            set { Entry.lenghtPassword = value; }
        }

        /// <summary>
        /// Mot de passe pour l'URL
        /// </summary>
        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        /// <summary>
        /// Référence vers l'adresse du site web correspondant
        /// </summary>
        private string url;
        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        /// <summary>
        /// Identifiant pour l'URL
        /// </summary>
        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="inTitle"></param>
        /// <param name="inExpiration"></param>
        /// <param name="inUsername"></param>
        /// <param name="inUrl"></param>
        public Entry(string inTitle, Nullable<DateTime> inExpiration, DateTime inDateCreation, DateTime inDateModification, string inUsername, string inUrl)
            : base(inTitle, inExpiration,inDateCreation,inDateModification)
        {
            username = inUsername;
            url = inUrl;
            GeneratePassword(Entry.LenghtPassword);
        }

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public Entry()
            :base()
        {
            username = "Unknown";
            url = "no url";
            GeneratePassword(Entry.LenghtPassword);
        }

        /// <summary>
        /// Génère le mot de passe pour la clé courante
        /// </summary>
        /// <param name="length"></param>
        public void GeneratePassword(int length)
        {
            IPasswordGenerator ipg = PasswordGeneratorFactory.Create();

            try
            {
                password = ipg.GeneratePassword(length);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public override string ToString()
        {
            StringBuilder chaine = new StringBuilder();

            chaine.Append("Clé ");
            chaine.Append(Title);
            chaine.Append(" (expiration ");
            chaine.Append(Expiration);
            chaine.Append(") identifiant=");
            chaine.Append(Username);
            chaine.Append(" mot de passe=");
            chaine.Append(Password);

            return chaine.ToString();
        }
    }
}
