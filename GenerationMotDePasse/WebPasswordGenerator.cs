using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GenerationMotDePasse
{
    public class WebPasswordGenerator : IPasswordGenerator
    {
        /// <summary>
        /// Génère un mot de passe selon une longueur donnée
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string GeneratePassword(int length, int nbCaracSpec)
        {
            if (length < 2)
            {
                throw new ArgumentOutOfRangeException("length", "La longueur doit être supérieure ou égale à 2.");
            }

            return System.Web.Security.Membership.GeneratePassword(length, nbCaracSpec);
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        public WebPasswordGenerator()
        {

        }
    }
}
