using System;
using NationalReserve.View.Core;

#nullable disable

namespace NationalReserve.Model
{
    public partial class Sponsorship : CloneableObject
    {
        public int? IdPayment { get; set; }
        public int IdHuman { get; set; }
        public int IdType { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public int IsDeleted { get; set; }

    }
}
