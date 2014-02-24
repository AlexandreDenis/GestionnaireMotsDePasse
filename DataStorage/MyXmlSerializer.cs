using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Forms;

using Datas;
using System.IO;
using System.Runtime.Serialization;

using MessageBox = System.Windows.Forms.MessageBox;

namespace DataStorage
{
    class MyXmlSerializer : IDatabaseSerializer
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        public MyXmlSerializer()
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
                XmlSerializer formatter = new XmlSerializer(typeof(Database), "Datas");

                db = (Database)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Echec du chargement : " + e.Message);
            }
            finally
            {
                fs.Close();
                fs.Dispose();
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
            //try
            {
                System.Xml.Serialization.XmlSerializer formatter = new XmlSerializer(typeof(Database), "Datas");
                formatter.Serialize(fs, data);
            }
            /*catch (Exception e)
            {
                MessageBox.Show("Echec de la sauvegarde : " + e.StackTrace);
            }*/
           // finally
            {
                fs.Close();
                fs.Dispose();
            }
        }
    }
}
