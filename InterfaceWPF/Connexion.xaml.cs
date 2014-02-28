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
    /// Logique d'interaction pour Connexion.xaml
    /// </summary>
    public partial class Connexion : Window
    {
        private GestionUtilisateurs _gestionUtilisateurs;

        public Connexion()
        {
            _gestionUtilisateurs = new GestionUtilisateurs();
            InitializeComponent();
        }

        private void onClickInscriptionButton(object sender, RoutedEventArgs e)
        {
            string loginEntré = loginWPF.Text;
            string passwordEntré = mdpWPF.Password;

            if (loginEntré == "" || passwordEntré == "")
            {
                MessageBox.Show("Veuillez renseigner login ET mot de passe.");
            }
            else
            {
                if (_gestionUtilisateurs.isAlreadyExisting(loginEntré))
                {
                    MessageBox.Show("Ce login est déjà utilisé par un autre utilisateur.");
                }
                else
                {
                    _gestionUtilisateurs.AjouterNouvelUtilisateur(loginEntré, passwordEntré);
                    _gestionUtilisateurs.Connexion(loginEntré, passwordEntré);
                    openMainWindow();
                }
            }
        }

        private void onClickConnexionButton(object sender, RoutedEventArgs e)
        {
            string loginEntré = loginWPF.Text;
            string passwordEntré = mdpWPF.Password;

            if (loginEntré == "" || passwordEntré == "")
            {
                MessageBox.Show("Veuillez renseigner login ET mot de passe.");
            }
            else
            {
                if (_gestionUtilisateurs.Connexion(loginEntré, passwordEntré))
                {
                    openMainWindow();
                }
                else
                {
                    MessageBox.Show("Login/Mot de passe incorrect !");
                    loginWPF.Clear();
                    mdpWPF.Clear();
                    loginWPF.Focus();
                }
            }
        }

        private void openMainWindow()
        {
            MainWindow win = new MainWindow(loginWPF.Text.ToLower());
            win.Show();

            this.Close();
        }

        private void onKeyDownHandler(object sender, KeyEventArgs e)
        {

        }

        private void onClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _gestionUtilisateurs.Save();
        }
    }
}
