using CsvHelper.Configuration;

namespace MigrationAPI.Mappings
{
    public class JobCsvModel
    {
        public string _jobId;
        public string _job;

        public int GetValidJobId()
        {
            return int.TryParse(_jobId, out var parseId) ? parseId : -1;
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

            Map(m => m._jobId).Name("JobId");
            Map(m => m._job).Name("Job");
        }
    }

}
