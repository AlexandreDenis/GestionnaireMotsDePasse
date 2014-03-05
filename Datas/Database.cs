using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datas
{
    /// <summary>
    /// Classe contenant l'arborescence des données (Groupe + Entry)
    /// </summary>
    [Serializable]
    public class Database
    {
        /// <summary>
        /// Groupe racine de la base
        /// </summary>
        private Groupe root;
        public Groupe Root
        {
            get { return root; }
            set { root = value; }
        }

        /// <summary>
        /// Longueur totale des mots de passe générés pour les clés
        /// </summary>
        private int lenghtPassword = 8;
        public int LenghtPassword
        {
            get { return lenghtPassword; }
            set { lenghtPassword = value; }
        }

        /// <summary>
        /// Nombre de caractères spéciaux dans les mots de passe générés pour les clés
        /// </summary>
        private int nbCaracSpec = 4;
        public int NbCaracSpec
        {
            get { return nbCaracSpec; }
            set { nbCaracSpec = value; }
        }

        /// <summary>
        /// Constructeur de la classe Database
        /// </summary>
        /// <param name="inRoot">Groupe racine de la database</param>
        public Database(Groupe inRoot)
        {
            root = inRoot;
        }

        /// <summary>
        /// Constructeur par défaut de la classe Database -> utile pour la sérialisation
        /// </summary>
        public Database()
        {
            root = new Groupe();
        }

        /// <summary>
        /// Conversion en string de l'instance courante de la classe Database
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return root.ToString();
        }
    }
}
