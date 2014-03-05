using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text;

namespace CryptageFichier
{
    public static class KeyGenerator
    {
        // Génère une clé de 64 bits
        public static string GenerateKey()
        {
            // Créé une instance de DESCryptoServiceProvider. La clé et le vecteur IV sont générés automatiquement
            DESCryptoServiceProvider desCrypto = (DESCryptoServiceProvider)DESCryptoServiceProvider.Create();

            //Génère une clé pour l'encryptage des données
            return ASCIIEncoding.ASCII.GetString(desCrypto.Key);
        }
    }
}
