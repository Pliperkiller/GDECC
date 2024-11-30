using System.Text.Json.Serialization;

namespace MigrationAPI.Models
{
    public class Employee
    {
        public int id;
        public string name;
        public DateTime datetime;
        public int department_id;
        public int job_id;
        
        public virtual Department Department { get; set; }
        public virtual Job Job { get; set; }


        //public Employee(double _id, string _name, DateTime _dateTime, int _depId, int _jobId)
        //{
        //    id = _id;
        //    name = _name;
        //    datetime = _dateTime;
        //    department_id = _depId;
        //    job_id = _jobId;

        //}

    }
}
