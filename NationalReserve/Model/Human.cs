using NationalReserve.View.Core;

#nullable disable

namespace NationalReserve.Model
{
    public partial class Human : CloneableObject
    {
        public int? IdHuman { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Gender { get; set; }
        public int IdRole { get; set; }
        public int IsStaff { get; set; }
        public int IsDeleted { get; set; }

    }
}
