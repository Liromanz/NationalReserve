using NationalReserve.View.Core;

#nullable disable

namespace NationalReserve.Model
{
    public partial class Checkpoint : CloneableObject
    {
        public int? IdCheckpoint { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

    }
}
