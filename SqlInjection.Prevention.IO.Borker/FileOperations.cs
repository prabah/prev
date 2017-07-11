namespace SqlInjection.Prevention.IO.Borker
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.ActiveDirectory;
    using System.IO;
    using SqlInjection.Prevention.DataManager;

    public sealed  class FileOperations
    {
        private static FileOperations _instance;
        public static FileOperations Instance()
        {
            return _instance ?? (_instance = new FileOperations());
        }

        public static void WriteSqlQuery(string sqlStatement, string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (var writetext = new StreamWriter(path))
            {
                writetext.WriteLine(sqlStatement);
            }
        }

        public static string ReadSqlQuery(string path)
        {
            if (path == string.Empty)
            {
                throw new ActiveDirectoryOperationException();
            }

            var sqlQuery = File.ReadAllText(path);
            if (sqlQuery == string.Empty)
            {
                throw new ArgumentException("Invalid SQL Query found");
            }

            return sqlQuery;
        }

        /// <summary>
        /// This method could be extended to use a generic type as it explained in the following line
        /// public static bool WriteDataToFile<T></T>(List<T> dataList)
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public static bool WriteDataToFile(List<Employee> dataList, string path)
        {
            StreamWriter myFile = new StreamWriter(path);
            try
            {
                foreach (var row in dataList)
                {
                    myFile.WriteLine(row.Id + "," + row.FirstName + "," + row.LastName + "," + row.Salary);
                }
                return true;
            }
            finally
            {
                myFile.Close();
            }
        }

        public static List<Employee> ReadEmployeesFromFile(string path)
        {
            var employees = new List<Employee>();
            var file = new System.IO.StreamReader(path);

            try
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    string[] words = line.Split(',');
                    employees.Add(new Employee
                    {
                        Id = int.Parse(words[0]),
                        FirstName = words[1],
                        LastName = words[2],
                        Salary = float.Parse(words[3])
                    });
                }

                return employees;
            }
            finally
            {
                file.Close();
            }
        } 
    }
}
