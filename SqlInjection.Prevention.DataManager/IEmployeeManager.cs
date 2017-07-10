using System.Collections.Generic;

namespace SqlInjection.Prevention.DataManager
{
    public interface IEmployeeManager
    {
        List<Employee> Employees();
    }

    public class Employee
    {
        public int Id { get; set; }
        public string FullName => FirstName + ' ' + LastName;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double Salary { get; set; }
    }
}
