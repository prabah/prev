namespace SqlInjection.Prevention.DataManager.DataOperations
{
    using System.Collections.Generic;

    public interface ISqlWriter
    {
        string ConnectionString { get; set; }
        string TableName { get; set; }
        void InsertDataUsingSqlBulkCopy(IEnumerable<Employee> employees);
    }
}
