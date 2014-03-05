using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using API_GMP;

namespace TestAPI
{
    class Program
    {
        private static string NullToString(object value)
        {
            return value == null ? "null" : value.ToString();
        }


        static void Main(string[] args)
        {
            GestionnaireMDP _gestionnaireMDP = new GestionnaireMDP(true);
            string login;
            string password;

            //Test d'accès sans connexion
            Console.WriteLine("Test d'accès sans connexion :");
            Console.WriteLine("\tGetKey(\"test\") = " + NullToString((_gestionnaireMDP.GetKey("test"))));

            do
            {
                Console.Write("Login : ");
                login = Console.ReadLine();
                Console.Write("Password : ");
                password = Console.ReadLine();
                
                _gestionnaireMDP.Connexion(login, password);

                if(!_gestionnaireMDP.IsConnected)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Erreur d'authentification");
                    Console.ForegroundColor = ConsoleColor.White;
                }

            }while(!_gestionnaireMDP.IsConnected);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Utilisateur " + login + " connecté...");
            Console.ForegroundColor = ConsoleColor.White;

            //Test d'accès avec connexion sur clé non valide
            Console.WriteLine("Test d'accès avec connexion sur clé non valide :");
            Console.WriteLine("\tGetKey(\"Wikipedia\") = " + NullToString((_gestionnaireMDP.GetKey("Wikipedia"))));

            //Test d'accès avec connexion sur clé valide
            Console.WriteLine("Test d'accès avec connexion sur clé valide :");
            Console.WriteLine("\tGetKey(\"Facebook\") = " + NullToString((_gestionnaireMDP.GetKey("Facebook"))));

            Console.ReadLine();
        }
    }
}
