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
    /// Logique d'interaction pour AffichageClé.xaml
    /// </summary>
    public partial class AffichageClé : UserControl
    {
        public event ChangedEventHandler Changed;

        private string oldURL;
        private string oldId;
        private string oldMdp;

        public AffichageClé()
        {
            InitializeComponent();
        }

        protected void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (oldURL != inputUrl.Text || oldId != inputId.Text || oldMdp != inputMdp.Text)
            {
                if (Changed != null)
                    Changed(this, e);
            }
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            oldURL = inputUrl.Text;
            oldId = inputId.Text;
            oldMdp = inputMdp.Text;
        }
    }
}
