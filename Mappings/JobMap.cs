using CsvHelper.Configuration;

namespace MigrationAPI.Mappings
{
    public class JobCsvModel
    {
        public string _id;
        public string _job;

        public int GetValidId()
        {
            return int.TryParse(_id, out var parseId) ? parseId : -1;
        }

        public string GetValidJob()
        {
            return string.IsNullOrWhiteSpace(_job) ? "UNNAMED" : _job;
        }


    }

    public class JobCsvModelMap : ClassMap<JobCsvModel>
    {
        public JobCsvModelMap()
        {
            Map(m => m._id).Name("Id");
            Map(m => m._job).Name("Job");
        }
    }

}
