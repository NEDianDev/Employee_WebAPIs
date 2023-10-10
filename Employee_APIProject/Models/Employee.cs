using System.ComponentModel.DataAnnotations;

namespace Employee_APIProject.Models
{
    public class Employee
    {
        [Obsolete]
        public int EmpID { get; set; }

        [Required]
        [MaxLength(30)]
        public string EmpName { get; set; }

        [Required]
        [MaxLength(50)]
        public string EmpEmail { get; set; }

        [Required]
        [MaxLength(11)]
        public string EmpPhone { get; set; }


        public double EmpSalary { get; set; }

        [Required]
        public DateTime EmpHireDate { get; set; }

        [Obsolete]
        public double EmpAnnualSalary { get; set; }

        public string EmpDesignation { get; set; }

        [Required]
        [MaxLength(50)]
        public string EmpDepartment { get; set; }

        [Required]
        public int EmpManagerID { get; set; }

        [Obsolete]
        public string? EmpManagersName { get; set; }

        [Obsolete]
        public int EmpTypeID { get; set; }
        public Employee() { }
    }
}
