using System.Text.Json.Serialization;

namespace MigrationAPI.Models
{
    public class Employee
    {
        public int id;
        public int employeeId;
        public string name;
        public DateTime datetime;
        public int department_id;
        public int job_id;
        
        //public virtual Department Department { get; set; }
        //public virtual Job Job { get; set; }


    }
}
