namespace SqlInjection.Prevention.IO.Borker
{
    using System.Collections.Generic;
    using SqlInjection.Prevention.DataManager;
    public interface ITempDataReader
    {
        IEnumerable<Employee> Employees();

        void DeleteData();
    }
}
