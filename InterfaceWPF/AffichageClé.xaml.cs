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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InterfaceWPF
{
    public delegate void ChangedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Logique d'interaction pour AffichageClé.xaml.
    /// Gestion du contrôle utilisateur utilisé pour l'affichage 
    /// et la modification des informations concernant une clé
    /// </summary>
    public partial class AffichageClé : UserControl
    {
        public event ChangedEventHandler Changed;

        //Sauvegarde de l'ancienne valeur des champs pour
        //le traitement de la date de dernière modification
        private string oldURL;
        private string oldId;
        private string oldMdp;

        //Constructeur de la classe AffichageClé
        public AffichageClé()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Appelée quand l'utilisateur "quitte" l'un des champs du User Control
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        protected void OnLostFocus(object sender, RoutedEventArgs e)
        {
            //si une modification a eu lieu
            if (oldURL != inputUrl.Text || oldId != inputId.Text || oldMdp != inputMdp.Text)
            {
                //on appelle l'ensemble des méthodes qui se sont abonnées à l'évènement
                if (Changed != null)
                    Changed(this, e);
            }
        }

        /// <summary>
        /// Appelée quand l'utilisateur "entre" dans l'un des champs du User Control
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            //Sauvegarde de l'ancienne valeur des champs
            oldURL = inputUrl.Text;
            oldId = inputId.Text;
            oldMdp = inputMdp.Text;
        }
    }
}
