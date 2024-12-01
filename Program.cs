using Microsoft.EntityFrameworkCore;
using MigrationAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);





// Configure the DbContext with a connection string
builder.Services.AddDbContext<MigrationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultCn"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

// Set Batch settings
builder.Services.Configure<BatchController>(builder.Configuration.GetSection("BatchSettings"));

// Add services to the container.
builder.Services.AddControllers();


// Configure Swagger/OpenAPI for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MigrationDbContext>();
    dbContext.Database.Migrate();
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
