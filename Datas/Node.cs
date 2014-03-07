using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datas
{
    /// <summary>
    /// Classe de base de chaque noeud de l'arborescence
    /// </summary>
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
        /// Date de création de la clé
        /// </summary>
        private DateTime dateCreation;
        public DateTime DateCreation
        {
            get { return dateCreation; }
            set { dateCreation = value; }
        }

        /// <summary>
        /// Date de dernière modification de la clé
        /// </summary>
        private DateTime dateModification;
        public DateTime DateModification
        {
            get { return dateModification; }
            set { dateModification = value; }
        }


        /// <summary>
        /// Constructeur par défaut de la classe Node
        /// </summary>
        public Node()
        {
            title = "";
            expiration = null;
            dateCreation = System.DateTime.Now;
            dateModification = System.DateTime.Now;
        }

        /// <summary>
        /// Constructeur de la classe Node
        /// </summary>
        /// <param name="inTitle">Nom du noeud</param>
        /// <param name="inExpiration">Date d'expiration du noeud</param>
        /// <param name="inDateCreation">Date de création du noeud</param>
        /// <param name="inDateModification">Date de dernière modification du noeud</param>
        public Node(string inTitle, Nullable<DateTime> inExpiration, DateTime inDateCreation, DateTime inDateModification)
        {
            title = inTitle;
            expiration = inExpiration;
            dateCreation = inDateCreation;
            dateModification = inDateModification;
        }

        /// <summary>
        /// Mise à jour de la date de dernière modification du noeud courant
        /// </summary>
        public void UpdateDateModification()
        {
            dateModification = System.DateTime.Now;
        }
    }
}
