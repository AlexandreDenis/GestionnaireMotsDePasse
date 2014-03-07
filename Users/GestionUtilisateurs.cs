using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MessageBox = System.Windows.Forms.MessageBox;

namespace Users
{
    /// <summary>
    /// Classe de gestion des utilisateurs
    /// </summary>
    public class GestionUtilisateurs
    {
        //chemin de répertoires où sont stockés les fichiers manipulés par le programme
        public static string _usersDir = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), "GestionnaireMotsDePasse");
        
        //Chemin du fichier où est stocké la base des utilisateurs du programme
        public static string _usersFile = Path.Combine(_usersDir, "users.bin");

        private List<Utilisateur> _listeUtilisateurs;

        /// <summary>
        /// Utilisateur courant du programme
        /// </summary>
        private Utilisateur _utilisateurCourant;
        public Utilisateur UtilisateurCourant
        {
            get { return _utilisateurCourant; }
            set { _utilisateurCourant = value; }
        }

        /// <summary>
        /// Constructeur de la classe GestionUtilisateurs
        /// </summary>
        public GestionUtilisateurs()
        {
            _listeUtilisateurs = new List<Utilisateur>();

            //si le répertoire où doivent être stockés les fichiers n'existe pas
            if (!Directory.Exists(_usersDir))
                Directory.CreateDirectory(_usersDir);       //on le créé

            //chargement de la liste des utilisateurs à partir du fichier correspondant
            if (File.Exists(_usersFile))
            {
                _listeUtilisateurs = UserSerializerFactory.Create().Load();
            }
        }

        /// <summary>
        /// Fonction dont le but est de déterminer si un utilisateur est 
        /// présent dans la base à partir de son login
        /// </summary>
        /// <param name="inLogin"></param>
        /// <returns></returns>
        public bool isAlreadyExisting(string inLogin)
        {
            bool res = false;

            //Pour chaque utilisateur
            foreach (Utilisateur user in _listeUtilisateurs)
            {
                //si un utilisateur de ce nom existe
                if (user.Login == inLogin)
                {
                    res = true;
                    break;
                }
            }

            return res;
        }

        /// <summary>
        /// Fonction d'ajout d'un nouvel utilisateur dans la base
        /// </summary>
        /// <param name="inLogin">Login du nouvel utilisateur</param>
        /// <param name="inPassword">Mot de passe du nouvel utilisateur</param>
        public void AjouterNouvelUtilisateur(string inLogin, string inPassword)
        {
            //on ajoute le nouvel utilisateur à la liste en lui attribuant une clé générée aléatoirement
            _listeUtilisateurs.Add(new Utilisateur(inLogin, inPassword, CryptageFichier.KeyGenerator.GenerateKey()));
        }

        /// <summary>
        /// Sauvegarde de la liste des utilisateurs dans un fichier
        /// </summary>
        public void Save()
        {
            UserSerializerFactory.Create().Save(_listeUtilisateurs);
        }

        /// <summary>
        /// Connexion d'un utilisateur
        /// </summary>
        /// <param name="inLogin">Login de l'utilisateur qui cherche à se connecter</param>
        /// <param name="inPassword">Mot de passe de l'utilisateur qui cherche à se connecter</param>
        /// <returns>true si la connexion a fonctionné, false sinon</returns>
        public bool Connexion(string inLogin, string inPassword)
        {
            bool res = false;

            //si un utilisateur de ce nom (login) existe dans la base
            if (isAlreadyExisting(inLogin))
            {
                //on recherche l'utilisateur correspondant dans la liste
                foreach (Utilisateur user in _listeUtilisateurs)
                {
                    if (user.Login == inLogin)
                    {
                        _utilisateurCourant = user;
                        break;
                    }
                }

                //si le mot de passe saisi correspond au mot de passe de l'utilisateur de ce login
                if (_utilisateurCourant.Password == inPassword)
                    res = true; //on établit la connexion
            }

            return res;
        }
    }
}
