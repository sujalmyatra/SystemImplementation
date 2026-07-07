using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;

namespace practce;
public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int? DepartmentId { get; set; }
    public decimal Salary { get; set; }
}

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int DepartmentId { get; set; }
}

public class WorkLog
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int ProjectId { get; set; }
    public int Hours { get; set; }
}
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Status { get; set; } = ""; 
    // Paid, Failed, Refunded
    public DateTime PaidAt { get; set; }
}
public class Program
{
    public static void Main()
    {
         List<Employee> employees = new();
        List<Department> deparwtments = new();
        List<WorkLog> workLogs = new();
        List<Project> projects = new();

        List<Customer> customers = new();
        List<Category> categories = new();
        List<Product> products = new();
        List<Order> orders = new();
        List<OrderItem> orderItems = new();



    var today = DateTime.Today;
    var last10days = today.AddDays(-10);
    var thismonth = today.Month;
    var thisyear = today.Year;

    var firstdaythismonth = new DateTime(thisyear, thismonth, 1);
    var firstdayprevsmonth = firstdaythismonth.AddMonths(-1);

    //Q1
    // Show all orders placed in the last 10 days with customer name and order date.
    var e = orders.Where(x => x.OrderDate >= last10days && x.OrderDate < today).Join(customers, o => o.CustomerId, c => c.Id, (o, c) => new {CName = c.Name, ORderDate = o.OrderDate});
    // Q2
    // Show customer-wise order count for the last 10 days.
    orders.Where(x => x.OrderDate >= last10days && x.OrderDate < today).GroupBy(x => x.CustomerId).Select(x => new {customer = x.Key, Count = x.Count()});
    // Q3
    // Show customer-wise total purchase amount for last month.
    var ee = orders.Where(x => x.OrderDate >= firstdayprevsmonth && x.OrderDate < firstdaythismonth )
    .Join(customers, o => o.CustomerId, c => c.Id, (o, c) => new {Order = o, Customer = c})
    .Join(orderItems, o => o.Order.Id, oi => oi.Id, (oc, oi) => new {CustomerName = oc.Customer.Name, Amount = oi.Quantity * oi.UnitPrice})
    .GroupBy(c => c.Customer.Name).Select(s => new {Customer = s.Key, Total = s.Sum(o => o.Amount)});

    // Q4
    // Show all customers with their last month order count. If no order, count should be 0.

    // Q5
    // Show all products with total sold quantity in the last 10 days. If product was not sold, show quantity 0.

    // Q6
    // Show date-wise total orders and total revenue for the last 10 days.

    // Q7
    // Show category-wise total sales amount for last month.

    // Q8
    // Show all orders with customer name and payment status. If payment is not found, show "Payment Pending".

    // Q9
    // Find customers who placed more than 3 orders last month.

    // Q10
    // Show products that were not sold last month.

    // Q11
    // Show payment-status-wise order count for last month. Orders without payment should come under "Payment Pending".

    // Q12
    // Show customer-wise total products purchased last month.


    //Q1
    // Show all employees with their department name.
    var r = employees.Join(deparwtments, e => e.DepartmentId, d => d.Id, (e, d) => new{Employee = e.Name, Department = d?.Name ?? "No Department"});

    // Q2
    // Show all employees with their department name. If employee has no department, show "No Department".
    var e = employees.LeftJoin(deparwtments, e => e.DepartmentId, d => d.Id, (e, d) => new {Name = e.Name, Department = d?.Name ?? "No Department"});

    // Q3
    // Show department-wise employee count.
    var c = employees.Join(deparwtments, e => e.DepartmentId, d => d.Id, (e, d) => new{Employee = e, Department = d}).GroupBy(x => x.Department.Name).Select(x=>new { Department = x.Key, cg = x.Count()});
    // Q4
    // Show department-wise total salary.
    var s = employees.Join(deparwtments, e => e.DepartmentId, d => d.Id, (e,d) => new {Department = d, Employee = e}).GroupBy(x => x.DepartmentId).Select(x => new {Department = x.Key, Total = x.Sum(x => x.Salary)});
    // Q5
    // Show all departments with their employees.
    var t = deparwtments.GroupJoin(employees, d => d.Id, e => e.DepartmentId,(d, empgrp) =>  new {Department = d.Name, Employee = empgrp.Select(x => x.Name)});
    // Q6
    // Show all departments even if they have no employees.
    var f = deparwtments.GroupJoin(employees, d => d.Id, e => e.DepartmentId, (d, empgrp) => new {Department = d.Name, Employee = empgrp.Any()? empgrp.Select(x => x.Name) : "No Employee" });
    // Q7
    // Show employee name, project title, and hours worked.
    var y = workLogs.Join(employees, w => w.EmployeeId, e => e.Id, (w, e) => new {Work = w, Employee = e})
    .Join(projects, w => w.Work.ProjectId, p => p.Id, (w, p )=> new {Employee = w.Employee.Name, Project = p.Title, Hours = w.Work.Hours} );
    // Q8
    // Show project-wise total hours worked.
    var q = projects.Join(workLogs, p => p.Id, w => w.ProjectId, (p, w) => new {Project = p, Work = w})
    .GroupBy(x => x.Project.Title).Select(x => new { Project = x.Key, Hours = x.Sum(h => h.Work.Hours)});


    }
}