using System;
using System.Collections.Generic;

#nullable disable

namespace NationalReserve.Model
{
    public partial class Zone
    {
        public int? IdZone { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Square { get; set; }
        public int IsForStaff { get; set; }
        public int IsForWatch { get; set; }
        public int IdCheckpoint { get; set; }
    }
}
