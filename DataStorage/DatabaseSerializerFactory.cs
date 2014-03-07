using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage
{
    /// <summary>
    /// Factory utilisée pour la création de Sérialiseur/Désérialiseur d'instances de Database
    /// </summary>
    public class DatabaseSerializerFactory
    {
        //Instance unique du sérialiseur/désérialiseur utilisé
        private static IDatabaseSerializer _serializer;
        public static IDatabaseSerializer Serializer
        {
            get { return _serializer; }
            set { _serializer = value; }
        }

        /// <summary>
        /// Créé un sérialiseur/désérialiseur de Database
        /// </summary>
        /// <returns>Instance du sérialiseur/désérialiseur</returns>
        public static IDatabaseSerializer Create()
        {
            return Serializer;
        }
    }
}
