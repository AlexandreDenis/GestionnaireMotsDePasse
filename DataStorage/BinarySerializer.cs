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
    /// <summary>
    /// Sérialiseur binaire de la classe Database
    /// </summary>
    public class BinarySerializer : IDatabaseSerializer
    {
        /// <summary>
        /// Constructeur de la classe BinarySerializer
        /// </summary>
        public BinarySerializer()
        {

        }

        /// <summary>
        /// Chargement d'une Database à partir d'un fichier binaire
        /// </summary>
        /// <param name="filename">Nom du fichier à charger</param>
        /// <param name="key">Paramètre non utilisé pour la sérialisation binaire</param>
        /// <returns>Instance de la Database résultant de la désérialisation du fichier</returns>
        public Database Load(string filename, string key)
        {
            Database db = null;
            FileStream fs = null;

            fs = new FileStream(filename, FileMode.Open);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                //Désérialisation du fichier en entrée
                db = (Database)formatter.Deserialize(fs);

                fs.Close();
                fs.Dispose();
            }
            catch (SerializationException e)
            {
                MessageBox.Show("Echec du chargement : " + e.StackTrace);
            }
            finally
            {
                //Libération du flux utilisé
                fs.Close();
                fs.Dispose();
            }

            return db;
        }

        /// <summary>
        /// Sauvegarde d'une Database dans un fichier binaire
        /// </summary>
        /// <param name="filename">Nom du fichier en sortie de la sérialisation</param>
        /// <param name="data">Instance de la Database à sérialiser</param>
        /// <param name="key">Paramètre non utilisé pour la sérialisation binaire</param>
        public void Save(string filename, Database data, string key)
        {
            //création du fichier de sauvegarde
            FileStream fs = new FileStream(filename, FileMode.Create);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                //Sérialisation de la Database
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
        }
    }
}
