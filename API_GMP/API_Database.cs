using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Users;
using DataStorage;
using Datas;
using System.IO;

namespace API_GMP
{
    /// <summary>
    /// Interface de l'API avec la classe Database afin de l'utiliser comme une boîte noire
    /// </summary>
    internal class API_Database
    {
        private Database _database;

        /// <summary>
        /// Constructeur de la classe API_Database
        /// </summary>
        /// <param name="isXML">Booléen qui indique si la sérialisation est en XML ou pas (binaire en l'occurrence)</param>
        /// <param name="inGU">Instance utilisée pour la gestion des utilisateurs</param>
        internal API_Database(bool isXML, GestionUtilisateurs inGU)
        {

            //Gestion de la sérialisation des données
            if(isXML)
            {
                DatabaseSerializerFactory.Serializer = new MyXmlSerializer();
            }
            else
            {
                DatabaseSerializerFactory.Serializer = new BinarySerializer();
            }

            //si un fichier de sauvegarde du compte utilisateur connecté existe
            if (File.Exists(System.IO.Path.Combine(GestionUtilisateurs._usersDir, inGU.UtilisateurCourant.Login)))
            {
                //on charge la database à partir de ce fichier
                IDatabaseSerializer ids = DatabaseSerializerFactory.Create();

                _database = ids.Load(System.IO.Path.Combine(GestionUtilisateurs._usersDir, inGU.UtilisateurCourant.Login), inGU.UtilisateurCourant.CléDeCryptage);
            }
            //sinon
            else
                _database = null;
        }

        /// <summary>
        /// Retourne la clé correspondante au titre passée en paramètre
        /// </summary>
        /// <param name="inTitle">Nom de la clé recherchée</param>
        /// <returns>L'instance de la clé ou null</returns>
        internal API_Key GetKey(string inTitle)
        {
            return GetKey(inTitle, _database.Root);
        }

        /// <summary>
        /// Fonction utilisée pour recherche une clé dans la database
        /// </summary>
        /// <param name="inTitle">Nom de la clé recherchée</param>
        /// <param name="inRoot">Dossier à partir duquel on recherche la clé</param>
        /// <returns>L'instance de la clé ou null</returns>
        private API_Key GetKey(string inTitle, Groupe inRoot)
        {
            API_Key key = null;

            //Pour chaque clé du dossier racine passé en paramètre
            foreach (Entry entry in inRoot.Entries)
            {
                //si on a trouvé la clé recherchée
                if (entry.Title == inTitle)
                {
                    //on la récupère
                    key = new API_Key(entry.Title, entry.Username, entry.Password);
                    break;
                }
            }

            //si la clé n'a pas été trouvé dans le dossier courant
            if (key == null)
            {
                //on recherche dans les sous-répertoires du répertoire courant
                foreach (Groupe groupe in inRoot.Groups)
                {
                    key = GetKey(inTitle, groupe);

                    if (key != null)
                        break;
                }
            }

            return key;
        }
    }
}
