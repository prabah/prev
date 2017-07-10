using System.Collections.Generic;
using SqlInjection.Prevention.DataManager;

namespace SqlInjection.Prevention.IO.Borker
{
    public interface ITempDataReader
    {
        IEnumerable<Employee> Employees();

        void DeleteData();
    }
}
