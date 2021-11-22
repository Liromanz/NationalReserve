using System;
using System.Collections.Generic;

#nullable disable

namespace NationalReserve.Model
{
    public partial class CheckpointPass
    {
        public int? IdCheckpointPass { get; set; }
        public int IdHuman { get; set; }
        public int IdCheckpoint { get; set; }
        public string PassType { get; set; }
        public DateTime PassTime { get; set; }
    }
}
