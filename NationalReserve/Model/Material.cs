using System;
using System.Collections.Generic;

#nullable disable

namespace NationalReserve.Model
{
    public partial class Material
    {
        public int? IdMaterial { get; set; }
        public string Name { get; set; }
        public decimal CostPerOne { get; set; }
        public int IdType { get; set; }
    }
}
