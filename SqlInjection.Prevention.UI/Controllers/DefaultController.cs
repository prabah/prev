using System;
using System.Configuration;
using System.Web.Mvc;
using SqlInjection.Prevention.IO.Borker;

namespace SqlInjection.Prevention.UI.Controllers
{
    public class DefaultController : Controller
    {
        private const string FileName = "SqlQuery.txt";
        private readonly string _filePath;

        public DefaultController()
        {
            _filePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\")) + "Queries\\" + FileName;
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
                FileOperations.WriteSqlQuery(sqlStatement, _filePath);

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
    }
}