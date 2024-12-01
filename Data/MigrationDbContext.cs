using Microsoft.EntityFrameworkCore;
using MigrationAPI.Models;

public class MigrationDbContext : DbContext
{
    public MigrationDbContext(DbContextOptions<MigrationDbContext> options) : base(options) { }
    
    public DbSet<Department> Departments { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        List<Department> departmentInit = new List<Department>();
        departmentInit.Add(new Department()
        {
            id = 1,
            departmentId = 0,
            department = "Unknown",

        });


        List<Job> jobInit = new List<Job>();
        jobInit.Add(new Job()
        {
            id = 1,
            jobId = 0,
            job = "Unknown",

        });


        modelBuilder.Entity<Department>(department =>
        {
            department.ToTable("Department");
            department.HasKey(p => p.id);
            department.Property(p => p.departmentId).IsRequired();
            department.Property(p => p.department).IsRequired().HasMaxLength(150);

            department.HasData(departmentInit);

        });


        modelBuilder.Entity<Job>(job =>
        {
            job.ToTable("Job");
            job.HasKey(p => p.id);
            job.Property(p => p.jobId).IsRequired();
            job.Property(p => p.job).IsRequired().HasMaxLength(150);

            job.HasData(jobInit);

        });


        modelBuilder.Entity<Employee>(employee =>
        {
            employee.ToTable("Employee");
            employee.HasKey(p => p.id);
            employee.Property(p => p.employeeId).IsRequired();
            employee.Property(p => p.name).HasMaxLength(400);
            employee.Property(p => p.datetime).HasColumnName("hiredDatetime");
            employee.Property(p => p.department_id);
            employee.Property(p => p.job_id);
            //employee.HasOne(p => p.Job).WithMany(p => p.Employees).HasForeignKey(p => p.job_id);
            //employee.HasOne(p => p.Department).WithMany(p => p.Employees).HasForeignKey(p => p.department_id);

        });


        base.OnModelCreating(modelBuilder);
    }

}