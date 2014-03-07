using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Datas;

namespace DataStorage
{
    //Interface que doivent implémenter les sérialiseurs/désérialiseurs de Database
    public interface IDatabaseSerializer
    {
        Database Load(string filename, string key);             //Désérialisation d'une Database à partir d'un fichier
        void Save(string filename, Database data, string key);  //Sérialisation d'une Database
    }
}
