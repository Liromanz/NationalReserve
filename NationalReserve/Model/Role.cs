using NationalReserve.View.Core;

#nullable disable

namespace NationalReserve.Model
{
    public partial class Role : CloneableObject
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public bool IsAnimalFeedVisible { get; set; }
        public bool IsAnimalVisible { get; set; }
        public bool IsAnimalTypeVisible { get; set; }
        public bool IsCheckpointVisible { get; set; }
        public bool IsCheckpointPassVisible { get; set; }
        public bool IsHumanVisible { get; set; }
        public bool IsMaterialVisible { get; set; }
        public bool IsMaterialTypeVisible { get; set; }
        public bool IsPaymentTypeVisible { get; set; }
        public bool IsPlantListVisible { get; set; }
        public bool IsRoleVisible { get; set; }
        public bool IsSecurityListVisible { get; set; }
        public bool IsSponsorshipVisible { get; set; }
        public bool IsStaffDocumentsVisible { get; set; }
        public bool IsSupplierVisible { get; set; }
        public bool IsSupplyVisible { get; set; }
        public bool IsZoneVisible { get; set; }



    }
}
