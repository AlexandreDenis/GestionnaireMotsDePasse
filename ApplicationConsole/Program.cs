﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Users;
using Datas;
using DataStorage;

namespace ApplicationConsole
{
    class Program
    {
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

            if (_gestionUtilisateurs.Connexion(login, password))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Utilisateur " + login + " connecté...");
                Console.ForegroundColor = ConsoleColor.White;


                IDatabaseSerializer ids = DatabaseSerializerFactory.Create();
                _database = ids.Load(System.IO.Path.Combine(GestionUtilisateurs._usersDir, _gestionUtilisateurs.UtilisateurCourant.Login), _gestionUtilisateurs.UtilisateurCourant.CléDeCryptage);

                root = _database.Root;
                courant = root;

                Console.WriteLine("Pour des informations, veuillez entrer la commande help");

                do
                {
                    if (!choix.Equals("first"))
                    {
                        param = choix.Split(new Char[] { ' ' });
                        switch (param.ElementAt(0))
                        {
                            case "help":
                                Console.WriteLine("->Pour afficher l'arborescence : ls");
                                Console.WriteLine("->Pour afficher les informations d'une clé : print nom_de_la_clé");
                                Console.WriteLine("->Pour afficher l'arborescence d'un répertoire : ls nom_répertoire");
                                Console.WriteLine("->Pour changer de répertoire : cd");
                                Console.WriteLine("->Pour quitter : quit");
                                Console.WriteLine("->Pour afficher l'aide : help");
                                break;
                            case "ls":
                                if (param.Count() == 2)
                                {
                                    ChercherDossier(param.ElementAt(1), courant);
                                }
                                else if (param.Count() == 1)
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
                                if (!ChercherCle(param.ElementAt(1), root))
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("la clé n'existe pas");
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                                break;
                            case "cd":
                                if (param.Count() == 1)
                                {
                                    courant = root;
                                }
                                else
                                {
                                    Groupe courantNew = Deplacer(param.ElementAt(1), courant, root);
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
                    }
                    Console.Write(" > ");
                    choix = Console.ReadLine();
                } while (!choix.Equals("quit"));

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Erreur d'authentification");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

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

        static private void Afficher(Groupe node,int depth)
        {
            if (node != null)
            {
                for (int i = 0; i < depth; ++i)
                    Console.Write(" ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(node.Title);
                Console.ForegroundColor = ConsoleColor.White;

                foreach (Entry e in node.Entries)
                {
                    for (int i = 0; i < depth + 1; ++i)
                        Console.Write(" ");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("clé : ");
                    Console.WriteLine(e.Title);
                    Console.ForegroundColor = ConsoleColor.White;
                }

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


        static private bool ChercherCle(string keyName, Groupe node)
        {
            bool res = false;
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

            foreach (Groupe g in node.Groups)
            {
                res = ChercherCle(keyName, g);
            }
            return res;
        }

        static private Groupe RechercherPere(Groupe fils,Groupe root)
        {
            Groupe pere = null;

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
                foreach (Groupe g in root.Groups)
                {
                    pere = RechercherPere(fils, g);

                    if (pere != null)
                        break;
                }
            }

            return pere;
        }

        static private Groupe Deplacer(string chemin, Groupe courant,Groupe root)
        {
            String[] dossiers = chemin.Split(new Char[] { '/' });
            Groupe suivant = null;

            if (dossiers.ElementAt(0).Equals(".."))
            {
                suivant = RechercherPere(courant, root);
            }
            else
            {
                foreach (Groupe g in courant.Groups)
                {
                    if (g.Title.Equals(dossiers.ElementAt(0)))
                    {
                        suivant = g;
                        break;
                    }
                }
            }

            if (suivant != null)
            {
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
