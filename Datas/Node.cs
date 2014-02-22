using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datas
{
    [Serializable]
    public class Node
    {
        /// <summary>
        /// Date d'expiration de la clé
        /// </summary>
        private Nullable<DateTime> expiration;
        public Nullable<DateTime> Expiration
        {
            get { return expiration; }
            set { expiration = value; }
        }

        /// <summary>
        /// Nom de la clé
        /// </summary>
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public Node()
        {
            title = "";
            expiration = null;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="inTitle"></param>
        /// <param name="inExpiration"></param>
        public Node(string inTitle, Nullable<DateTime> inExpiration)
        {
            title = inTitle;
            expiration = inExpiration;
        }
    }
}
