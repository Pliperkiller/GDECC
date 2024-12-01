using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using System.Text.Json;

namespace MigrationAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class QueriesController : Controller
    {
        private readonly IConfiguration _configuration;

        public QueriesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("extract/hired")]
        public IActionResult GetHired() {

            string QueryFilePath = Path.Combine(
                Directory.GetCurrentDirectory(), 
                "Queries",
                "SQL_DepartmentKpiHiredDescriptor.sql");

            string sqlQuery;
            using (StreamReader reader = new StreamReader(QueryFilePath, Encoding.UTF8))
            {
                sqlQuery = reader.ReadToEnd();
            }

            DataTable dataTable = new DataTable("QueryResult");

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultCn")))
            {
                connection.Open();
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(sqlQuery, connection);
                    adapter.Fill(dataTable);
                }
                catch (Exception e)
                {
                    var exceptionMessage = $"Failed to retrieve data. Exception: {e.Message}";
                
                }
                finally
                {
                    connection.Close();
                }
            }



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

            var response = new
            {
                Results = result
            };

            string jsonResult = JsonSerializer.Serialize(result);

            return Content(jsonResult, "application/json");

        }


        [HttpGet("extract/quarters")]
        public IActionResult GetQuarters()
        {

            string QueryFilePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Queries",
                "SQL_QuarterKpiIndicators.sql");

            string sqlQuery;
            using (StreamReader reader = new StreamReader(QueryFilePath, Encoding.UTF8))
            {
                sqlQuery = reader.ReadToEnd();
            }

            DataTable dataTable = new DataTable("QueryResult");

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultCn")))
            {
                connection.Open();
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(sqlQuery, connection);
                    adapter.Fill(dataTable);
                }
                catch (Exception e)
                {
                    var exceptionMessage = $"Failed to retrieve data. Exception: {e.Message}";

                }
                finally
                {
                    connection.Close();
                }
            }



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

            var response = new
            {
                Results = result
            };

            string jsonResult = JsonSerializer.Serialize(result);

            return Content(jsonResult, "application/json");

        }

    }
}
