using NationalReserve.View.Core;

#nullable disable

namespace NationalReserve.Model
{
    public partial class Material : CloneableObject
    {
        public int? IdMaterial { get; set; }
        public string Name { get; set; }
        public decimal CostPerOne { get; set; }
        public int IdType { get; set; }
        public bool IsDeleted { get; set; }

    }
}
