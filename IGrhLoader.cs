using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aogrhx1
{
    public interface IGrhLoader
    {
        GrhData[] Load(string path);
    }
}