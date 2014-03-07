using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GenerationMotDePasse;

namespace Datas
{
    /// <summary>
    /// Classe représentant une clé
    /// </summary>
    [Serializable]
    public class Entry : Node
    {
        /// <summary>
        /// Longueur totale des mots de passe générés pour les clés
        /// </summary>
        private static int lenghtPassword = 8;
        public static int LenghtPassword
        {
            get { return Entry.lenghtPassword; }
            set { Entry.lenghtPassword = value; }
        }

        /// <summary>
        /// Nombre de caractères spéciaux dans les mots de passe générés pour les clés
        /// </summary>
        private static int nbCaracSpec = 4;
        public static int NbCaracSpec
        {
            get { return Entry.nbCaracSpec; }
            set { Entry.nbCaracSpec = value; }
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
        /// Constructeur de la classe Entry
        /// </summary>
        /// <param name="inTitle">Nom de le clé</param>
        /// <param name="inExpiration">Date d'expiration de la clé</param>
        /// <param name="inDateCreation">Date de création de la clé</param>
        /// <param name="inDateModification">Date de modification de la clé</param>
        /// <param name="inUsername">Identifiant correspondant à la clé</param>
        /// <param name="inUrl">Mot de passe correspondant à la clé</param>
        public Entry(string inTitle, Nullable<DateTime> inExpiration, DateTime inDateCreation, DateTime inDateModification, string inUsername, string inUrl)
            : base(inTitle, inExpiration,inDateCreation,inDateModification)
        {
            username = inUsername;
            url = inUrl;
            GeneratePassword(Entry.LenghtPassword, Entry.NbCaracSpec);
        }

        /// <summary>
        /// Constructeur par défaut de la classe Entry
        /// </summary>
        public Entry()
            :base()
        {
            username = "Unknown";
            url = "no url";
            GeneratePassword(Entry.LenghtPassword, Entry.NbCaracSpec);
        }
        
        /// <summary>
        /// Génère le mot de passe pour la clé courante
        /// </summary>
        /// <param name="length">Longueur totale du mot de passe à générer</param>
        /// <param name="nbCaracSpec">Nombre de caractères spéciaux pour le mot de passe à générer</param>
        public void GeneratePassword(int length, int nbCaracSpec)
        {
            IPasswordGenerator ipg = PasswordGeneratorFactory.Create();

            try
            {
                password = ipg.GeneratePassword(length, nbCaracSpec);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
            }
        }


        /// <summary>
        /// Conversion de la classe Entry en string
        /// </summary>
        /// <returns>Chaîne représentant l'instance courante de la classe Entry</returns>
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
