﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datas
{
    [Serializable]
    public class Database
    {
        /// <summary>
        /// Groupe racine de la base
        /// </summary>
        private Groupe root;
        public Groupe Root
        {
            get { return root; }
            set { root = value; }
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="inRoot"></param>
        public Database(Groupe inRoot)
        {
            root = inRoot;
        }

        /// <summary>
        /// Conversion en string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return root.ToString();
        }


    }
}
