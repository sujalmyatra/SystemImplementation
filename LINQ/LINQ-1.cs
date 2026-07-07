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
}
}