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
using Microsoft.VisualBasic;

using Datas;
//Test
namespace InterfaceWPF
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Database _database;

        public MainWindow()
        {
            _database = new Database(new Groupe());
            _database.Root.AddGroup(new Groupe("Réseau", null, new List<Entry>(), new List<Groupe>()));
            _database.Root.Groups[0].AddEntry(new Entry("LocalUser", null, "localuser", "http://google.fr"));
            InitializeComponent();
        }

        private void CreateTreeView(object sender, RoutedEventArgs e)
        {
            TreeViewItem item;

            item = new TreeViewItem();
            item.Header = "Root";
            item.IsExpanded = true;

            RemplirGroupe(_database.Root, item);

            Arborescence.Items.Add(item);
            
        }

        private void RemplirGroupe(Groupe inGroupe, TreeViewItem inItem)
        {
            TreeViewItem item;
            AffichageClé affKey;

            foreach(Groupe grp in inGroupe.Groups)
            {
                item = new TreeViewItem();
                item.Header = grp.Title;
                item.IsExpanded = true;

                inItem.Items.Add(item);

                RemplirGroupe(grp, item);
            }

            foreach(Entry entry in inGroupe.Entries)
            {
                item = new TreeViewItem();
                item.Header = entry.Title;

                affKey = new AffichageClé();
                affKey.DataContext = entry;

                item.Items.Add(affKey);

                inItem.Items.Add(item);
            }
        }

        private void onAjouterButtonClicked(object sender, RoutedEventArgs e)
        {
            TreeViewItem father;
            TreeViewItem newItem;
            AffichageClé newKey;
            Entry newEntry;

            string title = Interaction.InputBox("Entrez le nom de la nouvelle clé :", "Nom de la clé");

            try
            {
                if (Arborescence.SelectedItem != null)
                    father = (TreeViewItem)Arborescence.SelectedItem;
                else
                    father = (TreeViewItem)Arborescence.Items[0];

                if(father.Items.Count != 0)
                    if (!(father.Items[0] is AffichageClé))
                    {
                        //Recherche de l'élément de la Database correspondant à l'élément père
                        Groupe groupePere = RechercherGroupe(_database.Root, (string)father.Header);

                        newItem = new TreeViewItem();
                        newItem.Header = title;

                        newEntry = new Entry();
                        if(groupePere != null)
                            groupePere.Entries.Add(newEntry);

                        newKey = new AffichageClé();
                        newKey.DataContext = newKey;

                        newItem.Items.Add(newKey);

                        father.Items.Add(newItem);
                    }
                    else
                        MessageBox.Show("Vous ne pouvez ajouter des clés qu'à des dossiers.");
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Vous ne pouvez ajouter des clés qu'à des dossiers.");
            }
        }

        private Groupe RechercherGroupe(Groupe inGroupe, string inTitle)
        {
            Groupe res = null;

            if (inGroupe != null)
            {
                if (inGroupe.Title != inTitle)
                {
                    foreach (Groupe grp in inGroupe.Groups)
                    {
                        res = RechercherGroupe(grp,inTitle);
                        if (res != null)
                            break;
                    }
                }
                else
                    res = inGroupe;
            }

            return res;
        }
    }
}
