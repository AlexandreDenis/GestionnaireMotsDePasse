using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datas
{
    [Serializable]
    public class Groupe : Node
    {
        /// <summary>
        /// Liste d'entrées du groupe
        /// </summary>
        private List<Entry> entries;
        public List<Entry> Entries
        {
            get { return entries; }
            set { entries = (List<Entry>)value; }
        }

        /// <summary>
        /// Liste de groupes du groupe
        /// </summary>
        private List<Groupe> groups;
        public List<Groupe> Groups
        {
            get { return groups; }
            set { groups = (List<Groupe>)value; }
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="inTitle"></param>
        /// <param name="inExpiration"></param>
        /// <param name="inEntries"></param>
        /// <param name="inGroups"></param>
        public Groupe(string inTitle, Nullable<DateTime> inExpiration, DateTime inDateCreation, DateTime inDateModification, IEnumerable<Entry> inEntries, IEnumerable<Groupe> inGroups)
            :base(inTitle,inExpiration,inDateCreation,inDateModification)
        {
            entries = (List<Entry>)inEntries;
            groups = (List<Groupe>)inGroups;
        }

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public Groupe()
            :base()
        {
            entries = new List<Entry>();
            groups = new List<Groupe>();
        }

        /// <summary>
        /// Ajout d'une entrée au groupe
        /// </summary>
        /// <param name="inEntry"></param>
        public void AddEntry(Entry inEntry)
        {
            entries.Add(inEntry);

            inEntry.GeneratePassword(Entry.LenghtPassword, Entry.NbCaracSpec);

            //affichage des attributs de la nouvelle entrée
            Console.WriteLine("Clé : {0}", inEntry.Title);
            Console.WriteLine("Identifiant : {0}", inEntry.Username);
            Console.WriteLine("Mot de passe : {0}", inEntry.Password);
            Console.WriteLine("URL : {0}", inEntry.Url); 
        }

        /// <summary>
        /// Ajout d'un groupe au groupe
        /// </summary>
        /// <param name="inGroup"></param>
        public void AddGroup(Groupe inGroup)
        {
            groups.Add(inGroup);
        }

        /// <summary>
        /// Conversion en string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder chaine = new StringBuilder();

            chaine.Append("Groupe ");
            chaine.Append(Title);
            chaine.Append(" (expiration ");
            chaine.Append(Expiration);
            chaine.Append(")\n");

            foreach (Groupe grp in Groups)
            {
                chaine.Append(" ");
                chaine.Append(grp.ToString());
                chaine.Append("\n");
            }

            foreach (Entry ent in Entries)
            {
                chaine.Append(" | ");
                chaine.Append(ent.ToString());
                chaine.Append("\n");
            }

            return chaine.ToString();
        }
    }
}
