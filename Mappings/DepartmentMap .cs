using CsvHelper.Configuration;

namespace MigrationAPI.Mappings
{

    public class DepartmentCsvModel
    {

        public string _departmentId;
        public string _department;

        public int GetValidDepartmentId()
        {
            return int.TryParse(_departmentId, out var parseId) ? parseId : -1;
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
            Map(m => m._departmentId).Name("DepartmentId");
            Map(m => m._department).Name("Department");
        }
    }

}
