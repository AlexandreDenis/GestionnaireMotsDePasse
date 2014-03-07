using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using API_GMP;

namespace TestAPI
{
    /// <summary>
    /// Programme de test de l'API mise à disposition par le gestionnaire de mots de passe
    /// </summary>
    class Program
    {
        /// <summary>
        /// Fonction qui convertit l'objet en chaîne de caractères
        /// et qui renvoie null si la référence d'objet est nulle
        /// </summary>
        /// <param name="value">Objet à convertir</param>
        /// <returns>Chaîne de caractères correspondante</returns>
        private static string NullToString(object value)
        {
            return value == null ? "null" : value.ToString();
        }

        /// <summary>
        /// Point d'entrée du programme
        /// </summary>
        /// <param name="args">Tableau comprenant les paramètres passés en entrée du programme</param>
        static void Main(string[] args)
        {
            GestionnaireMDP _gestionnaireMDP = new GestionnaireMDP(true);   //Instanciation de la classe principale de l'API

            string login;
            string password;

            //Test d'accès sans connexion
            Console.WriteLine("Test d'accès sans connexion :");
            Console.WriteLine("\tGetKey(\"test\") = " + NullToString((_gestionnaireMDP.GetKey("test"))));

            //Boucle de connexion
            do
            {
                //Login et mot de passe demandé
                Console.Write("Login : ");
                login = Console.ReadLine();
                Console.Write("Password : ");
                password = Console.ReadLine();
                
                //Tentative de connexion
                _gestionnaireMDP.Connexion(login, password);

                //si la connexion n'a fonctionné
                if(!_gestionnaireMDP.IsConnected)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Erreur d'authentification");
                    Console.ForegroundColor = ConsoleColor.White;
                }

            }while(!_gestionnaireMDP.IsConnected);

            //la connexion a été établie
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Utilisateur " + login + " connecté...");
            Console.ForegroundColor = ConsoleColor.White;

            //Test d'accès avec connexion sur clé non valide
            Console.WriteLine("Test d'accès avec connexion sur clé non valide (l'utilisateur connecté ne doit pas avoir de clé nommée \"Wikipedia\") :");
            Console.WriteLine("\tGetKey(\"Wikipedia\") = " + NullToString((_gestionnaireMDP.GetKey("Wikipedia"))));

            //Test d'accès avec connexion sur clé valide
            Console.WriteLine("Test d'accès avec connexion sur clé valide (l'utilisateur connecté doit avoir une clé nommée \"Facebook\"):");
            Console.WriteLine("\tGetKey(\"Facebook\") = " + NullToString((_gestionnaireMDP.GetKey("Facebook"))));

            //Boucle de test de l'utilisateur sur des clés de son choix
            string choix;
            string choixQuit;
            while (true)
            {
                //Récupération du nom d'une clé
                Console.Write("Entrez le nom d'une clé :");
                choix = Console.ReadLine();

                //si l'utilisateur a saisi le mot "quit"
                if (choix == "quit")
                {
                    //Confirmation de la fermeture de l'application
                    do
                    {
                        Console.WriteLine("Souhaitez-vous réelement quitter l'application ? (y/n)");
                        choixQuit = Console.ReadLine();
                    } while (choixQuit != "y" && choixQuit != "n");

                    //si l'utilisateur souhaite quitter le programme
                    if (choixQuit == "y")
                        break;
                    
                }

                //affichage des informations sur la clé requise par l'utilisateur
                Console.WriteLine("\tGetKey(\"" + choix + "\") = " + NullToString((_gestionnaireMDP.GetKey(choix))));
            }
        }
    }
}
