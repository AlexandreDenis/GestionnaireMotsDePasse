using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage
{
    public class DatabaseSerializerFactory
    {
        /// <summary>
        /// Créé un serializer de base de données
        /// </summary>
        /// <returns></returns>
        public static IDatabaseSerializer Create()
        {
            return new BinarySerializer();
        }
    }
}
