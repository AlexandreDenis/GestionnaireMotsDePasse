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
    /// <summary>
    /// Sérialiseur/Désérialiseur de la liste des utilisateurs
    /// </summary>
    public class UsersSerializer
    {
        /// <summary>
        /// Constructeur de la classe UsersSerializer
        /// </summary>
        public UsersSerializer()
        {

        }

        /// <summary>
        /// Chargement de la liste des utilisateurs à partir d'un fichier
        /// </summary>
        /// <returns>Liste des utilisateurs résultant de la désérialisation</returns>
        public List<Utilisateur> Load()
        {
            List<Utilisateur> users = new List<Utilisateur>();
            string filename = GestionUtilisateurs._usersFile;

            //ouverture du fichier à désérialiser
            FileStream fs = new FileStream(filename, FileMode.Open);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                //Désérialisation du fichier
                users = (List<Utilisateur>)formatter.Deserialize(fs);
            }
            catch (SerializationException)
            {
                MessageBox.Show("Echec du chargement de la base des utilisateurs.");
            }
            finally
            {
                //Libération du flux utilisé pour la désérialisation
                fs.Close();
                fs.Dispose();
            }

            return users;
        }

        /// <summary>
        /// Sauvegarde d'une liste d'utilisateurs dans un fichier
        /// </summary>
        /// <param name="users">Liste des utilisateurs à sérialiser</param>
        public void Save(List<Utilisateur> users)
        {
            string filename = GestionUtilisateurs._usersFile;

            //création du fichier de sauvegarde
            FileStream fs = new FileStream(filename, FileMode.Create);

            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                //Sérialisation de la liste des utilisateurs dans le fichier
                formatter.Serialize(fs, users);
            }
            catch (SerializationException)
            {
                MessageBox.Show("Echec de la sauvegarde de la base des utilisateurs.");
            }
            finally
            {
                //Libération du flux utilisé pour la sérialisation
                fs.Close();
                fs.Dispose();
            }
        }
    }
}
