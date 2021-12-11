using System;
using NationalReserve.View.Core;

#nullable disable

namespace NationalReserve.Model
{
    public partial class CheckpointPass : CloneableObject
    {
        public int? IdCheckpointPass { get; set; }
        public int IdHuman { get; set; }
        public int IdCheckpoint { get; set; }
        public string PassType { get; set; }
        public DateTime PassTime { get; set; }
        public int IsDeleted { get; set; }

    }
}
