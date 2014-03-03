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
using System.Windows.Forms;

using Datas;
using DataStorage;
using Users;

using MessageBox = System.Windows.MessageBox;
using TextBlock = System.Windows.Controls.TextBlock;
using Clipboard = System.Windows.Clipboard;
using System.Security.Cryptography;
using System.IO;
using System.Threading;

namespace InterfaceWPF
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Database _database;
        private GestionUtilisateurs _gestionUtilisateurs;

        /// <summary>
        /// 
        /// </summary>
        public MainWindow(GestionUtilisateurs inGestionUtilisateurs, bool newUser)
        {
            _gestionUtilisateurs = inGestionUtilisateurs;

            //Gestion de la configuration
            CustomConfigurationSection customConfig = CustomConfigurationSection.GetSection();
            string typePersistance = customConfig.typePersistance;

            switch (typePersistance)
            {
                case "xml":
                    DatabaseSerializerFactory.Serializer = new MyXmlSerializer();
                    break;

                case "bin":
                default:
                    DatabaseSerializerFactory.Serializer = new BinarySerializer();
                    break;
            }

            if (newUser || !File.Exists(System.IO.Path.Combine(GestionUtilisateurs._usersDir, _gestionUtilisateurs.UtilisateurCourant.Login)))
            {
                _database = new Database(new Groupe(_gestionUtilisateurs.UtilisateurCourant.Login, null, System.DateTime.Now, System.DateTime.Now, new List<Entry>(), new List<Groupe>()));
                _database.Root.AddGroup(new Groupe("Applications", null, System.DateTime.Now, System.DateTime.Now, new List<Entry>(), new List<Groupe>()));
                _database.Root.AddGroup(new Groupe("Internet", null, System.DateTime.Now, System.DateTime.Now, new List<Entry>(), new List<Groupe>()));
                _database.Root.AddGroup(new Groupe("Machines", null, System.DateTime.Now, System.DateTime.Now, new List<Entry>(), new List<Groupe>()));
            }
            else
            {
                IDatabaseSerializer ids = DatabaseSerializerFactory.Create();
                
                _database = ids.Load(System.IO.Path.Combine(GestionUtilisateurs._usersDir, _gestionUtilisateurs.UtilisateurCourant.Login), _gestionUtilisateurs.UtilisateurCourant.CléDeCryptage);
            }

            InitializeComponent();

            this.Title += " : " + _gestionUtilisateurs.UtilisateurCourant.Login;

            inputNbCarac.Text = Entry.LenghtPassword.ToString();
        }

        private StackPanel createHeader(string inText, bool isFolder)
        {
            StackPanel stackPanel = new StackPanel();
            string imageFileName;

            if (isFolder)
                imageFileName = "Folder.png";
            else
                imageFileName = "Key.png";

            stackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;

            Image image = new Image();
            image.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/InterfaceWPF;component/resources/" + imageFileName));

            TextBlock textBlock = new TextBlock();
            textBlock.Text = inText;

            stackPanel.Children.Add(image);
            stackPanel.Children.Add(textBlock);

            return stackPanel;
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

            item.Header = createHeader(_database.Root.Title, true);

            item.IsExpanded = true;

            RemplirGroupe(_database.Root, item);

            Arborescence.Items.Add(item);
        }

        private void onSelectedItem(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string str = "";

            try
            {
                TreeViewItem item = (TreeViewItem)Arborescence.SelectedItem;
                StackPanel stackPanel = (StackPanel)item.Header;
                TextBlock textBlock = (TextBlock)stackPanel.Children[1];

                if (item != null)
                {
                    if (isClé(item))
                    {
                        Entry entry = RechercherEntry(_database.Root, (string)textBlock.Text);

                        if(entry != null)
                            str = "Création : " + entry.DateCreation + " | Dernière modification : " + entry.DateModification;
                    }
                    else
                    {
                        Groupe groupe = RechercherGroupe(_database.Root, (string)textBlock.Text);

                        if(groupe != null)
                            str = "Création : " + groupe.DateCreation + " | Dernière modification : " + groupe.DateModification;
                    }
                }
            }
            catch (InvalidCastException)
            {

            }

            statusDates.Text = str;
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
                item.Header = createHeader(grp.Title, true);
                item.IsExpanded = true;

                inItem.Items.Add(item);

                RemplirGroupe(grp, item);
            }

            foreach(Entry entry in inGroupe.Entries)
            {
                item = new TreeViewItem();
                item.Header = createHeader(entry.Title, false);

                affKey = new AffichageClé();
                affKey.DataContext = entry;

                item.Items.Add(affKey);

                inItem.Items.Add(item);

                affKey.Changed += onInputTextChanged;
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

            try
            {
                if (Arborescence.SelectedItem != null)
                    father = (TreeViewItem)Arborescence.SelectedItem;
                else
                    father = (TreeViewItem)Arborescence.Items[0];

                if(!isClé(father))
                {
                    string title = Interaction.InputBox("Entrez le nom de la nouvelle clé :", "Nom de la clé");

                    //Recherche de l'élément de la Database correspondant à l'élément père
                    Groupe groupePere = RechercherGroupe(_database.Root, (string)((TextBlock)((StackPanel)father.Header).Children[1]).Text);

                    newItem = new TreeViewItem();
                    newItem.Header = createHeader(title, false);

                    newEntry = new Entry();
                    newEntry.Title = title;
                    if (groupePere != null)
                    {
                        groupePere.Entries.Add(newEntry);
                    }

                    newKey = new AffichageClé();
                    newKey.DataContext = newEntry;
                    newKey.Changed += onInputTextChanged;

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

        void onInputTextChanged(object sender, EventArgs e)
        {
            AffichageClé affKey = (AffichageClé)sender;

            Entry entry = (Entry)affKey.DataContext;

            if (entry != null)
                entry.UpdateDateModification();
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
                        Groupe father = RechercherGroupe(_database.Root, (string)((TextBlock)((StackPanel)((TreeViewItem)item.Parent).Header).Children[1]).Text);

                        if (father != null)
                        {
                            Entry entryToDelete = RechercherEntry(father, (string)((TextBlock)((StackPanel)((TreeViewItem)item).Header).Children[1]).Text);

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

            if (entry == null)
            {
                foreach (Groupe groupe in inGroupe.Groups)
                {
                    entry = RechercherEntry(groupe, inTitle);

                    if (entry != null)
                        break;
                }
            }

            return entry;
        }

        private void Enregistrer()
        {
            SaveFileDialog ofd = new SaveFileDialog();

            DialogResult res = ofd.ShowDialog();

            do
            {
                string filename = ofd.FileName;

                if (filename != "")
                {
                    Thread newThread = new Thread(() => EnregistrerThread(filename));
                    newThread.Start();
                }
                else
                {
                    MessageBox.Show("Problème de sauvegarde du fichier !");
                    break;
                }
            } while (res.ToString() != "OK");
        }

        private void EnregistrerThread(string filename)
        {
            IDatabaseSerializer ids = DatabaseSerializerFactory.Create();
            ids.Save(filename, _database, _gestionUtilisateurs.UtilisateurCourant.CléDeCryptage);
        }

        private void EnregistrerCompte()
        {
            IDatabaseSerializer ids = DatabaseSerializerFactory.Create();
            //ids.Save(_gestionUtilisateurs.UtilisateurCourant.Login, _database, _gestionUtilisateurs.UtilisateurCourant.CléDeCryptage);
            ids.Save(System.IO.Path.Combine(GestionUtilisateurs._usersDir, _gestionUtilisateurs.UtilisateurCourant.Login), _database, _gestionUtilisateurs.UtilisateurCourant.CléDeCryptage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onOuvrirClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            Ouvrir();
        }

        private void Ouvrir()
        {
             OpenFileDialog ofd = new OpenFileDialog();

            DialogResult res = ofd.ShowDialog();

            if (res.ToString() == "OK")
            {
                string filename = ofd.FileName;

                IDatabaseSerializer ids = DatabaseSerializerFactory.Create();

                try
                {
                    _database = ids.Load(filename, _gestionUtilisateurs.UtilisateurCourant.CléDeCryptage);

                    Arborescence.Items.Clear();
                    RemplirArborescence();
                }
                catch (CryptographicException)
                {
                    MessageBox.Show("Vous n'êtes pas autorisé à lire ce fichier.");
                }
            }
            else
                MessageBox.Show("Problème d'ouverture du fichier !");
        }

       private void OuvrirCompte()
        {
            IDatabaseSerializer ids = DatabaseSerializerFactory.Create();
            _database = ids.Load(System.IO.Path.Combine(GestionUtilisateurs._usersDir, _gestionUtilisateurs.UtilisateurCourant.Login), _gestionUtilisateurs.UtilisateurCourant.CléDeCryptage);

            Arborescence.Items.Clear();
            RemplirArborescence();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onQuitterClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        private void GererFermetureProgramme(System.ComponentModel.CancelEventArgs e)
        {
            string messageBoxText = "Souhaitez-vous sauvegarder vos modifications avant de quitter le programme?";
            string caption = "Fermeture du programme";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;
            Connexion connexion;

            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    Thread newThread = new Thread(EnregistrerCompte);
                    newThread.Start();
                    connexion = new Connexion();
                    connexion.Show();
                    break;
                case MessageBoxResult.No:
                    connexion = new Connexion();
                    connexion.Show();
                    break;
                case MessageBoxResult.Cancel:
                    e.Cancel = true;
                    break;
            }
        }

        private void onClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GererFermetureProgramme(e);
        }

        private void onNbCaracChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                int newValue = int.Parse(inputNbCarac.Text);
                Entry.LenghtPassword = newValue;
            }
            catch (FormatException)
            {
                MessageBox.Show("Valeur rentrée incorrecte");
                inputNbCarac.Text = Entry.LenghtPassword.ToString();
            }
        }

        private void onAjouterDossierButtonClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            TreeViewItem father;
            TreeViewItem newItem;
            Groupe newGroupe;

            try
            {
                if (Arborescence.SelectedItem != null)
                    father = (TreeViewItem)Arborescence.SelectedItem;
                else
                    father = (TreeViewItem)Arborescence.Items[0];

                if(!isClé(father))
                {
                    string title = Interaction.InputBox("Entrez le nom de la nouvelle clé :", "Nom de la clé");

                    //Recherche de l'élément de la Database correspondant à l'élément père
                    Groupe groupePere = RechercherGroupe(_database.Root, (string)((TextBlock)((StackPanel)father.Header).Children[1]).Text);

                    newItem = new TreeViewItem();
                    newItem.Header = createHeader(title, true);

                    newGroupe = new Groupe();
                    newGroupe.Title = title;
                    if (groupePere != null)
                    {
                        groupePere.Groups.Add(newGroupe);
                    }

                    father.Items.Add(newItem);
                }
                else
                    MessageBox.Show("Vous ne pouvez ajouter des dossiers qu'à des dossiers.");
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Vous ne pouvez ajouter des dossiers qu'à des dossiers.");
            }
        }

        private void onSupprimerDossierButtonClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                TreeViewItem item = (TreeViewItem)Arborescence.SelectedItem;

                if (Arborescence.SelectedItem != null)
                {
                    if (!isClé(item))
                    {
                        Groupe father = RechercherGroupe(_database.Root, (string)((TextBlock)((StackPanel)((TreeViewItem)item.Parent).Header).Children[1]).Text);

                        if (father != null)
                        {
                            Groupe groupToDelete = RechercherGroupe(father, (string)((TextBlock)((StackPanel)((TreeViewItem)item).Header).Children[1]).Text);

                            father.Groups.Remove(groupToDelete);

                            if (item.Parent is TreeViewItem)
                                (item.Parent as TreeViewItem).Items.Remove(item);
                        }
                    }
                    else
                        MessageBox.Show("Veuillez sélectionner un dossier.");
                }
                else
                    MessageBox.Show("Veuillez sélectionner le dossier à supprimer");
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Veuillez sélectionner un dossier.");
            }
        }

        private void onRenommerClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                TreeViewItem item = (TreeViewItem)Arborescence.SelectedItem;

                if (Arborescence.SelectedItem != null)
                {
                    StackPanel stackPanel = (StackPanel)item.Header;
                    TextBlock textBlock = (TextBlock)stackPanel.Children[1];

                    string title = Interaction.InputBox("Entrez le nouveau nom :", "Nom du noeud");

                    if (!isClé(item))
                    {
                        Groupe groupe = RechercherGroupe(_database.Root, textBlock.Text);
                        if (groupe != null)
                        {
                            groupe.Title = title;
                            groupe.UpdateDateModification();
                        }
                    }
                    else
                    {
                        Entry entry = RechercherEntry(_database.Root, textBlock.Text);
                        if (entry != null)
                        {
                            entry.Title = title;
                            entry.UpdateDateModification();
                        }
                    }

                    textBlock.Text = title;
                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner l'élément à renommer.");
                }
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Veuillez sélectionner l'élément à renommer.");
            }
        }

        private void onAllerURLClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                TreeViewItem item = (TreeViewItem)Arborescence.SelectedItem;

                if (Arborescence.SelectedItem != null)
                {
                    if (isClé(item))
                    {
                        AffichageClé affKey = (AffichageClé)item.Items[0];

                        try
                        {
                            System.Diagnostics.Process.Start(affKey.inputUrl.Text);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("URL inconnue.");
                        }
                    }
                    else
                        MessageBox.Show("Veuillez sélectionner une clé.");
                }
                else
                    MessageBox.Show("Veuillez sélectionner une clé.");
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Veuillez sélectionner une clé.");
            }
        }

        private void onKeyDownHandler(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Arborescence.SelectedItem != null)
            {
                if(Keyboard.Modifiers == ModifierKeys.Control)
                {
                    //Copie de l'identifiant
                    if (e.Key == Key.B)
                    {
                        CopierClé(true);
                    }

                    //Copie du mot de passe
                    if (e.Key == Key.C)
                    {
                        CopierClé(false);
                    }
                }
            }
        }

         private void CopierClé(bool id)
         {
             try
             {
                 TreeViewItem item = (TreeViewItem)Arborescence.SelectedItem;

                 if (isClé(item))
                 {
                     AffichageClé affKey = (AffichageClé)item.Items[0];

                     if (item != null)
                     {
                         string aCopier;

                         if (id)
                         {
                             aCopier = affKey.inputId.Text;
                         }
                         else
                         {
                             aCopier = affKey.inputMdp.Text;
                         }

                         Clipboard.SetData(System.Windows.DataFormats.Text, aCopier);
                     }
                 }
                 else
                 {
                     MessageBox.Show("Veuillez sélectionner une clé.");
                 }
             }
             catch (InvalidCastException)
             {
                 MessageBox.Show("Veuillez sélectionner une clé.");
             }
        }

         private void onEnregistrerClicked(object sender, RoutedEventArgs e)
         {
             Enregistrer();
         }
    }
}
