using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalReserve.Model
{
    public class Authorization
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public string FullName { get; set; }
        public Role Role { get; set; }
    }
}
