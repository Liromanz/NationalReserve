using NationalReserve.View.Core;

#nullable disable

namespace NationalReserve.Model
{
    public partial class AnimalType : CloneableObject
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int IsDeleted { get; set; }

    }
}
