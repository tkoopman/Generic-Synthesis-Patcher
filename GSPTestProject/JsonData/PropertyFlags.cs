using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSPTestProject.JsonData
{
    [Flags]
    public enum PropertyFlags
    {
        None = 0,
        Common = 1 << 0,
        Unused = 1 << 1,
        Match = 1 << 2,
        Fill = 1 << 3,
        Forward = 1 << 4,
        ForwardSelfOnly = 1 << 5,
        Merge = 1 << 6,
        DeepCopyIn = 1 << 7,
    }
}