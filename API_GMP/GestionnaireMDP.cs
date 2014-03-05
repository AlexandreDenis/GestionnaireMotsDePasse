using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Users;

namespace API_GMP
{
    /// <summary>
    /// Classe principale de l'API permettant notamment de récupérer des clés en fonction de leurs noms
    /// </summary>
    public class GestionnaireMDP
    {
        private GestionUtilisateurs _gestionUtilisateurs;
        private API_Database _apiDatabase;
        private bool _isXML;

        //Permet d'indiquer si utilisateur est authentifié ou pas
        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; }
        }

        /// <summary>
        /// Etablit la connexion d'un utilisateur en chargeant son fichier de database
        /// </summary>
        /// <param name="inLogin">Login de l'utilisateur</param>
        /// <param name="inPassword">Mot de passe de l'utilisateur</param>
        /// <returns>Booléen indiquant si la connexion a fonctionné ou pas</returns>
        public bool Connexion(string inLogin, string inPassword)
        {
            IsConnected = _gestionUtilisateurs.Connexion(inLogin, inPassword);

            if(IsConnected)
                _apiDatabase = new API_Database(_isXML, _gestionUtilisateurs);

            return IsConnected;
        }

        /// <summary>
        /// Constructeur de la classe GestionnaireMDP
        /// </summary>
        /// <param name="isXML">Booléen qui indique si la sérialisation est en XML ou pas (binaire en l'occurrence)</param>
        public GestionnaireMDP(bool isXML)
        {
            _gestionUtilisateurs = new GestionUtilisateurs();
            _isXML = isXML;
        }

        /// <summary>
        /// Récupération d'une clé en fonction de son nom
        /// </summary>
        /// <param name="inTitle">Nom de la clé recherchée</param>
        /// <returns>Clé correspondante ou null</returns>
        public API_Key GetKey(string inTitle)
        {
            API_Key key = null;

            if(IsConnected)
                key = _apiDatabase.GetKey(inTitle);

            return key;
        }
    }
}
