using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalReserve.Helpers.Interface
{
    interface IDataHandler
    {
        void ReadAsync();
        void AddObject();
        void LogicalDelete();
        void LogicalRecover();
        void SaveAsync();
        string ValidationErrorMessage();
    }
}
