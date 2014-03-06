using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace InterfaceWPF
{
    /// <summary>
    /// Classe utilisée pour la gestion de la configuration du programme
    /// </summary>
    internal class CustomConfigurationSection : ConfigurationSection
    {
        //Nom de l'attribut utilisé pour le choix du type de sérialisation (XML ou binaire par exemple)
        private const string persistanceArtifact = "typePersistance";

        #region GetSection

        //Déclaration de la balise pour la configuration du programme
        private const string SectionName = "windowsConfig";

        public static CustomConfigurationSection GetSection()
        {
            return (CustomConfigurationSection)ConfigurationManager.GetSection(SectionName);
        }

        #endregion GetSection

        //Propriété utilisée pour la gestion du choix du type de sérialisation
        //Par défaut, la sérialisation/désérialisation est binaire
        [ConfigurationProperty(persistanceArtifact, DefaultValue = "bin")]
        public string typePersistance
        {
            get
            {
                return (string)this[persistanceArtifact];
            }
            set
            {
                this[persistanceArtifact] = value;
            }
        }
    }
}
