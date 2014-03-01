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

        protected void OnChanged(object sender, RoutedEventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        public AffichageClé()
        {
            InitializeComponent();
        }
    }
}
