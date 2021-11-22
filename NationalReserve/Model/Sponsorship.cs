using System;
using System.Collections.Generic;

#nullable disable

namespace NationalReserve.Model
{
    public partial class Sponsorship
    {
        public int? IdPayment { get; set; }
        public int IdHuman { get; set; }
        public int IdType { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
