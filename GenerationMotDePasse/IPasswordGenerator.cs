using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerationMotDePasse
{
    /// <summary>
    /// Interface que doivent implémenter les générateurs de mots de passe
    /// </summary>
    public interface IPasswordGenerator
    {
        string GeneratePassword(int length, int nbCaracSpec);   //génération d'un mot de passe aléatoire
    }
}
