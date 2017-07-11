namespace SqlInjection.Prevention.UI.Controllers
{
    using System;
    using System.Configuration;
    using System.Web.Mvc;
    using SqlInjection.Prevention.IO.Borker;
    using System.IO;
    public class DefaultController : Controller
    {
        private const string FileName = "SqlQuery.txt";
        private const string DataFileName = "SqlData.txt";
        private readonly string _filePath;

        public DefaultController()
        {
            _filePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\")) + "Queries\\";
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult WriteQuery(string maxSalary)
        {
            try
            {
                var sqlStatement = "SELECT * FROM Employees WHERE Salary <= " + maxSalary;
                FileOperations.WriteSqlQuery(sqlStatement, _filePath + FileName);

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult DisplayDataUsingInBuiltQueryEngine()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["SqlPrevention_Temp"].ConnectionString;
            var tableName = ConfigurationManager.AppSettings["employeeTableName"];
            var tempDataReader = new TempDataReader(connectionString, tableName);
            try
            {
                var employees = tempDataReader.Employees();
                tempDataReader.DeleteData();
                return Json(employees, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var errorMessage = "Errors while reading the data";
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetFileContent(string fileSwitch)
        {
            var filePath = fileSwitch == "query" ? _filePath + FileName : _filePath + DataFileName;
            var content = string.Empty;

            try
            {
                using (var stream = new StreamReader(filePath))
                {
                    while (!stream.EndOfStream)
                    {
                        content += "<p>" + stream.ReadLine() + "</p>";
                    }
                }
            }
            catch (Exception exception)
            {
                return Json("Errors occurred while reading the file", JsonRequestBehavior.AllowGet);
            }
            return Json(content, JsonRequestBehavior.AllowGet);
        }
    }
}