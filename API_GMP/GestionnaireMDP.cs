using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Users;

namespace API_GMP
{
    public class GestionnaireMDP
    {
        private GestionUtilisateurs _gestionUtilisateurs;
        private API_Database _apiDatabase;
        private bool _isXML;

        private bool _isConnected;

        public bool IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; }
        }

        public bool Connexion(string inLogin, string inPassword)
        {
            IsConnected = _gestionUtilisateurs.Connexion(inLogin, inPassword);

            if(IsConnected)
                _apiDatabase = new API_Database(_isXML, _gestionUtilisateurs);

            return IsConnected;
        }

        public GestionnaireMDP(bool isXML)
        {
            _gestionUtilisateurs = new GestionUtilisateurs();
            _isXML = isXML;
        }

        public API_Key GetKey(string inTitle)
        {
            API_Key key = null;

            if(IsConnected)
                key = _apiDatabase.GetKey(inTitle);

            return key;
        }
    }
}
