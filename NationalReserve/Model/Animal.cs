using System;
using NationalReserve.View.Core;

#nullable disable

namespace NationalReserve.Model
{
    public partial class Animal : CloneableObject
    {
        public int? IdAnimal { get; set; }
        public string Name { get; set; }
        public int IdType { get; set; }
        public int Age { get; set; }
        public int HasFamily { get; set; }
        public int IsSick { get; set; }
        public DateTime DateRegistration { get; set; }
        public int IdZone { get; set; }
        public DateTime LastCheck { get; set; }
    }
}
