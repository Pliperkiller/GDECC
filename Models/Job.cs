using System.Text.Json.Serialization;

namespace MigrationAPI.Models
{
    public class Job
    {
        public int id;
        public int jobId;
        public string job;

        //[JsonIgnore]
        //public ICollection<Employee> Employees { get; set; }

    }
}
