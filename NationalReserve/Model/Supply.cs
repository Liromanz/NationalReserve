using System;
using NationalReserve.View.Core;

#nullable disable

namespace NationalReserve.Model
{
    public class Supply : CloneableObject
    {
        public int? IdSupply { get; set; }
        public int IdSupplier { get; set; }
        public int IdMaterial { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
        public bool IsDeleted { get; set; }

    }
}
