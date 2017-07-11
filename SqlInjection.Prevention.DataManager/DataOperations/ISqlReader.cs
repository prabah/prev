namespace SqlInjection.Prevention.DataManager.DataOperations
{
    using System.Collections.Generic;
    public interface ISqlReader
    {
        string ConnectionString { get; set; }
        string TableName { get; set; }

        string Query { get; set; }
        List<Employee> Employees();
    }
}
