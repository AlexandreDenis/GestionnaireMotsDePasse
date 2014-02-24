using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage
{
    public class DatabaseSerializerFactory
    {
        private static IDatabaseSerializer _serializer;
        public static IDatabaseSerializer Serializer
        {
            get { return _serializer; }
            set { _serializer = value; }
        }

        /// <summary>
        /// Créé un serializer de base de données
        /// </summary>
        /// <returns></returns>
        public static IDatabaseSerializer Create()
        {
            return Serializer;
        }
    }
}
