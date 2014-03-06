using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users
{
    /// <summary>
    /// Classe chargée de la création de sérialiseurs/désérialiseurs 
    /// pour la liste des utilisateurs du programme
    /// </summary>
    class UserSerializerFactory
    {
        /// <summary>
        /// Instance du sérialiseur/désérialiseur
        /// </summary>
        private static UsersSerializer _serializer = new UsersSerializer();
        public static UsersSerializer Serializer
        {
            get { return _serializer; }
            set { _serializer = value; }
        }

        /// <summary>
        /// Renvoie l'instance du sérialiseur/désérialiseur
        /// </summary>
        /// <returns>Instance du sérialiseur/désérialiseur</returns>
        public static UsersSerializer Create()
        {
            return Serializer;
        }
    }
}
