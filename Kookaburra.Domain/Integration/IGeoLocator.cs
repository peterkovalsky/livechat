using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Integration
{
    public interface IGeoLocator
    {
        void GetLocation(string ip);
    }
}
