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
using System.Security.Cryptography;
using System.IO;
using System.Threading;

using Datas;
using DataStorage;
using Users;

using MessageBox = System.Windows.MessageBox;
using TextBlock = System.Windows.Controls.TextBlock;
using Clipboard = System.Windows.Clipboard;

namespace InterfaceWPF
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml.
    /// Gestion de la fenêtre principale du programme
    /// </summary>
    public partial class MainWindow : Window
    {
        private Database _database;         //Database de l'utilisateur connecté
        private GestionUtilisateurs _gestionUtilisateurs;   //Gestion des utilisateurs

        /// <summary>
        /// Constructeur de la classe MainWindow
        /// </summary>
        /// <param name="inGestionUtilisateurs">Gestion des utilisateurs passée par la fenêtre de connexion</param>
        /// <param name="newUser">Booléen indiquant s'il s'agit d'un nouvel utilisateur ou pas</param>
        public MainWindow(GestionUtilisateurs inGestionUtilisateurs, bool newUser)
        {
            _gestionUtilisateurs = inGestionUtilisateurs;

            //Gestion de la configuration
            CustomConfigurationSection customConfig = CustomConfigurationSection.GetSection();
            string typePersistance = customConfig.typePersistance;

            switch (typePersistance)
            {
                case "xml": //Sérialisation/désérialisation XML
                    DatabaseSerializerFactory.Serializer = new MyXmlSerializer();
                    break;

                case "bin": //Sérialisation/désérialisation binaire
                default:
                    DatabaseSerializerFactory.Serializer = new BinarySerializer();
                    break;
            }

            //s'il s'agit d'un nouvel utilisateur ou qu'aucun fichier de sauvegarde de l'utilisateur connecté existe
            if (newUser || !File.Exists(System.IO.Path.Combine(GestionUtilisateurs._usersDir, _gestionUtilisateurs.UtilisateurCourant.Login)))
            {
                //Création de l'arborescence de base
                _database = new Database(new Groupe(_gestionUtilisateurs.UtilisateurCourant.Login, null, System.DateTime.Now, System.DateTime.Now, new List<Entry>(), new List<Groupe>()));
                _database.Root.AddGroup(new Groupe("Applications", null, System.DateTime.Now, System.DateTime.Now, new List<Entry>(), new List<Groupe>()));
                _database.Root.AddGroup(new Groupe("Internet", null, System.DateTime.Now, System.DateTime.Now, new List<Entry>(), new List<Groupe>()));
                _database.Root.AddGroup(new Groupe("Machines", null, System.DateTime.Now, System.DateTime.Now, new List<Entry>(), new List<Groupe>()));
            }
            //sinon
            else
            {
                //Chargement du fichier de sauvegarde de l'utilisateur connecté
                IDatabaseSerializer ids = DatabaseSerializerFactory.Create();
                
                _database = ids.Load(System.IO.Path.Combine(GestionUtilisateurs._usersDir, _gestionUtilisateurs.UtilisateurCourant.Login), _gestionUtilisateurs.UtilisateurCourant.CléDeCryptage);
            }

            //Récupération des préférences utilisateurs concernant la génération des mots de passe
            Entry.LenghtPassword = _database.LenghtPassword;
            Entry.NbCaracSpec = _database.NbCaracSpec;

            InitializeComponent();

            //Affichage du login de l'utilisateur connecté dans le titre de la fenêtre principale
            this.Title += " : " + _gestionUtilisateurs.UtilisateurCourant.Login;

            inputNbCarac.DataContext = _database;
            inputNbCaracSpec.DataContext = _database;
        }

        /// <summary>
        /// Création des headers pour l'affichage de l'arborescence graphique des dossiers et clés
        /// </summary>
        /// <param name="inText">Texte à afficher</param>
        /// <param name="isFolder">Booléen indiquant s'il s'agit d'un dossier ou pas</param>
        /// <returns>Panel comprenant l'ensemble des éléments graphiques du header</returns>
        private StackPanel createHeader(string inText, bool isFolder)
        {
            StackPanel stackPanel = new StackPanel();
            string imageFileName;

            //Image différente selon que ce soit une clé ou un dossier
            if (isFolder)
                imageFileName = "Folder.png";
            else
                imageFileName = "Key.png";

            stackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;

            //chargement de l'image correspondante
            Image image = new Image();
            image.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/InterfaceWPF;component/resources/" + imageFileName));

            //chargement du texte correspondant
            TextBlock textBlock = new TextBlock();
            textBlock.Text = inText;

            //Placement des éléments dans le panel
            stackPanel.Children.Add(image);
            stackPanel.Children.Add(textBlock);

            return stackPanel;
        }

        /// <summary>
        /// Création de l'arborescence
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void CreateTreeView(object sender, RoutedEventArgs e)
        {
            //Remplissage de l'arborescence
            RemplirArborescence();
        }

        /// <summary>
        /// Remplissage de l'arborescence via le Database de l'utilisateur connecté
        /// </summary>
        private void RemplirArborescence()
        {
            TreeViewItem item;

            //Création de la racine
            item = new TreeViewItem();
            item.Header = createHeader(_database.Root.Title, true);
            item.IsExpanded = true;

            //Remplissage de l'arborescence
            RemplirGroupe(_database.Root, item);

            //Ajout de la racine à l'arborescence
            Arborescence.Items.Add(item);
        }

        /// <summary>
        /// Lorsqu'un noeud (clé ou dossier) est sélectionné par l'utilisateur
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void onSelectedItem(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string str = "";

            try
            {
                //Récupération des informations sur le noeud sélectionné
                TreeViewItem item = (TreeViewItem)Arborescence.SelectedItem;
                StackPanel stackPanel = (StackPanel)item.Header;
                TextBlock textBlock = (TextBlock)stackPanel.Children[1];

                if (item != null)
                {
                    //si le noeud sélectionné est une clé
                    if (isClé(item))
                    {
                        //on recherche la clé correspondante dans la Database
                        Entry entry = RechercherEntry(_database.Root, (string)textBlock.Text);

                        //si on l'a trouvé
                        if(entry != null)
                            str = "Création : " + entry.DateCreation + " | Dernière modification : " + entry.DateModification;
                    }
                    //sinon, le noeud sélectionné est alors un dossier
                    else
                    {
                        //on recherche le groupe/dossier correspondant dans la Database
                        Groupe groupe = RechercherGroupe(_database.Root, (string)textBlock.Text);

                        //si on l'a trouvé
                        if(groupe != null)
                            str = "Création : " + groupe.DateCreation + " | Dernière modification : " + groupe.DateModification;
                    }
                }
            }
            catch (InvalidCastException)
            {

            }

            //on affiche les dates de création et de dernière modification dans le bandeau de statut de la fenêtre
            statusDates.Text = str;
        }

        /// <summary>
        /// Remplissage d'un groupe
        /// </summary>
        /// <param name="inGroupe">Groupe à remplir</param>
        /// <param name="inItem">Elément de l'arborescence graphique correspondant</param>
        private void RemplirGroupe(Groupe inGroupe, TreeViewItem inItem)
        {
            TreeViewItem item;
            AffichageClé affKey;

            //Pour chaque sous-dossier
            foreach(Groupe grp in inGroupe.Groups)
            {
                //on créé un noeud
                item = new TreeViewItem();
                item.Header = createHeader(grp.Title, true);
                item.IsExpanded = true;

                //on ajoute le sous-dossier à l'arborescence
                inItem.Items.Add(item);

                //on remplit le sous-projet
                RemplirGroupe(grp, item);
            }

            //Pour chaque clé
            foreach(Entry entry in inGroupe.Entries)
            {
                //on créé un noeud
                item = new TreeViewItem();
                item.Header = createHeader(entry.Title, false);

                //on lui affecte un User Control
                affKey = new AffichageClé();
                affKey.DataContext = entry;
                item.Items.Add(affKey);

                //Ajout de la clé à l'arborescence
                inItem.Items.Add(item);

                //on s'abonne à l'évènement Changed pour gérer la modification des informations de la clé créée
                affKey.Changed += onInputTextChanged;
            }
        }

        /// <summary>
        /// Appelé lors du click sur le bouton d'ajout d'une clé
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void onAjouterButtonClicked(object sender, RoutedEventArgs e)
        {
            TreeViewItem father;
            TreeViewItem newItem;
            AffichageClé newKey;
            Entry newEntry;

            try
            {
                //Récupération du dossier auquel on va ajouter la clé
                if (Arborescence.SelectedItem != null)
                    father = (TreeViewItem)Arborescence.SelectedItem;
                else
                    father = (TreeViewItem)Arborescence.Items[0];

                //si l'élément sélectionné est bien un dossier
                if(!isClé(father))
                {
                    //Récupération du nom de la nouvelle clé via une boîte de dialogue
                    string title = Interaction.InputBox("Entrez le nom de la nouvelle clé :", "Nom de la clé");

                    //si un noeud du même nom n'existe pas déjà dans la base
                    if(!isNodeExisting(title, _database.Root))
                    {
                        //Recherche de l'élément de la Database correspondant à l'élément père
                        Groupe groupePere = RechercherGroupe(_database.Root, (string)((TextBlock)((StackPanel)father.Header).Children[1]).Text);

                        //création du nouvel élément graphique de l'arborescence
                        newItem = new TreeViewItem();
                        newItem.Header = createHeader(title, false);

                        //Création de la nouvelle clé
                        newEntry = new Entry();
                        newEntry.Title = title;

                        //Ajout de la clé à la base
                        if (groupePere != null)
                        {
                            groupePere.Entries.Add(newEntry);
                        }

                        //Création du User Control pour cette nouvelle clé
                        newKey = new AffichageClé();
                        newKey.DataContext = newEntry;
                        newKey.Changed += onInputTextChanged;

                        newItem.Items.Add(newKey);

                        //Ajout de la clé à l'arborescence graphique
                        father.Items.Add(newItem);

                        //Mise à jour des dates de dernière modification sur tous les dossiers pères
                        UpdateCascadeDateModification(newEntry, _database.Root);
                    }
                    else
                        MessageBox.Show("Impossible de créer la clé : un noeud du même nom existe déjà dans l'arborescence.");
                }
                else
                {
                    MessageBox.Show("Vous ne pouvez ajouter des clés qu'à des dossiers.");
                }
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Vous ne pouvez ajouter des clés qu'à des dossiers.");
            }
        }

        /// <summary>
        /// A la modification d'une des informations d'une clé
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        void onInputTextChanged(object sender, EventArgs e)
        {
            AffichageClé affKey = (AffichageClé)sender;

            //On récupère la clé ayant été modifiée
            Entry entry = (Entry)affKey.DataContext;

            if (entry != null)
            {
                //On met à jour les dates de dernière modification de la clé
                //concernée et des répertoires pères
                entry.UpdateDateModification();
                UpdateCascadeDateModification(entry, _database.Root);
            }
        }

        /// <summary>
        /// Appelée lors du click sur le bouton de suppression d'une clé
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void onSupprimerButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                //on récupère l'élément sélectionné
                TreeViewItem item = (TreeViewItem)Arborescence.SelectedItem;

                if (Arborescence.SelectedItem != null)
                {
                    if (isClé(item))
                    {
                        //Récupération du groupe père de la clé à supprimer
                        Groupe father = RechercherGroupe(_database.Root, (string)((TextBlock)((StackPanel)((TreeViewItem)item.Parent).Header).Children[1]).Text);

                        if (father != null)
                        {
                            //Récupération de la clé à supprimer
                            Entry entryToDelete = RechercherEntry(father, (string)((TextBlock)((StackPanel)((TreeViewItem)item).Header).Children[1]).Text);

                            //Suppression de la clé de la base
                            father.Entries.Remove(entryToDelete);

                            //Suppression du noeud graphique correspondant
                            if (item.Parent is TreeViewItem)
                                (item.Parent as TreeViewItem).Items.Remove(item);

                            //Mise à jour des dates de dernière modification sur tous les répertoires pères
                            UpdateCascadeDateModification(father, _database.Root);
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
        /// Renvoie true si le noeud est une clé, false sinon
        /// </summary>
        /// <param name="inItem">Noeud à tester</param>
        /// <returns>Résultat du test</returns>
        private bool isClé(TreeViewItem inItem)
        {
            bool res = false;

            //si le noeud possède un User Control de clé
            if (inItem.Items.Count != 0)
                if (inItem.Items[0] is AffichageClé)
                    res = true;     //alors c'est une clé

            return res;
        }

        /// <summary>
        /// Recherche un groupe dans la base à partir de son nom
        /// </summary>
        /// <param name="inGroupe">Groupe racine à partir duquel il faut effectuer la recherche</param>
        /// <param name="inTitle">Nom du groupe à rechercher</param>
        /// <returns>Instance du groupe correspondant ou null</returns>
        private Groupe RechercherGroupe(Groupe inGroupe, string inTitle)
        {
            Groupe res = null;

            //si la racine est "valide"
            if (inGroupe != null)
            {
                //si la racine ne correspond pas au groupe recherché
                if (inGroupe.Title != inTitle)
                {
                    //On effectue la recherche dans tous les sous-dossiers
                    foreach (Groupe grp in inGroupe.Groups)
                    {
                        res = RechercherGroupe(grp,inTitle);
                        if (res != null)
                            break;
                    }
                }
                //sinon
                else
                    res = inGroupe;
            }

            return res;
        }

        /// <summary>
        /// Recherche une clé dans la base à partir de son nom
        /// </summary>
        /// <param name="inGroupe">Groupe racine à partir duquel il faut effectuer la recherche</param>
        /// <param name="inTitle">Nom de la clé à rechercher</param>
        /// <returns></returns>
        private Entry RechercherEntry(Groupe inGroupe, string inTitle)
        {
            Entry entry = null;

            //Pour chaque clé du groupe racine
            foreach (Entry en in inGroupe.Entries)
            {
                //si l'on a trouvé la clé
                if (en.Title == inTitle)
                {
                    entry = en;
                    break;
                }
            }

            //si l'on a toujours pas trouvé la clé
            if (entry == null)
            {
                //on effectue la recherche dans les sous-dossiers
                foreach (Groupe groupe in inGroupe.Groups)
                {
                    entry = RechercherEntry(groupe, inTitle);

                    if (entry != null)
                        break;
                }
            }

            return entry;
        }

        /// <summary>
        /// Sauvegarde la Database courante dans un fichier choisi par l'utilisateur
        /// </summary>
        private void Enregistrer()
        {
            SaveFileDialog ofd = new SaveFileDialog();

            //ouverture d'une boîte de dialogue pour le choix du fichier
            DialogResult res = ofd.ShowDialog();

            do
            {
                string filename = ofd.FileName;

                //si le nom du fichier a été renseigné
                if (filename != "")
                {
                    //enregistrement réalisé dans un thread pour ne pas bloquer l'interface graphique durant le traitement
                    Thread newThread = new Thread(() => EnregistrerThread(filename));
                    newThread.Start();
                }
                //sinon
                else
                {
                    MessageBox.Show("Problème de sauvegarde du fichier !");
                    break;
                }
            } while (res.ToString() != "OK");
        }

        /// <summary>
        /// Fonction de démarrage du thread de sauvegarde
        /// </summary>
        /// <param name="filename">Nom du fichier dans lequel la Database courante va être enregistrée</param>
        private void EnregistrerThread(string filename)
        {
            IDatabaseSerializer ids = DatabaseSerializerFactory.Create();

            //Sérialisation de la Database courante
            ids.Save(filename, _database, _gestionUtilisateurs.UtilisateurCourant.CléDeCryptage);
        }

        /// <summary>
        /// Fonction d'enregistrement de la Database dans le fichier de compte
        /// de l'utilisateur courant
        /// </summary>
        private void EnregistrerCompte()
        {
            IDatabaseSerializer ids = DatabaseSerializerFactory.Create();
            
            //Sérialisation de la Database courante de le fichier compte de l'utilisateur
            ids.Save(System.IO.Path.Combine(GestionUtilisateurs._usersDir, 
                _gestionUtilisateurs.UtilisateurCourant.Login),
                _database,
                _gestionUtilisateurs.UtilisateurCourant.CléDeCryptage);
        }

        /// <summary>
        /// Fonction appelée lors du click sur le menu d'ouverture d'un fichier
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void onOuvrirClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            Ouvrir();
        }

        /// <summary>
        /// Ouverture du fichier
        /// Chargement de la Database à partir du fichier ouvert
        /// </summary>
        private void Ouvrir()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            //Récupération du choix de l'utilisateur
            DialogResult res = ofd.ShowDialog();

            if (res.ToString() == "OK")
            {
                string filename = ofd.FileName;

                IDatabaseSerializer ids = DatabaseSerializerFactory.Create();

                try
                {
                    //Chargement de la Database à partir du fichier
                    _database = ids.Load(filename, _gestionUtilisateurs.UtilisateurCourant.CléDeCryptage);

                    //On vide l'arborescence courante
                    Arborescence.Items.Clear();

                    //Remplissage de l'arborescence graphique
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

        /// <summary>
        /// Chargement de la Database à partir du fichier de compte de l'utilisateur courant
        /// </summary>
        private void OuvrirCompte()
        {
            //Désérialisation de la Database du fichier de compte de l'utilisateur courant
            IDatabaseSerializer ids = DatabaseSerializerFactory.Create();
            _database = ids.Load(System.IO.Path.Combine(GestionUtilisateurs._usersDir, _gestionUtilisateurs.UtilisateurCourant.Login), _gestionUtilisateurs.UtilisateurCourant.CléDeCryptage);

            //On vide l'arborescence courante
            Arborescence.Items.Clear();

            //Remplissage de l'arborescence graphique
            RemplirArborescence();
        }


        /// <summary>
        /// Fonction appelée lors du click sur le menu "Quitter"
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void onQuitterClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            //Fermeture de la fenêtre principale
            this.Close();
        }

        /// <summary>
        /// Gestion de la fermeture du programme
        /// </summary>
        /// <param name="e">Arguments de l'évènement de fermeture</param>
        private void GererFermetureProgramme(System.ComponentModel.CancelEventArgs e)
        {
            string messageBoxText = "Souhaitez-vous sauvegarder vos modifications avant de quitter le programme?";
            string caption = "Fermeture du programme";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;
            Connexion connexion;

            //Boîte de dialogue pour confirmer la fermeture du programme
            //avec possibilité de sauvegarder
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            //Traitement du choix de l'utilisateur
            switch (result)
            {
                //sauvegarde puis fermeture
                case MessageBoxResult.Yes:
                    Thread newThread = new Thread(EnregistrerCompte);
                    newThread.Start();
                    connexion = new Connexion();
                    connexion.Show();
                    break;

                //fermeture sans sauvegarde
                case MessageBoxResult.No:
                    connexion = new Connexion();
                    connexion.Show();
                    break;

                //annulation de la fermeture
                case MessageBoxResult.Cancel:
                    e.Cancel = true;
                    break;
            }
        }

        /// <summary>
        /// Fonction appelée lors du click sur la croix rouge de fermeture du programme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //on gère alors la fermeture du programme avec possibilité de sauvegarder la Database
            GererFermetureProgramme(e);
        }

        /// <summary>
        /// Fonction appelée lors de la modification du nombre
        /// de caractères total des mots de passe à générer
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void onNbCaracChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                //Récupération de la nouvelle valeur
                int newValue = int.Parse(inputNbCarac.Text);

                //si celle-ci est inférieure au nombre de caractères spéciaux
                if (newValue < int.Parse(inputNbCaracSpec.Text))
                {
                    MessageBox.Show("Valeur entrée incorrecte : valeur inférieure au nombre de caractères spéciaux");

                    //on n'accepte pas la modification
                    inputNbCarac.Text = Entry.LenghtPassword.ToString();
                }
                //si la nouvelle valeur est inférieure à 2
                else if (newValue < 2)
                {
                    MessageBox.Show("Valeur entrée incorrecte : celle-ci doit être supérieure ou égale à 2");

                    //on n'accepte pas la modification
                    inputNbCarac.Text = Entry.LenghtPassword.ToString();
                }
                //sinon
                else
                    Entry.LenghtPassword = newValue;        //on autorise la modification
            }
            catch (FormatException) //gestion du cas où l'utilisateur ne rentre pas un nombre
            {
                MessageBox.Show("Valeur entrée incorrecte");
                inputNbCarac.Text = Entry.LenghtPassword.ToString();
            }
        }

        /// <summary>
        /// Fonction appelée lors de la modification du nombre
        /// de caractères spéciaux des mots de passe à générer
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void onNbCaracSpecChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                //Récupération de la nouvelle valeur
                int newValue = int.Parse(inputNbCaracSpec.Text);

                //si celle-ci est supérieure au nombre total de caractères
                if (newValue > int.Parse(inputNbCarac.Text))
                {
                    MessageBox.Show("Valeur entrée incorrecte : valeur supérieure au nombre total de caractères");

                    //on n'accepte pas la modification
                    inputNbCaracSpec.Text = Entry.NbCaracSpec.ToString();
                }
                //sinon
                else
                    Entry.NbCaracSpec = newValue;       //on autorise la modification
            }
            catch (FormatException) //gestion du cas où l'utilisateur ne rentre pas un nombre
            {
                MessageBox.Show("Valeur entrée incorrecte");
                inputNbCaracSpec.Text = Entry.NbCaracSpec.ToString();
            }
        }

        /// <summary>
        /// Gestion de l'ajout d'un dossier.
        /// Fonction appelée lors du click sur le bouton d'ajout d'un dossier
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void onAjouterDossierButtonClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            TreeViewItem father;
            TreeViewItem newItem;
            Groupe newGroupe;

            try
            {
                //Récupération du dossier père
                if (Arborescence.SelectedItem != null)
                    father = (TreeViewItem)Arborescence.SelectedItem;
                else
                    father = (TreeViewItem)Arborescence.Items[0];

                //si celui-ci est bien un dossier
                if(!isClé(father))
                {
                    //récupération du nom de ce nouveau dossier via une boîte de dialogue
                    string title = Interaction.InputBox("Entrez le nom de la nouvelle clé :", "Nom de la clé");

                    //si un noeud du même nom n'est pas déjà présent dans l'arborescence
                    if(!isNodeExisting(title, _database.Root))
                    {
                        //Recherche de l'élément de la Database correspondant à l'élément père
                        Groupe groupePere = RechercherGroupe(_database.Root,
                            (string)((TextBlock)((StackPanel)father.Header).Children[1]).Text);

                        //Création du nouveau noeud graphique
                        newItem = new TreeViewItem();
                        newItem.Header = createHeader(title, true);

                        //Création du nouveau groupe
                        newGroupe = new Groupe();
                        newGroupe.Title = title;

                        //Ajout du nouveau groupe à la Database
                        if (groupePere != null)
                        {
                            groupePere.Groups.Add(newGroupe);
                        }

                        //Ajout du noeud graphique à l'arborescence
                        father.Items.Add(newItem);

                        //Mise à jour des dates de dernière modification sur tous les dossiers pères
                        UpdateCascadeDateModification(newGroupe, _database.Root);
                    }
                    else
                        MessageBox.Show("Impossible de créer le dossier : un noeud du même nom existe déjà dans l'arborescence.");
                        
                }
                else
                {
                    MessageBox.Show("Vous ne pouvez ajouter des dossiers qu'à des dossiers.");
                }
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Vous ne pouvez ajouter des dossiers qu'à des dossiers.");
            }
        }

        /// <summary>
        /// Gestion de la suppression d'un dossier.
        /// Fonction appelée lors du click sur le bouton de suppression d'un dossier
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void onSupprimerDossierButtonClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                //Récupération de l'élément sélectionné par l'utilisateur
                TreeViewItem item = (TreeViewItem)Arborescence.SelectedItem;

                if (Arborescence.SelectedItem != null)
                {
                    //si celui-ci est bien un dossier
                    if (!isClé(item))
                    {
                        //on récupère le dossier père
                        Groupe father = RechercherGroupe(_database.Root, (string)((TextBlock)((StackPanel)((TreeViewItem)item.Parent).Header).Children[1]).Text);

                        if (father != null)
                        {
                            //on récupère le dossier correspondant à supprimer
                            Groupe groupToDelete = RechercherGroupe(father, (string)((TextBlock)((StackPanel)((TreeViewItem)item).Header).Children[1]).Text);

                            //on supprime le dossier de la Database
                            father.Groups.Remove(groupToDelete);

                            //on supprime le noeud de l'arborescence graphique
                            if (item.Parent is TreeViewItem)
                                (item.Parent as TreeViewItem).Items.Remove(item);

                            //Mise à jour des dates de dernière modification sur tous les dossiers pères
                            UpdateCascadeDateModification(father, _database.Root);
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

        /// <summary>
        /// Fonction appelée lors du click sur le bouton de renommage d'un élément
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void onRenommerClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                //Récupération de l'élément sélectionné par l'utilisateur
                TreeViewItem item = (TreeViewItem)Arborescence.SelectedItem;

                if (Arborescence.SelectedItem != null)
                {
                    //Récupération du header correspondant
                    StackPanel stackPanel = (StackPanel)item.Header;
                    TextBlock textBlock = (TextBlock)stackPanel.Children[1];

                    //Récupération du nouveau nom via une boîte de dialogue
                    string title = Interaction.InputBox("Entrez le nouveau nom :", "Nom du noeud");

                    //si un noeud du même nom n'est pas déjà présent dans la base
                    if (!isNodeExisting(title, _database.Root))
                    {
                        //si l'élément à renommer est un dossier
                        if (!isClé(item))
                        {
                            //on recherche le dossier correspondant dans la base
                            Groupe groupe = RechercherGroupe(_database.Root, textBlock.Text);

                            if (groupe != null)
                            {
                                //on effectue le renommage du dossier
                                groupe.Title = title;
                                groupe.UpdateDateModification();

                                //Mise à jour des dates de dernière modification sur tous les dossiers pères
                                UpdateCascadeDateModification(groupe, _database.Root);
                            }
                        }
                        //sinon, alors l'élément à renommer est une clé
                        else
                        {
                            //on recherche la clé correspondante dans la base
                            Entry entry = RechercherEntry(_database.Root, textBlock.Text);

                            if (entry != null)
                            {
                                //on effectue le renommage de la clé
                                entry.Title = title;
                                entry.UpdateDateModification();

                                //Mise à jour des dates de dernière modification sur tous les dossiers pères
                                UpdateCascadeDateModification(entry, _database.Root);
                            }
                        }

                        //on effectue le renommage de manière graphique
                        textBlock.Text = title;
                    }
                    else
                    {
                        MessageBox.Show("Impossible de réaliser le renommage : un noeud du même nom existe déjà dans l'arborescence.");
                    }
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

        /// <summary>
        /// Fonction appelée lors de l'appui sur le bouton
        /// permettant d'aller à l'URL de la clé sélectionnée
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void onAllerURLClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                //Récupération de l'élément sélectionné par l'utilisateur
                TreeViewItem item = (TreeViewItem)Arborescence.SelectedItem;

                if (Arborescence.SelectedItem != null)
                {
                    //si celui-ci est bien une clé
                    if (isClé(item))
                    {
                        //on récupère le User Control correspondant
                        AffichageClé affKey = (AffichageClé)item.Items[0];

                        try
                        {
                            //on lance le navigateur par défaut avec l'URL de la clé sélectionnée
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

        /// <summary>
        /// Gestion de l'appui sur des touches du clavier
        /// </summary>
        /// <param name="sender">Objet qui envoie l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void onKeyDownHandler(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //on teste si un élément est sélectionné
            if (Arborescence.SelectedItem != null)
            {
                if(Keyboard.Modifiers == ModifierKeys.Control)
                {
                    //Copie de l'identifiant -- Ctrl + B
                    if (e.Key == Key.B)
                    {
                        CopierClé(true);
                    }

                    //Copie du mot de passe -- Ctrl + C
                    if (e.Key == Key.C)
                    {
                        CopierClé(false);
                    }
                }
            }
        }

        /// <summary>
        /// Fonction effectuant la copie des informations d'une clé dans le presse-papier
        /// </summary>
        /// <param name="id">Booléen indiquant si l'on cherche à copier l'identifiant (true) ou le mot de passe (false)</param>
         private void CopierClé(bool id)
         {
             try
             {
                 //Récupération de l'élément sélectionné par l'utilisateur
                 TreeViewItem item = (TreeViewItem)Arborescence.SelectedItem;

                 //si celui-ci est une clé
                 if (isClé(item))
                 {
                     //on récupère le User Control correspondant
                     AffichageClé affKey = (AffichageClé)item.Items[0];

                     if (item != null)
                     {
                         string aCopier;

                         if (id)    //copie de l'identifiant
                         {
                             aCopier = affKey.inputId.Text;
                         }
                         else       //copie du mot de passe
                         {
                             aCopier = affKey.inputMdp.Text;
                         }

                         //on met le résultat dans le presse-papier
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

        /// <summary>
        /// Gestion de l'appui sur le menu "Enregistrer"
        /// </summary>
         /// <param name="sender">Objet qui envoie l'évènement</param>
         /// <param name="e">Arguments de l'évènement</param>
         private void onEnregistrerClicked(object sender, RoutedEventArgs e)
         {
             Enregistrer();
         }

        /// <summary>
        /// Recherche si un noeud d'un nom en particulier existe déjà dans l'arborescence ou pas
        /// </summary>
        /// <param name="inTitle">Nom du noeud à rechercher</param>
        /// <param name="inRoot">Dossier à partir duquel doit s'effectuer le recherche</param>
        /// <returns>true si un noeud de ce nom existe, false sinon</returns>
        private bool isNodeExisting(string inTitle, Groupe inRoot)
        {
            bool res = false;

            //si le dossier racine possède le nom recherché
            if(inRoot.Title == inTitle)
                res = true;

            //sinon
            if(!res)
            {
                //pour chaque sous-dossier
                foreach (Groupe groupe in inRoot.Groups)
                {
                    //on recherche l'existance d'un noeud de ce nom
                    res = isNodeExisting(inTitle, groupe);

                    //si on en a trouvé un
                    if (res)
                        break;
                }

                //si on a toujours pas trouvé de noeud de ce nom
                if(!res)
                {
                    //on effectue la recherche parmi les clés du dossier racine passé en entrée
                    foreach (Entry entry in inRoot.Entries)
                    {
                        //si on trouve une clé de ce nom là
                        if (entry.Title == inTitle)
                        {
                            res = true;
                            break;
                        }
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Met à jour les dates de dernière modification de tous les dossiers pères du noeud passé en paramètre
        /// </summary>
        /// <param name="inNode">Noeud qui a été modifié</param>
        /// <param name="inPere">Dossier à partir duquel on doit modifier toutes les dates de dernière modification</param>
        private void UpdateCascadeDateModification(Node inNode, Groupe inPere)
        {
            if (inNode != null && inPere != null)
            {
                //si le dossier passé en paramètre possède le noeud en question
                if (isNodeExisting(inNode.Title, inPere))
                    inPere.UpdateDateModification();        //on met à jour sa date de dernière modification

                //Pour chaque sous-dossier
                foreach (Groupe groupe in inPere.Groups)
                {
                    //on effectue la mise à jour
                    UpdateCascadeDateModification(inNode, groupe);
                }
            }
        }
    }
}
