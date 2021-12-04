using System;

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
