using System.Net.NetworkInformation;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace practce;
public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    public decimal Salary { get; set; }

    public int? DepartmentId { get; set; }
}

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public decimal Budget { get; set; }
    public int? EmployeeId { get; set; }
}


public class Program
{




public static void Main()
{
    var departments = new List<Department>();
    var employees = new List<Employee>();
    var projects = new List<Project>();

    //1 - Get all employees with their department name.
    //  
    var res = employees.Join(departments, e => e.DepartmentId, d => d.Id, (e,d) => new {EmployeeName = e.Name, Department = d.Name});

    //2 - Get all employees with department name.
    //    If employee has no department, show "No Department".
    var rr = employees.LeftJoin(departments, e => e.DepartmentId, d => d.Id, (e, d) => new {EmploayeeName = e.Name, Department = d.Name ?? "No Department"});
    var r = employees.GroupJoin(departments, e => e.DepartmentId, d => d.Id, (e, deptgrp) => new { Employee = e, grp = deptgrp })
        .SelectMany(x => x.grp.DefaultIfEmpty(),
                    (x, grp) => new {EmployeeName = x.Employee.Name, Department = grp?.Name ?? "No Department"});

    //3 - Get all departments with employee count. Departments with zero employees should also come.
    var t = departments.GroupJoin(employees, d => d.Id, e => e.DepartmentId, (d, empgrp) => new {Department = d.Name, EmpCount = empgrp.Count()});
    t = from d in departments
        join e in employees
        on d.Id equals e.DepartmentId into g
        select new
        {
          Department = d.Name,
          EmpCount = g.Count()
        };

    //4 = Get department-wise total salary and average salary.
    var g = employees.GroupBy(x => x.DepartmentId)
        .Select(x => new {Department = x.Key, Total = x.Sum(c => c.Salary), Avg = x.Average(c => c.Salary)});

    var f = employees
    .Join(departments, e => e.DepartmentId, d => d.Id, (e, d) => new {Employee = e, DepartmentName = d.Name})
    .GroupBy(x => x.DepartmentName)
    .Select(g => new {Department = g.Key, TotalSalary = g.Sum(x => x.Employee.Salary)});

    // 5 - Get all employees with their project names. Employees without projects should also come.
    
}
}
