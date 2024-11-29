namespace MigrationAPI.Models
{
    public class Employee
    {
        public double id;
        public string name;
        public DateTime datetime;
        public int department_id;
        public int job_id;
        
        public Employee(double _id, string _name, DateTime _dateTime, int _depId, int _jobId)
        {
            id = _id;
            name = _name;
            datetime = _dateTime;
            department_id = _depId;
            job_id = _jobId;

        }

    }
}
