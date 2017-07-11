namespace SqlInjection.Prevention.DataManager
{
    using System.Collections.Generic;
    using System.Linq;
    using SqlInjection.Prevention.DataManager.DataOperations;
    public class EmployeeManager:IEmployeeManager
    {
        private readonly ISqlReader _sqlDataReader;
        private readonly ISqlWriter _sqlWriter;
        public EmployeeManager(ISqlReader sqlDataReader, ISqlWriter sqlWriter)
        {
            _sqlDataReader = sqlDataReader;
            _sqlWriter = sqlWriter;
        }

        public List<Employee> Employees()
        {
            return _sqlDataReader.Employees(); 
        }

        public void WriteEmployees(List<Employee> employees)
        {
            _sqlWriter.InsertDataUsingSqlBulkCopy(employees.ToList());
        }


    }
}
