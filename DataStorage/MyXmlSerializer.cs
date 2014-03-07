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
    /// <summary>
    /// Sérialiseur XML de la classe Database
    /// </summary>
    public class MyXmlSerializer : IDatabaseSerializer
    {
        /// <summary>
        /// Constructeur de la classe MyXmlSerializer
        /// </summary>
        public MyXmlSerializer()
        {

        }

        /// <summary>
        /// Chargement d'une Database à partir d'un fichier XML
        /// </summary>
        /// <param name="filename">Nom du fichier à charger</param>
        /// <param name="key">Clé utilisée pour le décryptage du fichier à désérialiser</param>
        /// <returns>Instance de la Database résultant de la désérialisation du fichier</returns>
        public Database Load(string filename, string key)
        {
            Database db = null;
            string decryptedFilename = filename.Split('.')[0] + "D.xml";
            FileStream fs = null;

            try
            {
                //Décryptage du fichier
                CryptageFichier.FileCrypter.DecryptFile(filename, decryptedFilename, key);

                fs = new FileStream(decryptedFilename, FileMode.Open);

                try
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(Database), "Datas");

                    //Désérialisation du fichier
                    db = (Database)formatter.Deserialize(fs);
                    
                    //Libération du flux utilisé pour la désérialisation
                    fs.Close();
                    fs.Dispose();

                    //Suppression du fichier intermédiaire utilisé pour le décryptage
                    File.Delete(decryptedFilename);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Echec du chargement : " + e.Message);
                }
                finally
                {
                    //Libération du flux utilisé pour la désérialisation
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (CryptographicException e)
            {
                if (fs != null)
                {
                    //Libération du flux utilisé pour la désérialisation
                    fs.Close();
                    fs.Dispose();
                }

                //Suppression du fichier intermédiaire utilisé pour le décryptage
                File.Delete(decryptedFilename);
                throw e;
            }

            return db;
        }

        /// <summary>
        /// Sauvegarde d'une Database dans un fichier XML
        /// </summary>
        /// <param name="filename">Nom du fichier en sortie de la sérialisation</param>
        /// <param name="data">Instance de la Database à sérialiser</param>
        /// <param name="key">Clé utilisée pour l'encryptage du fichier résultant de la sérialisation</param>
        public void Save(string filename, Database data, string key)
        {
            string encryptedFilename = filename.Split('.')[0] + "C.xml";

            //Création du fichier de sauvegarde
            FileStream fs = new FileStream(filename, FileMode.Create);
            try
            {
                //Sérialisation de la Database
                System.Xml.Serialization.XmlSerializer formatter = new XmlSerializer(typeof(Database), "Datas");
                formatter.Serialize(fs, data);
            }
            catch (Exception e)
            {
                MessageBox.Show("Echec de la sauvegarde : " + e.Message);
            }
            finally
            {
                //Libération du flux utilisé pour la sérialisation
                fs.Close();
                fs.Dispose();
            }

            //Libération du flux utilisé pour la sérialisation
            fs.Close();
            fs.Dispose();

            //Cryptage du fichier en sortie
            CryptageFichier.FileCrypter.EncryptFile(filename,encryptedFilename,key);

            //Suppression du fichier intermédiaire utilisé pour l'encryptage
            File.Delete(filename);
            FileSystem.Rename(encryptedFilename, filename);
        }
    }
}
