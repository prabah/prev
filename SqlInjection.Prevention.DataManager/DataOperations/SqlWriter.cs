using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SqlInjection.Prevention.DataManager.DataOperations
{
    public class SqlWriter : ISqlWriter
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
 
        public void InsertDataUsingSqlBulkCopy(IEnumerable<Employee> employees)
        {
            if (string.IsNullOrEmpty(ConnectionString) || string.IsNullOrEmpty(TableName))
            {
                throw new ArgumentException("ConnectionString and TableName must be specified");
            }

            var destination = new SqlConnection(ConnectionString);
            destination.Open();
            var bulkCopy = new SqlBulkCopy(destination) {DestinationTableName = TableName};
            bulkCopy.ColumnMappings.Add("LastName", "LastName");
            bulkCopy.ColumnMappings.Add("FirstName", "FirstName");
            bulkCopy.ColumnMappings.Add("Salary", "Salary");

            using (var dataReader = new EmployeeDataReader<Employee>(employees))
            {
                bulkCopy.WriteToServer(dataReader);
            }
        }
    }


    public class EmployeeDataReader<TData> : IDataReader
    {
        private IEnumerator<TData> _dataEnumerator;
        private readonly Func<TData, object>[] _accessors;
        private readonly Dictionary<string, int> _ordinalLookup;
        public EmployeeDataReader(IEnumerable<TData> data)
        {
            this._dataEnumerator = data.GetEnumerator();

            // Get all the readable properties for the class and
            // compile an expression capable of reading it
            var propertyAccessors = typeof(TData)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead)
                .Select((p, i) => new
                {
                    Index = i,
                    Property = p,
                    Accessor = CreatePropertyAccessor(p)
                })
                .ToArray();

            this._accessors = propertyAccessors.Select(p => p.Accessor).ToArray();
            this._ordinalLookup = propertyAccessors.ToDictionary(
                p => p.Property.Name,
                p => p.Index,
                StringComparer.OrdinalIgnoreCase);
        }

        private Func<TData, object> CreatePropertyAccessor(PropertyInfo p)
        {
            var parameter = Expression.Parameter(typeof(TData), "input");
            var propertyAccess = Expression.Property(parameter, p.GetGetMethod());
            var castAsObject = Expression.TypeAs(propertyAccess, typeof(object));
            var lamda = Expression.Lambda<Func<TData, object>>(castAsObject, parameter);
            return lamda.Compile();
        }

        #region IDataReader Members

        public void Close()
        {
            this.Dispose();
        }

        public int Depth => 1;

        public DataTable GetSchemaTable()
        {
            return null;
        }

        public bool IsClosed => this._dataEnumerator == null;

        public bool NextResult()
        {
            return false;
        }

        public bool Read()
        {
            if (this._dataEnumerator == null)
            {
                throw new ObjectDisposedException("ObjectDataReader");
            }

            return this._dataEnumerator.MoveNext();
        }

        public int RecordsAffected => -1;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._dataEnumerator != null)
                {
                    this._dataEnumerator.Dispose();
                    this._dataEnumerator = null;
                }
            }
        }

        #endregion

        #region IDataRecord Members

        public int FieldCount
        {
            get { return this._accessors.Length; }
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public int GetOrdinal(string name)
        {
            int ordinal;
            if (!this._ordinalLookup.TryGetValue(name, out ordinal))
            {
                throw new InvalidOperationException("Unknown parameter name " + name);
            }

            return ordinal;
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public object GetValue(int i)
        {
            if (this._dataEnumerator == null)
            {
                throw new ObjectDisposedException("ObjectDataReader");
            }

            return this._accessors[i](this._dataEnumerator.Current);
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        public object this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        public object this[int i]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
