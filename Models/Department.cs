using System.Text.Json.Serialization;

namespace MigrationAPI.Models
{
    public class Department
    {

        public int id { get; set; }
        
        public string department { get; set; }

        [JsonIgnore]
        public ICollection<Employee> Employees { get; set; }
        //public Department(int _id, string _dep)
        //{
        //    id = _id;
        //    department = _dep;
        //}
    }
}
