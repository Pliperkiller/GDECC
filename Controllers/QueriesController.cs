using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using System.Text.Json;

namespace MigrationAPI.Controllers
{
    [ApiController]
    [Route("api/extract")]
    public class QueriesController : Controller
    {
        private readonly IConfiguration _configuration;

        public QueriesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private IActionResult ExecuteQuery(string queryFileName)
        {
            try
            {
                string sqlQuery = LoadQueryFromFile(queryFileName);

                DataTable dataTable = ExecuteSqlQuery(sqlQuery);

                var result = ConvertDataTableToList(dataTable);

                string jsonResult = JsonSerializer.Serialize(result);

                return Content(jsonResult, "application/json");
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = $"Failed to execute exception: {e.Message}" });
            }
        }

        [HttpGet("hired-by-department")]
        public IActionResult GetHired()
        {
            return ExecuteQuery("SQL_DepartmentKpiHiredDescriptor.sql");
        }

        [HttpGet("hired-by-quarters")]
        public IActionResult GetQuarters()
        {
            return ExecuteQuery("SQL_QuarterKpiIndicators.sql");
        }

        [HttpDelete("delete/reset-db")]
        public IActionResult ResetDatabase()
        {
            return ExecuteQuery("SQL_ResetDB.sql");
        }


        private string LoadQueryFromFile(string fileName)
        {
            string queryFilePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Queries",
                fileName);

            using (StreamReader reader = new StreamReader(queryFilePath, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        private DataTable ExecuteSqlQuery(string sqlQuery)
        {
            DataTable dataTable = new DataTable("QueryResult");

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultCn")))
            {
                connection.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter(sqlQuery, connection))
                {
                    adapter.Fill(dataTable);
                }
            }

            return dataTable;
        }

        private List<Dictionary<string, object>> ConvertDataTableToList(DataTable dataTable)
        {
            var result = new List<Dictionary<string, object>>();

            foreach (DataRow row in dataTable.Rows)
            {
                var rowDict = new Dictionary<string, object>();
                foreach (DataColumn column in dataTable.Columns)
                {
                    rowDict[column.ColumnName] = row[column];
                }
                result.Add(rowDict);
            }

            return result;
        }
    }
}
