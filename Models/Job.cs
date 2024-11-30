using System.Text.Json.Serialization;

namespace MigrationAPI.Models
{
    public class Job
    {
        public int id;   
        public string job;

        [JsonIgnore]
        public ICollection<Employee> Employees { get; set; }

        //public Job(double _id, string _job)
        //{
        //    id = _id;
        //    job = _job; 
        //}
    }
}
