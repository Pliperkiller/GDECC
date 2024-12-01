using CsvHelper.Configuration;

namespace MigrationAPI.Mappings
{

    public class DepartmentCsvModel
    {
        public string _id;
        public string _department;

        public int GetValidId()
        {
            return int.TryParse(_id, out var parseId) ? parseId : -1;
        }

        public string GetValidDepartment()
        {
            return string.IsNullOrWhiteSpace(_department) ? "UNNAMED" : _department;
        }

    }
    public class DepartmentCsvModelMap : ClassMap<DepartmentCsvModel>
    {
        public DepartmentCsvModelMap()
        {
            Map(m => m._id).Name("Id");
            Map(m => m._department).Name("Job");
        }
    }

}
