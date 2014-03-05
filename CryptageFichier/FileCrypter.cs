using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text;

namespace CryptageFichier
{
    /// <summary>
    /// Classe dont la fonction est de crypter/décrypter des fichiers
    /// </summary>
    public class FileCrypter
    {
        // Supprime la clé de la mémoire avant utilisation pour des raisons de sécurité
        [System.Runtime.InteropServices.DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
        public static extern bool ZeroMemory(ref string Destination, int Length);

        /// <summary>
        /// Crypte un fichier dont le nom est passé en entrée
        /// </summary>
        /// <param name="sInputFilename">Fichier en entrée du cryptage</param>
        /// <param name="sOutputFilename">Fichier en sortie du cryptage</param>
        /// <param name="sKey">Clé pour crypter les données</param>
        public static void EncryptFile(string sInputFilename, string sOutputFilename, string sKey)
        {
            FileStream fsInput = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);

            FileStream fsEncrypted = new FileStream(sOutputFilename, FileMode.Create, FileAccess.Write);

            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();

            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);

            ICryptoTransform desencrypt = DES.CreateEncryptor();
            CryptoStream cryptostream = new CryptoStream(fsEncrypted, desencrypt, CryptoStreamMode.Write);

            byte[] bytearrayinput = new byte[fsInput.Length];
            fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);

            //Libération des ressources utilisées
            cryptostream.Flush();
            cryptostream.Close();
            fsInput.Close();
            fsInput.Dispose();
            fsEncrypted.Close();
            fsEncrypted.Dispose();
        }

        /// <summary>
        /// Décrypte un fichier dont le nom est passé en entrée
        /// </summary>
        /// <param name="sInputFilename">Fichier en entrée du décryptage</param>
        /// <param name="sOutputFilename">Fichier en sortie du décryptage</param>
        /// <param name="sKey">Clé pour décrypter les données</param>
        public static void DecryptFile(string sInputFilename, string sOutputFilename, string sKey)
        {
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();

            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);

            FileStream fsread = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);

            ICryptoTransform desdecrypt = DES.CreateDecryptor();

            CryptoStream cryptostreamDecr = new CryptoStream(fsread,desdecrypt, CryptoStreamMode.Read);

            StreamWriter fsDecrypted = null;

            try
            {
                fsDecrypted = new StreamWriter(sOutputFilename);
                fsDecrypted.Write(new StreamReader(cryptostreamDecr).ReadToEnd());
                fsDecrypted.Flush();
            }
            //si une exception a lieu pendant le décryptage
            finally
            {
                //on libère les ressources utilisées
                if (fsDecrypted != null)
                {
                    fsDecrypted.Close();
                    fsDecrypted.Dispose();
                }

                fsread.Close();
                fsread.Dispose();
            }

            //on libère les ressources utilisées après le décryptage
            fsDecrypted.Close();
            fsDecrypted.Dispose();

            fsread.Close();
            fsread.Dispose();
        }
    }
}
