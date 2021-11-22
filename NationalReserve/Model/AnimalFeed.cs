using System;
using System.Collections.Generic;

#nullable disable

namespace NationalReserve.Model
{
    public partial class AnimalFeed
    {
        public int? IdFeed { get; set; }
        public int IdSupply { get; set; }
        public int IdAnimal { get; set; }
        public int Amount { get; set; }
    }
}
