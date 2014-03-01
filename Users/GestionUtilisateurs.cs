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
    public class GestionUtilisateurs
    {
        internal const string _usersFile = "users.bin";

        private List<Utilisateur> _listeUtilisateurs;

        private Utilisateur _utilisateurCourant;
        public Utilisateur UtilisateurCourant
        {
            get { return _utilisateurCourant; }
            set { _utilisateurCourant = value; }
        }

        public GestionUtilisateurs()
        {
            _listeUtilisateurs = new List<Utilisateur>();

            //chargement de la liste des utilisateurs
            if (File.Exists(_usersFile))
            {
                _listeUtilisateurs = UserSerializerFactory.Create().Load();
            }
        }

        public bool isAlreadyExisting(string inLogin)
        {
            bool res = false;

            foreach (Utilisateur user in _listeUtilisateurs)
            {
                if (user.Login == inLogin)
                {
                    res = true;
                    break;
                }
            }

            return res;
        }

        public void AjouterNouvelUtilisateur(string inLogin, string inPassword)
        {
            _listeUtilisateurs.Add(new Utilisateur(inLogin, inPassword, CryptageFichier.KeyGenerator.GenerateKey()));
        }

        public void Save()
        {
            UserSerializerFactory.Create().Save(_listeUtilisateurs);
        }

        public bool Connexion(string inLogin, string inPassword)
        {
            bool res = false;

            if (isAlreadyExisting(inLogin))
            {
                foreach (Utilisateur user in _listeUtilisateurs)
                {
                    if (user.Login == inLogin)
                    {
                        _utilisateurCourant = user;
                        break;
                    }
                }

                if (_utilisateurCourant.Password == inPassword)
                    res = true;
            }

            return res;
        }
    }
}
