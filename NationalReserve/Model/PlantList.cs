using System;
using NationalReserve.View.Core;

#nullable disable

namespace NationalReserve.Model
{
    public partial class PlantList : CloneableObject
    {
        public int? IdPlant { get; set; }
        public string Name { get; set; }
        public int IdZone { get; set; }
        public int IdHuman { get; set; }
        public DateTime DateGarden { get; set; }
        public int Amount { get; set; }
        public int DaysToCheck { get; set; }
        public DateTime LastCheck { get; set; }
        public int IdSupply { get; set; }
    }
}
