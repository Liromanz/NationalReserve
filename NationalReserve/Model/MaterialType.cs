using NationalReserve.View.Core;

#nullable disable

namespace NationalReserve.Model 
{
    public partial class MaterialType : CloneableObject
    {
        public int? Id { get; set; }
        public string Name { get; set; }

    }
}
