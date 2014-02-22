using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerationMotDePasse
{
    public interface IPasswordGenerator
    {
        string GeneratePassword(int length);
    }
}
