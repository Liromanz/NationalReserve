using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalReserve.View.Core
{
    public class CloneableObject : ICloneable
    {
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
