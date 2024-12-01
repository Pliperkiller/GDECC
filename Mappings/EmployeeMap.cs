using CsvHelper.Configuration;

namespace MigrationAPI.Mappings
{

    public class EmployeeCsvModel
    {
        public string _id;
        public string _name;
        public string _datetime;
        public string _departmentId;
        public string _jobId;

        public int GetValidId()
        {
            return int.TryParse(_id, out var parseId) ? parseId : -1;
        }

        public int GetValidDepartmentId()
        {
            return int.TryParse(_departmentId, out var parsedId) ? (parsedId + 10000) : 10000;
        }

        public int GetValidJobId()
        {
            return int.TryParse(_jobId, out var parsedId) ? (parsedId + 10000) : 10000;
        }

        public DateTime GetLocalDateTime()
        {

            return DateTime.TryParse(_datetime, out var parsedDateTime) ? parsedDateTime.ToLocalTime() : DateTime.MinValue;
        }

        public string FillNullName()
        {
            return string.IsNullOrWhiteSpace(_name) ? "UNNAMED" : _name;
        }


    }

    public class EmployeeCsvModelMap : ClassMap<EmployeeCsvModel>
    {
        public EmployeeCsvModelMap()
        {
            Map(m => m._id).Name("Id");
            Map(m => m._name).Name("Name");
            Map(m => m._datetime).Name("DateTime");
            Map(m => m._departmentId).Name("DepartmentId");
            Map(m => m._jobId).Name("JobId");
        }
    }
}
