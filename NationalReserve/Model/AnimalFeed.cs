using NationalReserve.View.Core;

#nullable disable

namespace NationalReserve.Model
{
    public partial class AnimalFeed : CloneableObject
    {
        public int? IdFeed { get; set; }
        public int IdSupply { get; set; }
        public int IdAnimal { get; set; }
        public int Amount { get; set; }
        public int IsDeleted { get; set; }

    }
}
