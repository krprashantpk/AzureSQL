using AzureSQL.Data;
using AzureSQL.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace AzureSQL.WebApi.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class EmployeesController : ApiControllerBase
    {
        private readonly IDbContextFactory<SQLDbContext> contextFactory;

        public EmployeesController(IDbContextFactory<SQLDbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }
        [HttpGet]
        public async Task<IEnumerable<Employee>> GetAsync(CancellationToken cancellationToken)
        {
            var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Employees.Include(x=>x.Department).ToListAsync(cancellationToken);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
        {
            var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            var user =  await context.Employees.Include(x => x.Department).FirstOrDefaultAsync(x=>x.Id==id, cancellationToken);
            if (user == null) StatusCode((int)HttpStatusCode.NoContent);
            return Ok(user);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            var user = await context.Employees.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if(user == null) StatusCode((int)HttpStatusCode.BadRequest);
            context.Employees.Remove(user!);
            await context.SaveChangesAsync(cancellationToken);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(Employee employee, CancellationToken cancellationToken)
        {
            SQLDbContext context = await AddEmployeeAsync(employee, cancellationToken);
            return Ok();
        }

        

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(Employee employee, CancellationToken cancellationToken)
        {
            var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            var user = await context.Employees.FirstOrDefaultAsync(x => x.Id == employee.Id, cancellationToken);
            if (user == null) StatusCode((int)HttpStatusCode.BadRequest);
            context.Employees.Remove(user!);
            await AddEmployeeAsync(employee, cancellationToken);
            return Ok();
        }



        private async Task<SQLDbContext> AddEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            var context = await contextFactory.CreateDbContextAsync(cancellationToken);

            var department = await context.Departments.FirstOrDefaultAsync(x => x.Name == employee.Department.Name, cancellationToken);
            if (department == null)
            {
                var departmentEntry = await context.Departments.AddAsync(new Department
                {
                    Name = employee.Department.Name,
                    Address = employee.Department.Address,

                }, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);
                department = departmentEntry.Entity;
            }
            var user = await context.Employees.AddAsync(new Employee
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DepartmentId = department.Id
            }, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return context;
        }
    }
}

