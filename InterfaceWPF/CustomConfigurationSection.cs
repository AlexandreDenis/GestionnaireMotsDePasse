using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace InterfaceWPF
{
    class CustomConfigurationSection : ConfigurationSection
    {
        private const string persistanceArtifact = "typePersistance";

        #region GetSection

        private const string SectionName = "windowsConfig";

        public static CustomConfigurationSection GetSection()
        {
            return (CustomConfigurationSection)ConfigurationManager.GetSection(SectionName);
        }

        #endregion GetSection


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
