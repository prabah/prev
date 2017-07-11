namespace SqlInjection.Prevention.IO.Borker
{
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    public class SqlQueryProcessor : ISqlQueryProcessor
    {
        private readonly List<string> _allowedFields;
        private readonly string _tableName;
        readonly SystemValidQuery _systemValidQuery;
        private readonly string _sqlQuery;


        public SqlQueryProcessor(List<string> allowedFields, string tableName, string sqlQuery)
        {
            _allowedFields = allowedFields;
            _tableName = tableName;
            _sqlQuery = sqlQuery;
            this._systemValidQuery = new SystemValidQuery(sqlQuery);
        }

        public string GetQueryValidated()
        {
            return BuildSql();
        }

        string BuildSql()
        {
            var sqlQuerybuilder = new StringBuilder("SELECT ");
            var fieldsList = new List<string>();
            foreach (var field in _systemValidQuery.Fields)
            {
                if (_allowedFields.FirstOrDefault(c => c == field) != null)
                {
                    fieldsList.Add(field);
                }
            }
            
            var fieldsPart = string.Join(", ", fieldsList);
            sqlQuerybuilder.Append(fieldsPart);
            sqlQuerybuilder.Append(" FROM ");
            sqlQuerybuilder.Append(_tableName);
            sqlQuerybuilder.Append(" WHERE ");
            sqlQuerybuilder.Append(_systemValidQuery.Condition);

            return sqlQuerybuilder.ToString();
        }

        public string ValidateQuery()
        {
            //This can be extended to include filter for tables on config
            var sqlQueryProcessor = new SqlQueryProcessor(_allowedFields, "TbTest", _sqlQuery);
            return sqlQueryProcessor.GetQueryValidated();
        }
    }

    public class SystemValidQuery
    {
        private readonly string[] _words;
        private readonly int _fromClauseStartPosition;
        public SystemValidQuery(string sqlQuery)
        {
            var sqlQueryWithOutComma = sqlQuery.Replace(",", string.Empty);
            sqlQueryWithOutComma = sqlQueryWithOutComma.Replace("'", string.Empty);

            _words = sqlQueryWithOutComma.Split(' ');
            _fromClauseStartPosition = _words.ToList().IndexOf("FROM");

            if (_words.Length < 4)
            {
                throw new InvalidArgumentsException("Insufficient query");
            }
        }

        public List<string> Fields
        {
            get
            {
                try
                {
                    return _words.ToList().GetRange(1, _fromClauseStartPosition - 1);
                }
                catch
                {
                    throw new InvalidArgumentsException("Malformed fields list");
                }
            }
        }

        public string Condition
        {
            get
            {
                var conditionalOperator = _words[_fromClauseStartPosition + 4] != "LIKE" ? _words[_fromClauseStartPosition + 4] : "=";
                return _words[_fromClauseStartPosition + 3] + " " + conditionalOperator + " " +
                         _words[_fromClauseStartPosition + 5];
            }
        }
    }
}