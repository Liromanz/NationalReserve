using NationalReserve.View.Core;

#nullable disable

namespace NationalReserve.Model
{
    public class StaffDocument : CloneableObject
    {
        public int Id { get; set; }
        public int SerialPass { get; set; }
        public int NumberPass { get; set; }
        public long BankNumber { get; set; }
        public bool IsDeleted { get; set; }


    }
}
