using Employee_APIProject.Models;
using Microsoft.AspNetCore.JsonPatch;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;


namespace Employee_APIProject.DataLayer
{
    public class EmployeeDAL
    {
        public string conn = "";

        public EmployeeDAL()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appSettings.json").Build();

            conn = builder.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public List<Employee> GetAllEmployees()
        {
            List<Employee> ListOfEmployees = new List<Employee>();
            using (SqlConnection cn = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM employee WHERE DeleteFlag = 0", cn))
                {
                    if (cn.State == System.Data.ConnectionState.Closed)
                    {
                        cn.Open();
                    }
                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ListOfEmployees.Add(new Employee()
                        {
                            EmpID = int.Parse(reader["EmployeeID"].ToString()),
                            EmpName = reader["Name"].ToString(),
                            EmpEmail = reader["Email"].ToString(),
                            EmpPhone = reader["Phone"].ToString(),
                            EmpSalary = double.Parse(reader["Salary"].ToString()),
                            EmpHireDate = DateTime.Parse(reader["HireDate"].ToString()),
                            EmpAnnualSalary = double.Parse(reader["AnnualSalary"].ToString()),
                            EmpDesignation = reader["Designation"].ToString(),
                            EmpDepartment = reader["Department"].ToString(),
                            EmpManagerID = int.Parse(reader["ManagerID"].ToString()),
                            EmpTypeID = int.Parse(reader["TypeID"].ToString())
                        });
                    }

                    return ListOfEmployees;
                }
            }
        }


        public List<Employee> GetEmployee(int Id)
        {
            List<Employee> employee = new List<Employee>();
            using (SqlConnection cn = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM employee WHERE EmployeeID = @Id AND DeleteFlag = 0", cn))
                {

                    cmd.Parameters.AddWithValue("@Id", Id);
                    if (cn.State == System.Data.ConnectionState.Closed)
                    {
                        cn.Open();
                    }

                    IDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        employee.Add(new Employee()
                        {
                            EmpID = int.Parse(reader["EmployeeID"].ToString()),
                            EmpName = reader["Name"].ToString(),
                            EmpEmail = reader["Email"].ToString(),
                            EmpPhone = reader["Phone"].ToString(),
                            EmpSalary = double.Parse(reader["Salary"].ToString()),
                            EmpHireDate = DateTime.Parse(reader["HireDate"].ToString()),
                            EmpAnnualSalary = double.Parse(reader["AnnualSalary"].ToString()),
                            EmpDesignation = reader["Designation"].ToString(),
                            EmpDepartment = reader["Department"].ToString(),
                            EmpManagerID = int.Parse(reader["ManagerID"].ToString()),
                            EmpTypeID = int.Parse(reader["TypeID"].ToString())
                        });

                        return employee;
                    }
                    else
                    {
                        return null;
                    }

                }
            }
        }


        public List<int> GetAllManagerIDs()
        {
            List<int> IDs = new List<int>();
            using (SqlConnection cn = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT EmployeeID FROM employee WHERE TypeID = 1", cn))
                {
                    if (cn.State == System.Data.ConnectionState.Closed)
                    {
                        cn.Open();
                    }

                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        IDs.Add(int.Parse(reader["EmployeeID"].ToString()));
                    }

                    return IDs;
                }
            }
        }


        public IDictionary<int, String> GetAllManagersNameAndIDs()
        {
            IDictionary<int, string> Managers = new Dictionary<int, String>();
            using (SqlConnection cn = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT EmployeeID, Name FROM employee WHERE TypeID = 1", cn))
                {
                    if (cn.State == System.Data.ConnectionState.Closed)
                    {
                        cn.Open();
                    }

                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Managers.Add(int.Parse(reader["EmployeeID"].ToString()), reader["Name"].ToString());
                    }

                    return Managers;
                }
            }
        }


        public String CreateEmployee(Employee emp)
        {
            using (SqlConnection cn = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO employee VALUES(@EmpName, @EmpEmail, @EmpPhone, @EmpSalary, @EmpHireDate, @EmpAnnualSalary, @EmpDesignation, @EmpDepartment, @EmpManagerID, @EmpTypeID, @DeleteFlag)", cn))
                {
                    if (cn.State == System.Data.ConnectionState.Closed)
                    {
                        cn.Open();
                    }

                    cmd.Parameters.AddWithValue("@EmpName", emp.EmpName);
                    cmd.Parameters.AddWithValue("@EmpEmail", emp.EmpEmail);
                    cmd.Parameters.AddWithValue("@EmpPhone", emp.EmpPhone);
                    cmd.Parameters.AddWithValue("@EmpSalary", emp.EmpSalary);
                    cmd.Parameters.AddWithValue("@EmpHireDate", emp.EmpHireDate);
                    cmd.Parameters.AddWithValue("@EmpAnnualSalary", emp.EmpSalary * 12);

                    TextInfo myStr = new CultureInfo("en-US", false).TextInfo;
                    emp.EmpDesignation = myStr.ToTitleCase(emp.EmpDesignation);

                    cmd.Parameters.AddWithValue("@EmpDesignation", emp.EmpDesignation);
                    cmd.Parameters.AddWithValue("@EmpDepartment", emp.EmpDepartment);

                    List<int> ManagerIDs = GetAllManagerIDs();
                    if (ManagerIDs.Contains(emp.EmpManagerID))
                    {
                        cmd.Parameters.AddWithValue("@EmpManagerID", emp.EmpManagerID);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@EmpManagerID", 0);
                    }

                    if (emp.EmpDesignation == "Manager")
                    {
                        cmd.Parameters.AddWithValue("@EmpTypeID", 1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@EmpTypeID", 1);
                    }
                    cmd.Parameters.AddWithValue("@DeleteFlag", 0);

                    cmd.ExecuteNonQuery();
                    return "{ \"message\": \"An employee has been created!\" }";
                }
            }
        }


        public List<int> GetAllEmployeeIDs()
        {
            List<int> EmpIDs = new List<int>();
            using (SqlConnection cn = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT EmployeeID FROM employee", cn))
                {
                    if (cn.State == System.Data.ConnectionState.Closed)
                    {
                        cn.Open();
                    }

                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        EmpIDs.Add(int.Parse(reader["EmployeeID"].ToString()));
                    }

                    return EmpIDs;
                }
            }
        }


        public int DeleteEmployee(int Id)
        {
            using (SqlConnection cn = new SqlConnection(conn))
            {

                cn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE employee SET DeleteFlag = 1 WHERE EmployeeID = @Id", cn);
                cmd.Parameters.AddWithValue("@Id", Id);
                int rowsAffected = cmd.ExecuteNonQuery();

                String message;
                if (rowsAffected == 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }

            }
        }


        public int UpdatingEmployee(int Id, JsonPatchDocument<Employee> emp)
        {
            using (SqlConnection cn = new SqlConnection(conn))
            {
                int length = emp.Operations.Count();
                if (length > 0)
                {
                    String query = "UPDATE employee SET ";
                    foreach (var i in emp.Operations)
                    {
                        if (length == 1)
                        {
                            if (i.path == "Salary")
                            {
                                double salary = i.value != null ? Convert.ToDouble(i.value) : 0;
                                query = query + $"{i.path} = {salary}, ";
                                query = query + $"AnnualSalary = {salary * 12} ";
                            }
                            else if (i.path == "Designation" && i.value == "Manager")
                            {
                                query = query + $"{i.path} = '{i.value}', ";
                                query = query + $"TypeID = 1, ";
                                query = query + $"ManagerID = 0";
                            }
                            else
                            {
                                query = query + $"{i.path} = '{i.value}' ";
                            }
                        }
                        else if (length > 1)
                        {
                            if (i.path == "Salary")
                            {
                                double salary = i.value != null ? Convert.ToDouble(i.value) : 0;
                                query = query + $"{i.path} = {salary},";
                                query = query + $" AnuualSalary = {salary * 12}, ";
                            }
                            else if (i.path == "Designation" && i.value == "Manager")
                            {
                                query = query + $"{i.path} = '{i.value}', ";
                                query = query + $"TypeID = 1, ";
                                query = query + $"ManagerID = 0, ";
                            }
                            else
                            {
                                query = query + $"{i.path} = '{i.value}', ";
                            }
                        }
                        length -= 1;
                    }

                    query = query + $"WHERE employeeID = {Id};";

                    SqlCommand cmd = new SqlCommand(query, cn);

                    if (cn.State == System.Data.ConnectionState.Closed)
                    {
                        cn.Open();
                    }
                    cmd.ExecuteNonQuery();
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
