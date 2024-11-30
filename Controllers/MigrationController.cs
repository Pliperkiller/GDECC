using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MigrationAPI.Controllers;
using MigrationAPI.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

[ApiController]
[Route("api")]
public class MigrationController : ControllerBase
{
    private readonly MigrationDbContext _context;
    private readonly int _batchSize;

    public MigrationController(MigrationDbContext context, IOptions<BatchController> batchSettings)
    {
        _context = context;
        _batchSize = batchSettings.Value.BatchSize;
    }


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
                id = r.GetValidId(),
                name = r.FillNullName(),
                datetime = r.GetLocalDateTime(),
                department_id = r.GetValidDepartmentId(),
                job_id = r.GetValidJobId()

            }
            ).ToList();

            var totalBatches = (int)Math.Ceiling((double)employees.Count / _batchSize);

            for (int i = 0; i < totalBatches; i++)
            {

                var batch = employees.Skip(i * _batchSize).Take(_batchSize).ToList();


                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    await _context.Employees.AddRangeAsync(batch);
                    await _context.SaveChangesAsync();


                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {

                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Failed to process batch {i + 1} of {totalBatches} with exception /n [ {e.Message} ]");
                }
            }

            return Ok(new { Message = "Employees file processed successfully", TotalRecords = employees.Count, Batches = totalBatches });


        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
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
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) 
            {
                HasHeaderRecord = false
            };

            using var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(stream,config);

            csv.Context.RegisterClassMap<DepartmentCsvModelMap>();

            var records = csv.GetRecords<DepartmentCsvModel>().ToList();

            var departments = records.Select(r => new Department
            {
                department = r.GetValidDepartment()
            }
            ).ToList();

            var totalBatches = (int)Math.Ceiling((double)departments.Count / _batchSize);

            for (int i = 0;i < totalBatches; i++)
            {
                var batch = departments.Skip(i * _batchSize).Take(_batchSize).ToList();

                var excecutionStrategy = _context.Database.CreateExecutionStrategy();

                await excecutionStrategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();

                    try
                    {
                        await _context.Departments.AddRangeAsync(batch);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }
                    catch (Exception e) 
                    {
                        await transaction.RollbackAsync();
                        var exceptionMessage = $"Failed to process batch {i + 1} of {totalBatches}. Exception: {e.Message}";
                        if (e.InnerException != null)
                        {
                            exceptionMessage += $"\nInner Exception: {e.InnerException.Message}";
                            exceptionMessage += $"\nStack Trace: {e.InnerException.StackTrace}";
                        }
                    }

                });
            }

            return Ok(new { Message = "Department file processed successfully", RecordCount = records.Count });
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
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
             var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false  
            };

            using var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(stream, config); 

            csv.Context.RegisterClassMap<JobCsvModelMap>();

            var records = csv.GetRecords<JobCsvModel>().ToList();

            var jobs = records.Select(r => new Job
            {

                job = r.GetValidJob()
            }

            ).ToList();

            var totalBatches = (int)Math.Ceiling((double)jobs.Count / _batchSize);

            for (int i = 0; i < totalBatches; i++)
            {
                var batch = jobs.Skip(i * _batchSize).Take(_batchSize).ToList();

                var executionStrategy = _context.Database.CreateExecutionStrategy();

                await executionStrategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();

                    try
                    {
                        await _context.Jobs.AddRangeAsync(batch);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        var exceptionMessage = $"Failed to process batch {i + 1} of {totalBatches}. Exception: {e.Message}";
                        if (e.InnerException != null)
                        {
                            exceptionMessage += $"\nInner Exception: {e.InnerException.Message}";
                            exceptionMessage += $"\nStack Trace: {e.InnerException.StackTrace}";
                        }
                    }
                });
            }

            return Ok(new { Message = "Jobs file processed successfully", RecordCount = records.Count });
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
        }
    }



    /// Simple GET endpoint to verify API is running.
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new { Message = "Migration API is running" });
    }
}

// TODO: CHECK IF DONT REPEAT YOURSELF CAN BE APPLIED ALL POSTS RQ

// === Helper CSV Models ===

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
        return int.TryParse(_departmentId, out var parsedId) ? parsedId : 10000;
    }

    public int GetValidJobId()
    {
        return int.TryParse(_jobId, out var parsedId) ? parsedId : 10000;
    }

    public DateTime GetLocalDateTime() {
        return DateTime.Parse(_datetime).ToLocalTime();
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
