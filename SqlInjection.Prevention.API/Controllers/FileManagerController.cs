namespace SqlInjection.Prevention.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Configuration;
    using System.Web.Http;
    using SqlInjection.Prevention.DataManager;
    using SqlInjection.Prevention.DataManager.DataOperations;
    using SqlInjection.Prevention.IO.Borker;

    [AllowAnonymous]
    [RoutePrefix("api/FileManager")]
    public class FileManagerController : ApiController
    {
        
        private readonly string _queryFilePath;
        private readonly string _dataFilePath;
        private readonly string _tableName;

        public FileManagerController()
        {
            _tableName = WebConfigurationManager.AppSettings["employeeTableName"];
            var queryFileName = "SqlQuery.txt";
            var dataFileName = "SqlData.txt";
            _queryFilePath = Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\")) +
                             "Queries\\" + queryFileName;
            _dataFilePath = Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\")) +
                             "Queries\\" + dataFileName;
        }


        [HttpGet]
        [Route("FileExists")]
        public IHttpActionResult FileExists(string fileType)
        {
            try
            {
                var filePath = fileType == "read" ? _queryFilePath : _dataFilePath;
                var status = File.Exists(_queryFilePath);
                
                return Ok(status);
            }
            catch (HttpResponseException)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("WriteDataToFile")]
        public IHttpActionResult WriteDataToFile()
        {
            var sqlQueryProcessor = new SqlQueryProcessor(GetAllowedFields(), _tableName, FileOperations.ReadSqlQuery(_queryFilePath));
            try
            {
                var connectionString = WebConfigurationManager.ConnectionStrings["SqlPrevention_Master"].ConnectionString;
                
                var validatedQuery = sqlQueryProcessor.ValidateQuery();
                var sqlDataReader = new SqlReader { ConnectionString = connectionString, TableName = _tableName, Query = validatedQuery};
                var employeeManager = new EmployeeManager(sqlDataReader, null);

                var employees = employeeManager.Employees();

                FileOperations.WriteDataToFile(employees, _dataFilePath);
                return Ok(true);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("ReadDataFromFile")]
        public IHttpActionResult ReadDataFromFile()
        {
            var sqlQueryProcessor = new SqlQueryProcessor(GetAllowedFields(), _tableName, FileOperations.ReadSqlQuery(_queryFilePath));
            try
            {
                var employees = FileOperations.ReadEmployeesFromFile(_dataFilePath);
                var connectionString = WebConfigurationManager.ConnectionStrings["SqlPrevention_Temp"].ConnectionString;
                var sqlDataWriter = new SqlWriter{ ConnectionString = connectionString, TableName = _tableName };

                var employeeManager = new EmployeeManager(null, sqlDataWriter);
                employeeManager.WriteEmployees(employees);

                return Ok(true);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        private List<string> GetAllowedFields()
        {
            var allowedListOfFields = WebConfigurationManager.AppSettings["allowedListOfFields"];
            return allowedListOfFields.Split(',').Select(t => t).ToList();
        }
    }
}
