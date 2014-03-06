using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerationMotDePasse
{
    /// <summary>
    /// Factory utilisée pour la création de générateurs de mots de passe
    /// </summary>
    public class PasswordGeneratorFactory
    {
        /// <summary>
        /// Créé un générateur de mot de passe
        /// </summary>
        /// <returns>Instance d'un générateur de mots de passe</returns>
        public static IPasswordGenerator Create()
        {
            return new WebPasswordGenerator();
        }
    }
}
