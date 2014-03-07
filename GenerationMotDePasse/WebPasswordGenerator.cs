using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GenerationMotDePasse
{
    /// <summary>
    /// Générateur de mots de passe
    /// </summary>
    public class WebPasswordGenerator : IPasswordGenerator
    {
        /// <summary>
        /// Génère un mot de passe selon une longueur donnée
        /// </summary>
        /// <param name="length">Longueur que doit faire le mot de passe</param>
        /// <param name="nbCaracSpec">Nombre de caractères spéciaux que doit contenir le mot de passe généré</param>
        /// <returns>Mot de passe généré aléatoirement</returns>
        public string GeneratePassword(int length, int nbCaracSpec)
        {
            //si la longueur du mot de passe à générer est inférieure à 2
            if (length < 2)
            {
                throw new ArgumentOutOfRangeException("length", "La longueur doit être supérieure ou égale à 2.");
            }

            //Renvoie un mot de passe généré aléatoirement selon les paramètres passés en entrée
            return System.Web.Security.Membership.GeneratePassword(length, nbCaracSpec);
        }

        /// <summary>
        /// Constructeur de la classe WebPasswordGenerator
        /// </summary>
        public WebPasswordGenerator()
        {

        }
    }
}
