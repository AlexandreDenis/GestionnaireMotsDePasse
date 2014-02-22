using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerationMotDePasse
{
    public class PasswordGeneratorFactory
    {
        /// <summary>
        /// Créé un générateur de mot de passe
        /// </summary>
        /// <returns></returns>
        public static IPasswordGenerator Create()
        {
            return new WebPasswordGenerator();
        }
    }
}
