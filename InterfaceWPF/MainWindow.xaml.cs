using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
using System.Windows.Forms;

using Datas;
using DataStorage;

using MessageBox = System.Windows.MessageBox;

namespace InterfaceWPF
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Database _database;

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            _database = new Database(new Groupe("Root", null, new List<Entry>(), new List<Groupe>()));
            _database.Root.AddGroup(new Groupe("Réseau", null, new List<Entry>(), new List<Groupe>()));
            _database.Root.Groups[0].AddEntry(new Entry("LocalUser", null, "localuser", "http://google.fr"));
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateTreeView(object sender, RoutedEventArgs e)
        {
            RemplirArborescence();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RemplirArborescence()
        {
            TreeViewItem item;

            item = new TreeViewItem();
            item.Header = _database.Root.Title;
            item.IsExpanded = true;

            RemplirGroupe(_database.Root, item);

            Arborescence.Items.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inGroupe"></param>
        /// <param name="inItem"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                if(!isClé(father))
                {
                    //Recherche de l'élément de la Database correspondant à l'élément père
                    Groupe groupePere = RechercherGroupe(_database.Root, (string)father.Header);

                    newItem = new TreeViewItem();
                    newItem.Header = title;

                    newEntry = new Entry();
                    newEntry.Title = title;
                    if (groupePere != null)
                    {
                        groupePere.Entries.Add(newEntry);
                    }

                    newKey = new AffichageClé();
                    newKey.DataContext = newEntry;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onSupprimerButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem item = (TreeViewItem)Arborescence.SelectedItem;

                if (Arborescence.SelectedItem != null)
                {
                    if (isClé(item))
                    {
                        Groupe father = RechercherGroupe(_database.Root, (string)((item.Parent as TreeViewItem).Header));

                        if (father != null)
                        {
                            Entry entryToDelete = RechercherEntry(father, (string)item.Header);

                            father.Entries.Remove(entryToDelete);

                            if (item.Parent is TreeViewItem)
                                (item.Parent as TreeViewItem).Items.Remove(item);
                        }
                    }
                    else
                        MessageBox.Show("Veuillez sélectionner une clé.");
                }
                else
                    MessageBox.Show("Veuillez sélectionner la clé à supprimer");
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Veuillez sélectionner une clé.");
            }
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inItem"></param>
        /// <returns></returns>
        private bool isClé(TreeViewItem inItem)
        {
            bool res = false;

            if (inItem.Items.Count != 0)
                if (inItem.Items[0] is AffichageClé)
                    res = true;

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inGroupe"></param>
        /// <param name="inTitle"></param>
        /// <returns></returns>
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

        private Entry RechercherEntry(Groupe inGroupe, string inTitle)
        {
            Entry entry = null;

            foreach (Entry en in inGroupe.Entries)
            {
                if (en.Title == inTitle)
                {
                    entry = en;
                    break;
                }
            }

            return entry;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnregistrerClicked(object sender, RoutedEventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();

            DialogResult res = ofd.ShowDialog();

            if (res.ToString() == "OK")
            {
                string filename = ofd.FileName;

                IDatabaseSerializer ids = DatabaseSerializerFactory.Create();
                ids.Save(filename, _database);
            }
            else
                MessageBox.Show("Problème d'enregistrement du fichier !");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onOuvrirClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            DialogResult res = ofd.ShowDialog();

            if (res.ToString() == "OK")
            {
                string filename = ofd.FileName;

                IDatabaseSerializer ids = DatabaseSerializerFactory.Create();
                _database = ids.Load(filename);

                Arborescence.Items.Clear();
                RemplirArborescence();
            }
            else
                MessageBox.Show("Problème d'ouverture du fichier !");
        }
    }
}
