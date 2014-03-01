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

        private DateTime dateCreation;
        public DateTime DateCreation
        {
            get { return dateCreation; }
            set { dateCreation = value; }
        }

        private DateTime dateModification;
        public DateTime DateModification
        {
            get { return dateModification; }
            set { dateModification = value; }
        }


        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public Node()
        {
            title = "";
            expiration = null;
            dateCreation = System.DateTime.Now;
            dateModification = System.DateTime.Now;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="inTitle"></param>
        /// <param name="inExpiration"></param>
        public Node(string inTitle, Nullable<DateTime> inExpiration, DateTime inDateCreation, DateTime inDateModification)
        {
            title = inTitle;
            expiration = inExpiration;
            dateCreation = inDateCreation;
            dateModification = inDateModification;
        }

        public void UpdateDateModification()
        {
            dateModification = System.DateTime.Now;
        }
    }
}
