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
    class Program
    {
        static void Main(string[] args)
        {
            Database _database;
            GestionUtilisateurs _gestionUtilisateurs = new GestionUtilisateurs();

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
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Erreur d'authentification");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.ReadLine();
        }
    }
}
