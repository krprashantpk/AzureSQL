using AzureSQL.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextFactory<SQLDbContext>(option =>
{
    option.UseSqlServer("", sqlOp=>sqlOp.MigrationsAssembly(typeof(SQLDbContext).Assembly.FullName));
});

builder.Services.AddScoped<IRepository, Repository>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
