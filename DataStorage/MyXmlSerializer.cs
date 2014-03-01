using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Forms;
using Microsoft.VisualBasic;

using Datas;
using System.IO;
using System.Runtime.Serialization;

using MessageBox = System.Windows.Forms.MessageBox;
using System.Security.Cryptography;

namespace DataStorage
{
    public class MyXmlSerializer : IDatabaseSerializer
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
        public Database Load(string filename, string key)
        {
            Database db = null;
            string decryptedFilename = filename.Split('.')[0] + "D.xml";
            FileStream fs = null;

            try
            {
                CryptageFichier.FileCrypter.DecryptFile(filename, decryptedFilename, key);

                fs = new FileStream(decryptedFilename, FileMode.Open);

                try
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(Database), "Datas");

                    db = (Database)formatter.Deserialize(fs);

                    fs.Close();
                    fs.Dispose();

                    File.Delete(decryptedFilename);
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
            }
            catch (CryptographicException e)
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }

                File.Delete(decryptedFilename);
                throw e;
            }

            return db;
        }

        /// <summary>
        /// Sauvegarde d'une base de données dans un fichier
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        public void Save(string filename, Database data, string key)
        {
            string encryptedFilename = filename.Split('.')[0] + "C.xml";

            //création du fichier de sauvegarde
            FileStream fs = new FileStream(filename, FileMode.Create);
            try
            {
                System.Xml.Serialization.XmlSerializer formatter = new XmlSerializer(typeof(Database), "Datas");
                formatter.Serialize(fs, data);
            }
            catch (Exception e)
            {
                MessageBox.Show("Echec de la sauvegarde : " + e.Message);
            }
            finally
            {
                fs.Close();
                fs.Dispose();
            }

            fs.Close();
            fs.Dispose();

            //cryptage du fichier en sortie
            CryptageFichier.FileCrypter.EncryptFile(filename,encryptedFilename,key);

            File.Delete(filename);
            FileSystem.Rename(encryptedFilename, filename);
        }
    }
}
