using System;
using System.Collections.Generic;

#nullable disable

namespace NationalReserve.Model
{
    public partial class StaffDocument
    {
        public int? Id { get; set; }
        public int SerialPass { get; set; }
        public int NumberPass { get; set; }
        public long BankNumber { get; set; }

    }
}
