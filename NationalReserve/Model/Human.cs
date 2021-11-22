using System;
using System.Collections.Generic;

#nullable disable

namespace NationalReserve.Model
{
    public partial class Human
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
    }
}
