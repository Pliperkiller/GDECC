using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MigrationAPI.Controllers;
using MigrationAPI.Models;
using MigrationAPI.Mappings;
using System.Globalization;
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

    [HttpPost("upload/employees")]
    public async Task<IActionResult> UploadEmployees(IFormFile file)
    {
        return await ProcessCsvFile<EmployeeCsvModel, Employee>(
            file,
            map: () => new EmployeeCsvModelMap(),
            transform: r => new Employee
            {
                employeeId = r.GetValidEmployeeId(),
                name = r.FillNullName(),
                datetime = r.GetLocalDateTime(),
                department_id = r.GetValidDepartmentId(),
                job_id = r.GetValidJobId()
            },
            dbSet: _context.Employees,
            successMessage: "Employees file processed successfully"
        );
    }

    [HttpPost("upload/departments")]
    public async Task<IActionResult> UploadDepartments(IFormFile file)
    {
        return await ProcessCsvFile<DepartmentCsvModel, Department>(
            file,
            map: () => new DepartmentCsvModelMap(),
            transform: r => new Department
            {
                departmentId = r.GetValidDepartmentId(),
                department = r.GetValidDepartment()
            },
            dbSet: _context.Departments,
            successMessage: "Departments file processed successfully"
        );
    }

    [HttpPost("upload/jobs")]
    public async Task<IActionResult> UploadJobs(IFormFile file)
    {
        return await ProcessCsvFile<JobCsvModel, Job>(
            file,
            map: () => new JobCsvModelMap(),
            transform: r => new Job
            {
                jobId = r.GetValidJobId(),
                job = r.GetValidJob()
            },
            dbSet: _context.Jobs,
            successMessage: "Jobs file processed successfully"
        );
    }

    private async Task<IActionResult> ProcessCsvFile<TCsv, TEntity>(
        IFormFile file,
        Func<ClassMap<TCsv>> map,
        Func<TCsv, TEntity> transform,
        DbSet<TEntity> dbSet,
        string successMessage) where TEntity : class
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
            csv.Context.RegisterClassMap(map());

            var records = csv.GetRecords<TCsv>().ToList();
            var entities = records.Select(transform).ToList();

            var totalBatches = (int)Math.Ceiling((double)entities.Count / _batchSize);

            for (int i = 0; i < totalBatches; i++)
            {
                var batch = entities.Skip(i * _batchSize).Take(_batchSize).ToList();

                var executionStrategy = _context.Database.CreateExecutionStrategy();
                await executionStrategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();

                    try
                    {
                        await dbSet.AddRangeAsync(batch);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                });
            }

            return Ok(new { Message = successMessage, TotalRecords = entities.Count, Batches = totalBatches });
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
        }
    }
}

