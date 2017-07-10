using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlInjection.Prevention.DataManager.DataOperations
{
    public interface ISqlWriter
    {
        string ConnectionString { get; set; }
        string TableName { get; set; }
        void InsertDataUsingSqlBulkCopy(IEnumerable<Employee> employees);
    }
}
