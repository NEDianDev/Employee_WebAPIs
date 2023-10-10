using Employee_APIProject.DataLayer;
using Employee_APIProject.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Employee_APIProject.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Employee))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Employee>> GetEmployees()
        {
            //return new List<Employee>
            //{
            //    new Employee{EmpID = 1, Name = "Faseeh"},
            //    new Employee{EmpID = 2, Name = "Fateh"}    
            //};

            EmployeeDAL _empDAL = new EmployeeDAL();
            if (_empDAL.GetAllEmployees().Count == 0)
            {
                return NotFound();
            }

            var Managers = _empDAL.GetAllManagersNameAndIDs();
            List<Employee> TempEmployees = _empDAL.GetAllEmployees();
            List<Employee> _Employees = new List<Employee>();
            foreach (Employee employee in TempEmployees)
            {
                if (employee.EmpTypeID == 2)
                {
                    if (employee.EmpManagerID != 0)
                    {
                        foreach (var Manager in Managers)
                        {
                            if (Manager.Key == employee.EmpManagerID)
                            {
                                employee.EmpManagersName = Manager.Value;
                            }
                        }
                    }
                    else
                    {
                        employee.EmpManagersName = "None";
                    }
                }
                else
                {
                    employee.EmpManagersName = null;
                }

                _Employees.Add(employee);
            }

            TempEmployees = null;


            return Ok(_Employees.Select(e => new
            {
                Id = e.EmpID,
                Name = e.EmpName,
                Email = e.EmpEmail,
                Phone = e.EmpPhone,
                Salary = e.EmpSalary,
                AnnualSalary = e.EmpAnnualSalary,
                HireDate = e.EmpHireDate,
                Designation = e.EmpDesignation,
                Department = e.EmpDepartment,
                Manager = e.EmpManagersName
            }));
        }



        [HttpGet("Id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(200, Type = typeof(Employee))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        public ActionResult<Employee> GetEmployee(int Id)
        {
            if (Id <= 0)
            {
                return BadRequest();
            }

            EmployeeDAL _empDAL = new EmployeeDAL();

            // Comment: Bad Approach for fetch an employee because here i fetch all the employees first and then search it out the required employee. i.e more processing and data storage

            //return Ok(_empDAL.GetAllEmployees().FirstOrDefault(u=>u.EmpID==id));

            List<Employee> employee = _empDAL.GetEmployee(Id);

            if (employee == null)
            {
                return NotFound();
            }

            if (employee[0].EmpTypeID == 2)
            {
                if (employee[0].EmpManagerID != 0)
                {
                    var Managers = _empDAL.GetAllManagersNameAndIDs();
                    foreach (var Manager in Managers)
                    {
                        if (Manager.Key == employee[0].EmpManagerID)
                        {
                            employee[0].EmpManagersName = Manager.Value;
                        }
                    }
                }
                else
                {
                    employee[0].EmpManagersName = "None";
                }
            }
            else
            {
                employee[0].EmpManagersName = null;
            }




            return Ok(employee.Select(e => new
            {
                Id = e.EmpID,
                Name = e.EmpName,
                Email = e.EmpEmail,
                Phone = e.EmpPhone,
                Salary = e.EmpSalary,
                AnnualSalary = e.EmpAnnualSalary,
                HireDate = e.EmpHireDate,
                Designation = e.EmpDesignation,
                Department = e.EmpDepartment,
                Manager = e.EmpManagersName
            }));
        }



        [HttpPost(Name = "CreateEmployee")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            if (employee == null)
            {
                return BadRequest(employee);
            }



            EmployeeDAL _empDAL = new EmployeeDAL();
            String message = _empDAL.CreateEmployee(employee);

            JObject json = JObject.Parse(message);
            return Ok(JsonConvert.SerializeObject(json));
        }



        [HttpDelete("Id:int")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteEmployee(int Id)
        {
            if (Id <= 0)
            {
                return BadRequest();
            }

            EmployeeDAL _empDAL = new EmployeeDAL();
            int signal = _empDAL.DeleteEmployee(Id);
            String message;
            if (signal == 0)
            {
                return NotFound();
            }
            else
            {
                message = $" \"Employee with Id = {Id} has been deleted successfully.\"";
                message = "{ \"message\":" + message + "}";
            }

            JObject json = JObject.Parse(message);
            return Ok(JsonConvert.SerializeObject(json));

            // return NoContent(); // StatusCode 204 means request done but nothinf to return
        }


        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateEmployee(int Id, [FromBody] JsonPatchDocument<Employee> EmpPatchDoc)
        {
            if (Id <= 0)
            {
                return BadRequest();
            }
            else
            {
                EmployeeDAL _empDAL = new EmployeeDAL();
                if (_empDAL.GetEmployee(Id)[0].EmpID == Id)
                {
                    int indicator = _empDAL.UpdatingEmployee(Id, EmpPatchDoc);
                    String message;
                    if (indicator == 1)
                    {
                        message = $" \"Employee with Id = {Id} has been updated successfully.\"";
                        message = "{ \"message\":" + message + "}";
                    }
                    else
                    {
                        message = "{ \"message\": \"Invalid Input! Record has not been updated!\" }";
                    }

                    JObject json = JObject.Parse(message);
                    return Ok(JsonConvert.SerializeObject(json));

                }
                else
                {
                    return NotFound();
                }
            }
        }
    }
}
