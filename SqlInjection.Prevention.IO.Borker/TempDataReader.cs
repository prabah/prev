namespace SqlInjection.Prevention.IO.Borker
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using SqlInjection.Prevention.DataManager;
    using SqlInjection.Prevention.DataManager.DataOperations;
    public class TempDataReader : ITempDataReader
    {
        private readonly string _connectionString;
        private readonly string _tableName;
        const string SelectFrom = "SELECT * FROM ";
        const string DeleteFrom = "DELETE FROM ";
        private readonly ISqlReader _sqlReader;

        public TempDataReader(string connectionString, string tableName)
        {
            _connectionString = connectionString;
            _tableName = tableName;
            _sqlReader = new SqlReader
            {
                ConnectionString = connectionString,
                TableName = tableName,
                Query = SelectFrom + tableName
            };
        }

        public IEnumerable<Employee> Employees()
        {
            return _sqlReader.Employees();
        }

        public void DeleteData()
        {
            using (var sc = new SqlConnection(_connectionString))
            using (var cmd = sc.CreateCommand())
            {
                sc.Open();
                cmd.CommandText = DeleteFrom + _tableName;
                cmd.Parameters.AddWithValue("@table", _tableName);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
