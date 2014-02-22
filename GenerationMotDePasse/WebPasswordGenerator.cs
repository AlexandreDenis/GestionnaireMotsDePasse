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
        public string GeneratePassword(int length)
        {
            if (length < 2)
            {
                throw new ArgumentOutOfRangeException("length", "La longueur doit être supérieure ou égale à 2.");
            }

            return System.Web.Security.Membership.GeneratePassword(length, length/2);
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        public WebPasswordGenerator()
        {

        }
    }
}
