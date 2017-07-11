namespace SqlInjection.Prevention.DataManager.DataOperations
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public class SqlReader : ISqlReader
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
        public string Query{ get; set; }
        public List<Employee> Employees()
        {
             if (string.IsNullOrEmpty(ConnectionString) || string.IsNullOrEmpty(TableName) || string.IsNullOrEmpty(Query))
            {
                throw new ArgumentException("ConnectionString, TableName and Query must be defined");
            }

            var employees = new List<Employee>();

            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(Query, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employees.Add(new Employee
                            {
                                Id = int.Parse(reader[0].ToString()),
                                FirstName = reader[1].ToString(),
                                LastName =  reader[2].ToString(),
                                Salary = double.Parse(reader[3].ToString())
                            });
                        }
                    } 
                }
            }
            return employees;
        }
    }
}
