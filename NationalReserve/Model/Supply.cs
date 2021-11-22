using System;
using System.Collections.Generic;

#nullable disable

namespace NationalReserve.Model
{
    public partial class Supply
    {
        public int? IdSupply { get; set; }
        public int IdSupplier { get; set; }
        public int IdMaterial { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
