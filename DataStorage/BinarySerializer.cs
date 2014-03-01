using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Datas;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Windows.Forms;
using Microsoft.VisualBasic;

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
        public Database Load(string filename, string key)
        {
            Database db = null;
            string decryptedFilename = filename.Split('.')[0] + "D";
            FileStream fs = null;

            try
            {
                CryptageFichier.FileCrypter.DecryptFile(filename, decryptedFilename, key);

                fs = new FileStream(decryptedFilename, FileMode.Open);

                //try
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    fs.Seek(0, SeekOrigin.Begin);
                    db = (Database)formatter.Deserialize(fs);

                    fs.Close();
                    fs.Dispose();

                    File.Delete(decryptedFilename);
                }
                /*catch (SerializationException e)
                {
                    MessageBox.Show("Echec du chargement : " + e.StackTrace);
                }
                finally
                {
                    fs.Close();
                    fs.Dispose();
                }*/
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
            string encryptedFilename = filename.Split('.')[0] + "C";

            //création du fichier de sauvegarde
            FileStream fs = new FileStream(filename, FileMode.Create);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
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
            CryptageFichier.FileCrypter.EncryptFile(filename, encryptedFilename, key);

            File.Delete(filename);
            FileSystem.Rename(encryptedFilename, filename);
        }
    }
}
