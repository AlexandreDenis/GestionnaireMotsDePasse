using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datas
{
    /// <summary>
    /// Classe représentant un dossier
    /// </summary>
    [Serializable]
    public class Groupe : Node
    {
        /// <summary>
        /// Liste des clés du groupe
        /// </summary>
        private List<Entry> entries;
        public List<Entry> Entries
        {
            get { return entries; }
            set { entries = (List<Entry>)value; }
        }

        /// <summary>
        /// Liste des sous-dossiers du groupe
        /// </summary>
        private List<Groupe> groups;
        public List<Groupe> Groups
        {
            get { return groups; }
            set { groups = (List<Groupe>)value; }
        }

        /// <summary>
        /// Constructeur de la classe Groupe
        /// </summary>
        /// <param name="inTitle">Nom du groupe</param>
        /// <param name="inExpiration">Date d'expiration du groupe</param>
        /// <param name="inDateCreation">Date de création du groupe</param>
        /// <param name="inDateModification">Date de modification du groupe</param>
        /// <param name="inEntries">Liste des clés du groupe</param>
        /// <param name="inGroups">Liste des sous-dossiers du groupe</param>
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
        /// Ajout d'une clé au groupe
        /// </summary>
        /// <param name="inEntry">Clé à ajouter au groupe</param>
        public void AddEntry(Entry inEntry)
        {
            entries.Add(inEntry);

            //Génération du mot de passe pour cette nouvelle clé
            inEntry.GeneratePassword(Entry.LenghtPassword, Entry.NbCaracSpec);

            //affichage des attributs de la nouvelle entrée
            Console.WriteLine("Clé : {0}", inEntry.Title);
            Console.WriteLine("Identifiant : {0}", inEntry.Username);
            Console.WriteLine("Mot de passe : {0}", inEntry.Password);
            Console.WriteLine("URL : {0}", inEntry.Url); 
        }

        /// <summary>
        /// Ajout d'un sous-dossier au groupe
        /// </summary>
        /// <param name="inGroup">Sous-dossier à ajouter au groupe</param>
        public void AddGroup(Groupe inGroup)
        {
            groups.Add(inGroup);
        }

        /// <summary>
        /// Conversion de la classe Groupe en string
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

            //On affiche chaque sous-dossier du groupe
            foreach (Groupe grp in Groups)
            {
                chaine.Append(" ");
                chaine.Append(grp.ToString());
                chaine.Append("\n");
            }

            //On affiche chaque clé du groupe
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
