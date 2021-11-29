using System;
using NationalReserve.View.Core;

#nullable disable

namespace NationalReserve.Model
{
    public partial class SecurityList : CloneableObject
    {
        public int? IdSecurity { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public int IdHuman { get; set; }
        public int IdCheckpoint { get; set; }
    }
}
