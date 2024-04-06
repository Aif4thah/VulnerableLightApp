using System.Data;

namespace VulnerableWebApplication.VLAModel
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
    }
    
    public class Creds
    {
        public string user { get; set; }
        public string passwd { get; set; }
    }

    public class Data
    {
        public static string GetLogPage()
        {
            return @"<!doctype html>
<html lang=""fr"">
<head>
<meta charset=""utf-8"">
<title>Application Logs</title>
</head>
<body>
<h1>Application Logs<h1>
</body>
</html>";
        }

        public static DataSet GetDataSet()
        {
            DataTable table = new DataTable();
            table.Columns.Add("user", typeof(string));
            table.Columns.Add("passwd", typeof(string));
            table.Rows.Add("root", "ce5ca673d13b36118d54a7cf13aeb0ca012383bf771e713421b4d1fd841f539a");
            table.Rows.Add("admin", "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918");
            table.Rows.Add("user", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
            var DataSet = new DataSet();
            DataSet.Tables.Add(table);
            return DataSet;
        }

        public static List<Employee> GetEmployees()
        {
            List<Employee> Employees = new List<Employee>() {
               new Employee() { Id = 1, Name = "John", Age = 16, Address = "4 rue jean moulin"},
               new Employee() { Id = 42, Name = "Steve",  Age = 21, Address = "3 rue Victor Hugo" },
               new Employee() { Id = 1000, Name = "Bill",  Age = 18, Address = "4 place du 18 juin" }
            };
            return Employees;
        }


    }
}
