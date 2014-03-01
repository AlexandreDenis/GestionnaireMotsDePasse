using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MessageBox = System.Windows.Forms.MessageBox;

namespace Users
{
    public class UsersSerializer
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        public UsersSerializer()
        {

        }

        /// <summary>
        /// Chargement d'une base de données à partir d'un fichier
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public List<Utilisateur> Load()
        {
            List<Utilisateur> users = new List<Utilisateur>();
            string filename = GestionUtilisateurs._usersFile;

            FileStream fs = new FileStream(filename, FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                users = (List<Utilisateur>)formatter.Deserialize(fs);
            }
            catch (SerializationException)
            {
                MessageBox.Show("Echec du chargement de la base des utilisateurs.");
            }
            finally
            {
                fs.Close();
                fs.Dispose();
            }

            return users;
        }

        /// <summary>
        /// Sauvegarde d'une base de données dans un fichier
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        public void Save(List<Utilisateur> users)
        {
            string filename = GestionUtilisateurs._usersFile;

            //création du fichier de sauvegarde
            FileStream fs = new FileStream(filename, FileMode.Create);

            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, users);
            }
            catch (SerializationException)
            {
                MessageBox.Show("Echec de la sauvegarde de la base des utilisateurs.");
            }
            finally
            {
                fs.Close();
                fs.Dispose();
            }
        }
    }
}
