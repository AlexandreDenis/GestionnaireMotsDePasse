using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Users;

namespace InterfaceWPF
{
    /// <summary>
    /// Logique d'interaction pour Connexion.xaml.
    /// Gestion de la fenêtre de connexion/inscription
    /// d'un utilisateur
    /// </summary>
    public partial class Connexion : Window
    {
        private GestionUtilisateurs _gestionUtilisateurs;   //gestion des utilisateurs

        /// <summary>
        /// Constructeur de la classe Connexion
        /// </summary>
        public Connexion()
        {
            _gestionUtilisateurs = new GestionUtilisateurs();
            InitializeComponent();

            //Curseur directement positionné sur le champs de login
            loginWPF.Focus();
        }

        /// <summary>
        /// Appelé lors du click sur le bouton d'inscription
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void onClickInscriptionButton(object sender, RoutedEventArgs e)
        {
            //récupération du login et du mot de passe saisis
            string loginEntré = loginWPF.Text;
            string passwordEntré = mdpWPF.Password;

            //si au moins l'un des deux n'a pas été renseigné
            if (loginEntré == "" || passwordEntré == "")
            {
                MessageBox.Show("Veuillez renseigner login ET mot de passe.");
            }
            //sinon
            else
            {
                //si le login est déjà utilisé par un autre utilisateur
                if (_gestionUtilisateurs.isAlreadyExisting(loginEntré))
                {
                    MessageBox.Show("Ce login est déjà utilisé par un autre utilisateur.");
                }
                //sinon
                else
                {
                    //Ajout et connexion du nouvel utilisateur
                    _gestionUtilisateurs.AjouterNouvelUtilisateur(loginEntré, passwordEntré);
                    _gestionUtilisateurs.Connexion(loginEntré, passwordEntré);
                    openMainWindow(true);
                }
            }
        }

        /// <summary>
        /// Appelé lors du click sur bouton de connexion
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void onClickConnexionButton(object sender, RoutedEventArgs e)
        {
            //récupération du login et du mot de passe saisis
            string loginEntré = loginWPF.Text;
            string passwordEntré = mdpWPF.Password;

            //si au moins l'un des deux n'a pas été renseigné
            if (loginEntré == "" || passwordEntré == "")
            {
                MessageBox.Show("Veuillez renseigner login ET mot de passe.");
            }
            //sinon
            else
            {
                //si la connexion de l'utilisateur a fonctionné
                if (_gestionUtilisateurs.Connexion(loginEntré, passwordEntré))
                {
                    openMainWindow(false);
                }
                //sinon
                else
                {
                    MessageBox.Show("Login/Mot de passe incorrect !");
                    loginWPF.Clear();
                    mdpWPF.Clear();
                    loginWPF.Focus();
                }
            }
        }

        /// <summary>
        /// Ouverture de la fenêtre principale
        /// </summary>
        /// <param name="newUser">Booléen indiquant s'il s'agit d'un nouvel utilisateur ou pas</param>
        private void openMainWindow(bool newUser)
        {
            //ouverture de la fenêtre principale
            MainWindow win = new MainWindow(_gestionUtilisateurs, newUser);
            win.Show();

            //fermeture de la fenêtre de connexion
            this.Close();
        }

        /// <summary>
        /// Appelé lors de l'appui sur une touche du clavier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onKeyDownHandler(object sender, KeyEventArgs e)
        {
            //si appui sur la touche Entrée
            if (e.Key == Key.Enter)
                onClickConnexionButton(sender, e);  //simule un appui sur le bouton de connexion
        }

        /// <summary>
        /// A le fermeture de la fenêtre et donc du programme
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void onClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //on sauvegarde la base d'utilisateurs
            _gestionUtilisateurs.Save();
        }
    }
}
