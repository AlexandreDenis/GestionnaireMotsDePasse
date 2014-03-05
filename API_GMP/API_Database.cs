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
    internal class API_Database
    {
        private Database _database;

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

            if (File.Exists(System.IO.Path.Combine(GestionUtilisateurs._usersDir, inGU.UtilisateurCourant.Login)))
            {
                IDatabaseSerializer ids = DatabaseSerializerFactory.Create();

                _database = ids.Load(System.IO.Path.Combine(GestionUtilisateurs._usersDir, inGU.UtilisateurCourant.Login), inGU.UtilisateurCourant.CléDeCryptage);
            }
            else
                _database = null;
        }

        internal API_Key GetKey(string inTitle)
        {
            return GetKey(inTitle, _database.Root);
        }

        private API_Key GetKey(string inTitle, Groupe inRoot)
        {
            API_Key key = null;

            foreach (Entry entry in inRoot.Entries)
            {
                if (entry.Title == inTitle)
                {
                    key = new API_Key(entry.Title, entry.Username, entry.Password);
                    break;
                }
            }

            if (key == null)
            {
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
