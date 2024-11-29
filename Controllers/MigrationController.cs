using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MigrationAPI.Models;
using System.Globalization;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class MigrationController : ControllerBase
{
    private readonly MigrationDbContext _context;

    public MigrationController(MigrationDbContext context)
    {
        _context = context;
    }

    public int batchSize = 1000;

    /*
     CSV Upload Endpoints
    */

    /// Upload a CSV file for Employees and save it to the database.

    [HttpPost("upload/employees")]
    public async Task<IActionResult> UploadEmployees(IFormFile file)
    {
        

        if (file == null || file.Length == 0)
            return BadRequest("File not provided or empty");

        try
        {
            using var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(stream, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<EmployeeCsvModel>().ToList();

            var employees = records.Select(r => new Employee
            {
                id = r._id,
                name = r._name,
                datetime = r._datetime,
                department_id = r._departmentId,
                job_id = r._jobId 
            }).ToList();

            var totalBatches = (int)Math.Ceiling((double)employees.Count / batchSize);

            for (int i = 0; i < totalBatches; i++)
            {
                // Actual batch
                var batch = employees.Skip(i * batchSize).Take(batchSize).ToList();

                // Begin batch transaction
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    await _context.Employees.AddRangeAsync(batch);
                    await _context.SaveChangesAsync();

                    // Confirm transaction
                    await transaction.CommitAsync();
                }
                catch
                {
                    // Revert transaction in error
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Failed to process batch {i + 1} of {totalBatches}");
                }
            }

            return Ok(new { Message = "Employees file processed successfully", TotalRecords = employees.Count, Batches = totalBatches });


        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    
    /// Upload a CSV file for Departments and save it to the database.
    
    [HttpPost("upload/departments")]
    public async Task<IActionResult> UploadDepartments(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File not provided or empty");

        try
        {
            using var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(stream, CultureInfo.InvariantCulture);


            var records = csv.GetRecords<DepartmentCsvModel>().ToList();



            var departments = records.Select(r => new Department
            {
                id = r._id,
                department = r._department,

            }).ToList();

            var totalBatches = (int)Math.Ceiling((double)departments.Count / batchSize);

            for (int i = 0; i < totalBatches; i++) 
            {

                var batch = departments.Skip(i * batchSize).Take(batchSize).ToList();


                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    await _context.Departments.AddRangeAsync(batch);
                    await _context.SaveChangesAsync();

                    // Confirmar la transacción
                    await transaction.CommitAsync();
                }
                catch
                {
                    // Revertir la transacción en caso de error
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Failed to process batch {i + 1} of {totalBatches}");
                }
            }

            return Ok(new { Message = "Departments file processed successfully", TotalRecords = departments.Count, Batches = totalBatches });


        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    
    /// Upload a CSV file for Jobs and save it to the database.
    
    [HttpPost("upload/jobs")]
    public async Task<IActionResult> UploadJobs(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File not provided or empty");

        try
        {
            using var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(stream, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<Job>().ToList();

            await _context.Jobs.AddRangeAsync(records);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Jobs file processed successfully", RecordCount = records.Count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// Simple GET endpoint to verify API is running.
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new { Message = "Migration API is running" });
    }
}



// === Helper CSV Models ===

public class EmployeeCsvModel
{
    public int _id { get; set; }
    public string _name { get; set; }
    public string _datetime { get; set; }
    public int _departmentId { get; set; }
    public int _jobId { get; set; }
}

public class DepartmentCsvModel
{
    public int _id { get; set; }
    public string _department { get; set; }

}

public class JobCsvModel
{
    public int _id { get; set; }
    public string _job { get; set; }

}