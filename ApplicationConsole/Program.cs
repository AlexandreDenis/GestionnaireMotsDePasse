using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Users;
using Datas;
using DataStorage;

namespace ApplicationConsole
{
    /// <summary>
    /// Application représentant l'usage des données d'un utilisateur via la console
    /// </summary>
    class Program
    {
        /// <summary>
        /// Point d'entrée du programme
        /// </summary>
        /// <param name="args">Les paramètres en entrée du programme</param>
        static void Main(string[] args)
        {
            string choix = "first";
            string[] param;

            Database _database;
            GestionUtilisateurs _gestionUtilisateurs = new GestionUtilisateurs();
            Groupe root;
            Groupe courant;

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

            Console.Write("Login : ");
            string login = Console.ReadLine();
            Console.Write("Password : ");
            string password = Console.ReadLine();

            //On teste si l'utilisateur existe
            if (_gestionUtilisateurs.Connexion(login, password))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Utilisateur " + login + " connecté...");
                Console.ForegroundColor = ConsoleColor.White;


                IDatabaseSerializer ids = DatabaseSerializerFactory.Create();
                _database = ids.Load(System.IO.Path.Combine(GestionUtilisateurs._usersDir, _gestionUtilisateurs.UtilisateurCourant.Login), _gestionUtilisateurs.UtilisateurCourant.CléDeCryptage);

                root = _database.Root;
                courant = root;

                //On teste si l'utilisateur a donné des arguments
                if (args.Count() > 0)
                {
                    if(args.Count() == 1 || args.Count() == 2)
                    {
                        courant = Menu(args, root, courant);
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Il y a un problème l'usage d'une commande passé en paramètre");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else
                {
                    Console.WriteLine("Pour des informations, veuillez entrer la commande help");
                    //boucle principale qui gère les commandes entrées par l'utilisateur
                    do
                    {
                        if (!choix.Equals("first"))
                        {
                            param = choix.Split(new Char[] { ' ' });
                            courant = Menu(param, root, courant);
                        }
                        Console.Write(" > ");
                        choix = Console.ReadLine();
                    } while (!choix.Equals("quit"));
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Erreur d'authentification");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// Menu de gestion de la boucle principale
        /// </summary>
        /// <param name="argu">La commande a exécuter</param>
        /// <param name="root">La racine des données de l'utilisateur</param>
        /// <param name="courant">Le noeud où se trouve l'utilisateur dans l'arborescence de ses donnéees</param>
        /// <returns>Le nouveau noeud où se trouve l'utilisateur dans l'arborescence de ses donnéees</returns>
        static private Groupe Menu(String[] argu,Groupe root,Groupe courant)
        {
            switch (argu.ElementAt(0))
            {
                case "help":
                    Console.WriteLine("->Pour afficher l'arborescence : ls");
                    Console.WriteLine("->Pour afficher les informations d'une clé : print nom_de_la_clé");
                    Console.WriteLine("->Pour afficher l'arborescence d'un répertoire : ls nom_répertoire");
                    Console.WriteLine("->Pour changer de répertoire : cd");
                    Console.WriteLine("->Pour quitter : quit");
                    Console.WriteLine("->Pour afficher l'aide : help");
                    Console.WriteLine("->Vous pouvez passer n'importe quelle commande en argument au programme");
                    break;
                case "ls":
                    if (argu.Count() == 2)
                    {
                        ChercherDossier(argu.ElementAt(1), courant);
                    }
                    else if (argu.Count() == 1)
                    {
                        Afficher(courant, 0);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("problème usage ls");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    break;
                case "print":
                    if (!ChercherCle(argu.ElementAt(1), root))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("la clé n'existe pas");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    break;
                case "cd":
                    if (argu.Count() == 1)
                    {
                        courant = root;
                    }
                    else
                    {
                        Groupe courantNew = Deplacer(argu.ElementAt(1), courant, root);
                        if (courantNew != null)
                        {
                            courant = courantNew;
                        }
                    }
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("commande inconnue");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            return courant;
        }

        /// <summary>
        /// recherche un dossier dans l'arborescence
        /// </summary>
        /// <param name="chemin">Le chemin permettant de trouver le dossier</param>
        /// <param name="node">Le noeud courant où l'utilisateur se situe dans son arborescence</param>
        static private void ChercherDossier(string chemin,Groupe node)
        {
            String[] dossiers = chemin.Split(new Char[] {'/'});
            Groupe suivant = null;

            foreach (Groupe g in node.Groups)
            {
                if (g.Title.Equals(dossiers.ElementAt(0)))
                {
                    suivant = g;
                    break;
                }
            }
            if (suivant != null)
            {
                if (dossiers.Count() == 1)
                {
                    //la recherche est finie, on affiche maintenant
                    Afficher(suivant, 0);
                }
                else
                {
                    //on continu la recherche
                    ChercherDossier(chemin.Substring(dossiers.ElementAt(0).Length + 1), suivant);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Le chemin n'existe pas");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// Affiche l'arborescence à partir d'un noeud donné
        /// </summary>
        /// <param name="node">Le noeud à partir duquel on doit afficher</param>
        /// <param name="depth">profondeur dans l'arboresence pour avoir une indentation correcte lors de l'affichage</param>
        static private void Afficher(Groupe node,int depth)
        {
            if (node != null)
            {
                for (int i = 0; i < depth; ++i)
                    Console.Write(" ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(node.Title);
                Console.ForegroundColor = ConsoleColor.White;

                //Affichage des clés
                foreach (Entry e in node.Entries)
                {
                    for (int i = 0; i < depth + 1; ++i)
                        Console.Write(" ");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("clé : ");
                    Console.WriteLine(e.Title);
                    Console.ForegroundColor = ConsoleColor.White;
                }

                //Affichage des noeuds fils
                foreach (Groupe g in node.Groups)
                {
                    Afficher(g, depth + 1);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Le dossier n'existe pas");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// Cherche une clé dans l'arborescence à partir d'un noeud donné
        /// </summary>
        /// <param name="keyName">Le nom de la clé à trouver</param>
        /// <param name="node">Le noeud à partir duquel on effetue les recherches</param>
        /// <returns>Un booléen indiquant qi la clé a été trouvée</returns>
        static private bool ChercherCle(string keyName, Groupe node)
        {
            bool res = false;

            //recherche de la clé dans le noeud courant
            foreach (Entry e in node.Entries)
            {
                if (e.Title.Equals(keyName))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("clé : ");
                    Console.WriteLine(e.Title+" id="+e.Username+" pwd="+e.Password);
                    Console.ForegroundColor = ConsoleColor.White;
                    res = true;
                }
            }

            //recherche de la clé dans les noeuds fils
            foreach (Groupe g in node.Groups)
            {
                res = ChercherCle(keyName, g);
            }
            return res;
        }

        /// <summary>
        /// Recherche le père d'un noeud -> permet l'utilisation de cd ../.. par exemple
        /// </summary>
        /// <param name="fils">Le noeud à qui on recherche le père</param>
        /// <param name="root">La racine de l'arborescence</param>
        /// <returns>Le noeud père</returns>
        static private Groupe RechercherPere(Groupe fils,Groupe root)
        {
            Groupe pere = null;

            //On regarde si on trouve le fils dans tous les sous-dossiers du dossier courant
            foreach (Groupe g in root.Groups)
            {
                if (g.Title.Equals(fils.Title))
                {
                    pere = root;
                    break;
                }
            }


            if (pere == null)
            {
                //on continu les recherches du père
                foreach (Groupe g in root.Groups)
                {
                    pere = RechercherPere(fils, g);

                    if (pere != null)
                        break;
                }
            }

            return pere;
        }

        /// <summary>
        /// Permet le déplacement dans l'arborescence
        /// </summary>
        /// <param name="chemin">Le chemin vers lequel on doit se déplacer</param>
        /// <param name="courant">Le noeud courant à partir duquel on se déplace</param>
        /// <param name="root">La racine de l'arborescence</param>
        /// <returns>Le nouveau noeud courant</returns>
        static private Groupe Deplacer(string chemin, Groupe courant,Groupe root)
        {
            String[] dossiers = chemin.Split(new Char[] { '/' });
            Groupe suivant = null;

            //on teste si l'on doit effectuer un déplacement du fils vers le père
            if (dossiers.ElementAt(0).Equals(".."))
            {
                suivant = RechercherPere(courant, root);
            }
            else
            {
                //on recherche le dossier vers lequel on doit se déplacer
                foreach (Groupe g in courant.Groups)
                {
                    if (g.Title.Equals(dossiers.ElementAt(0)))
                    {
                        suivant = g;
                        break;
                    }
                }
            }

            //si le dossier vers lequel on doit aller a été trouvé
            if (suivant != null)
            {
                //on teste si l'on doit se déplacer plus en profondeur dans l'arborescence
                if (dossiers.Count() != 1)
                {
                    suivant = Deplacer(chemin.Substring(dossiers.ElementAt(0).Length + 1), suivant,root);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Le chemin n'existe pas");
                Console.ForegroundColor = ConsoleColor.White;
            }
            return suivant;
        }
    }
}
