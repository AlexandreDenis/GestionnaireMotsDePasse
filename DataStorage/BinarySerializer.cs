using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Datas;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace DataStorage
{
    public class BinarySerializer : IDatabaseSerializer
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        public BinarySerializer()
        {

        }

        /// <summary>
        /// Chargement d'une base de données à partir d'un fichier
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public Database Load(string filename)
        {
            Database db = null;

            FileStream fs = new FileStream(filename, FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                db = (Database)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Echec du chargement : " + e.Message);
            }
            finally
            {
                fs.Close();
            }

            return db;
        }

        /// <summary>
        /// Sauvegarde d'une base de données dans un fichier
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        public void Save(string filename, Database data)
        {
            //création du fichier de sauvegarde
            FileStream fs = new FileStream(filename, FileMode.Create);

            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, data);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Echec de la sauvegarde : " + e.Message);
            }
            finally
            {
                fs.Close();
            }
        }
    }
}
