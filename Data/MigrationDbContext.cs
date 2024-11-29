using Microsoft.EntityFrameworkCore;
using MigrationAPI.Models;

public class MigrationDbContext : DbContext
{
    public MigrationDbContext(DbContextOptions<MigrationDbContext> options) : base(options) { }
    
    public DbSet<Department> Departments { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Employee> Employees { get; set; }

}