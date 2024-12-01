using System.Text.Json.Serialization;

namespace MigrationAPI.Models
{
    public class Department
    {

        public int id { get; set; }
        public int departmentId { get; set; }
        
        public string department { get; set; }

        //[JsonIgnore]
        //public ICollection<Employee> Employees { get; set; }

    }
}
