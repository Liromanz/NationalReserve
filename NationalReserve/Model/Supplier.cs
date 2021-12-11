using NationalReserve.View.Core;

#nullable disable

namespace NationalReserve.Model
{
    public class Supplier : CloneableObject
    {
        public int? IdSupplier { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public bool IsDeleted { get; set; }

    }
}
