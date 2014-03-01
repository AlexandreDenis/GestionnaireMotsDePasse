using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Datas;

namespace DataStorage
{
    public interface IDatabaseSerializer
    {
        Database Load(string filename, string key);
        void Save(string filename, Database data, string key);
    }
}
