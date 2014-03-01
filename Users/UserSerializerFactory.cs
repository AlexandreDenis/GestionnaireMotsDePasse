using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users
{
    class UserSerializerFactory
    {
        private static UsersSerializer _serializer = new UsersSerializer();
        public static UsersSerializer Serializer
        {
            get { return _serializer; }
            set { _serializer = value; }
        }

        /// <summary>
        /// Créé un serializer de base de données
        /// </summary>
        /// <returns></returns>
        public static UsersSerializer Create()
        {
            return Serializer;
        }
    }
}
