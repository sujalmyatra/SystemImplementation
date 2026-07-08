using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;

namespace practce;
public class Department {public int Id { get; set; } public string Name { get; set; } = "";}

public class Employee{public int Id { get; set; } public string Name { get; set; } = ""; public int? DepartmentId { get; set; } public decimal Salary { get; set; }}

public class Project
{ public int Id { get; set; } public string Title { get; set; } = ""; public int DepartmentId { get; set; }}

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
        List<Department> departments = new();
        List<WorkLog> workLogs = new();
        List<Project> projects = new();

        List<Customer> customers = new();
        List<Category> categories = new();
        List<Product> products = new();
        List<Order> orders = new();
        List<OrderItem> orderItems = new();
        List<Payment> payments = new();
        



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
    var lastmonthorders =  orders.Where(x => x.OrderDate >= firstdayprevsmonth && x.OrderDate < firstdaythismonth);
    var OrderCount = customers.GroupJoin(lastmonthorders, c => c.Id, o => o.CustomerId, (c, ordergrp) => new {Customer = c.Name, OrderCount = ordergrp.Count()});
    // Q5
    // Show all products with total sold quantity in the last 10 days. If product was not sold, show quantity 0.
    var last10daysorders = orders.Where(x => x.OrderDate >= last10days && x.OrderDate < today );
    var orderanditems = last10daysorders.Join(orderItems, o => o.Id, oi => oi.OrderId, (o, oi) => new {Orders = o, OrderItems = oi}); 
    var totalproducts = products.GroupJoin(orderanditems, p => p.Id, o => o.OrderItems.ProductId, (p, productgrp) => new {Product = p.Name, Quantity = productgrp.Sum(x => x.OrderItems.Quantity)});

    // Q6
    // Show date-wise total orders and total revenue for the last 10 days.
    var ordertoorderitem = orders.Where(x => x.OrderDate >= last10days && x.OrderDate <= today)
    .Join(orderItems, o => o.Id, Oi => Oi.OrderId, (o, oi) => new {OrderDate = o.OrderDate.Date, OrderId = o.Id, Amount = oi.Quantity * oi.UnitPrice})
    .GroupBy(x => x.OrderDate)
    .Select(g => new
    {
       OrderDate = g.Key,
        TotalRevenue = g.Sum(x => x.Amount),
        TotalOrder = g.Select(x =>  x.OrderId).Distinct().Count()
    });

    // Q7
    // Show category-wise total sales amount for last month.
    var lastMonthOrderItemsAll = orders.Where(x => x.OrderDate >= firstdayprevsmonth && x.OrderDate < firstdaythismonth)
        .Join(orderItems, o => o.Id, oi => oi.OrderId, (o, oi) => oi);

        var productsWithAmount = lastMonthOrderItemsAll.Join(products, oi => oi.ProductId, p => p.Id, (oi, p) => new {OrderItemAmount = oi.Quantity * oi.UnitPrice, Product = p});

        var categoryWiseSales = productsWithAmount.Join(categories, x => x.Product.CategoryId, c => c.Id, (x, c) => new {Category = c.Name, Amount = x.OrderItemAmount})
        .GroupBy(g => g.Category)
        .Select(x => new {Category = x.Key, TotalSales = x.Sum(t => t.Amount)});

    // Q8
    // Show all orders with customer name and payment status. If payment is not found, show "Payment Pending".
    var orderWithCustomer = orders.Join(customers, o => o.CustomerId, c => c.Id, (o, c) => new {Order = o, CustomerName = c.Name});
    var ord = orderWithCustomer.LeftJoin(payments, oc =>  oc.Order.Id, p => p.OrderId,(oc, p) => new {OrderId = oc.Order.Id, Customer = oc.CustomerName, OrderDate = oc.Order.OrderDate, PaymentStatus = p?.Status ?? "Payment Pending"} );

    var usingOlderWay = orderWithCustomer.GroupJoin(payments, oc => oc.Order.Id, p => p.OrderId, (oc, paymentgrp) => new {Order = oc.Order, Customer = oc.CustomerName, PaymentGroup - paymentgrp})
    .SelectMany(x => x.PaymentGroup.DefaultIfEmpty(),
    (x, grp) => new {OrderID = x.Order.OrderId, CustomerName = x.Customer, OrderDate = x.OrderDate, PaymentStatus = grp?.Status ?? "Payment Pending"});

    // Q9
    // Find customers who placed more than 3 orders last month.
      lastmonthorders.Join(customers, lm => lm.CustomerId, c => c.Id, (lm, c) => new {Orders = lm, Customer = c})
     .GroupBy(x => new {x.Customer.Id, x.Customer.Name})
     .Where(x => x.Count() > 3)
     .Select(x => new {CustomerId = x.Key.Id, CustomerName = x.Key.Name, TotalOrders = x.Count()});

    // Q10
    // Show products that were not sold last month.
    var soldProducts = lastmonthorders.Join(orderItems, lm => lm.Id, oi => oi.OrderId, (lm, oi) => new {OrderItemProductsId = oi.ProductId});
    //1st
    var notSold = products.Where(x => !soldProducts.Contains(x.Id))
    .Select(p => new{ Products = p.Name});

    //2nd
    notSold = products.LeftJoin(soldProducts, p => p.Id, ns => ns.OrderItemProductsId, (p, s) => new {Product = p, SoldProductId = s})
    .Where(x => x.SoldProductId == null)
    .Select(x => x.Product.Name);

    // Q11
    // Show payment-status-wise order count for last month. Orders without payment should come under "Payment Pending".
    var lastMonthOP = lastmonthorders.LeftJoin(payments, lm => lm.Id, p => p.OrderId, (lm ,p) => new {OrderId = lm.Id,PaymentStatus = p?.Status ?? "Pending Payment"})
    .GroupBy(x => x.PaymentStatus)
    .Select(g => PaymentStatus = g.Key, OrderCount = g.Count()); 

    var lastMonthOP = lastmonthorders.GroupJoin(payments, lm => lm.Id, p => p.OrderId, (lm, paymentgrp) => new {OrderId = lm.Id, PaymentGRP = paymentgrp})
    .SelectMany(x => x.PaymentGRP.DefaultIfEmpty(),
    (x, grp) => new {Order = x.OrderId, PaymentStatus = grp?.Status ?? "Pending Payment"})
    .GroupBy(x => x.PaymentStatus)
    .Select(g => new {Status = g.Key, Count = g.Count()});
    
    // Q12
    // Show customer-wise total products purchased last month.
    var tt = lastmonthorders.Join(orderItems, lm => lm.Id, oi => oi.OrderId, (lm, oi) => new {Order = lm, OrderItem = oi})
    .Join(customers, o => o.Order.CustomerId, c => c.Id, (o, c) => new {CustomerName = c.Name, ProductQuantity = o.OrderItem.Quantity})
    .GroupBy(x => x.CustomerName)
    .Select(g => new {Customer = g.Key, TotalProducts = g.Sum(x => x.ProductQuantity)});

    // Set One
    //Q1
    // Show all employees with their department name.
    var r = employees.Join(departments, e => e.DepartmentId, d => d.Id, (e, d) => new{Employee = e.Name, Department = d?.Name ?? "No Department"});

    // Q2
    // Show all employees with their department name. If employee has no department, show "No Department".
    var e = employees.LeftJoin(departments, e => e.DepartmentId, d => d.Id, (e, d) => new {Name = e.Name, Department = d?.Name ?? "No Department"});

    // Q3
    // Show department-wise employee count.
    var c = employees.Join(departments, e => e.DepartmentId, d => d.Id, (e, d) => new{Employee = e, Department = d}).GroupBy(x => x.Department.Name).Select(x=>new { Department = x.Key, cg = x.Count()});
    // Q4
    // Show department-wise total salary.
    var s = employees.Join(departments, e => e.DepartmentId, d => d.Id, (e,d) => new {Department = d, Employee = e}).GroupBy(x => x.Department.Name).Select(x => new {Department = x.Key, Total = x.Sum(x => x.Employee.Salary)});
    // Q5
    // Show all departments with their employees.
    var t = departments
    .GroupJoin(employees, d => d.Id, e => e.DepartmentId,(d, empgrp) =>  new {Department = d.Name, Employee = empgrp.Select(x => x.Name)});
    // Q6
    // Show all departments even if they have no employees.
    var f = departments
    .GroupJoin(employees, d => d.Id, e => e.DepartmentId, (d, empgrp) => new {Department = d.Name, Employee = empgrp.Any() ? empgrp.Select(x => x.Name) : new List<string>{"No Employee" }});

    // Q7
    // Show employee name, project title, and hours worked.
    var y = workLogs.Join(employees, w => w.EmployeeId, e => e.Id, (w, e) => new {Work = w, Employee = e})
    .Join(projects, w => w.Work.ProjectId, p => p.Id, (w, p )=> new {Employee = w.Employee.Name, Project = p.Title, WorkedHours = w.Work.Hours} );

    // Q8
    // Show project-wise total hours worked.
    var q = projects.Join(workLogs, p => p.Id, w => w.ProjectId, (p, w) => new {Project = p, Work = w})
    .GroupBy(x => x.Project.Title).Select(x => new { Project = x.Key, Hours = x.Sum(h => h.Work.Hours)});



    //Upload 

    // public class Department {public int Id { get; set; } public string Name { get; set; } = "";}

    // public class Employee{public int Id { get; set; } public string Name { get; set; } = ""; public int? DepartmentId { get; set; } public decimal Salary { get; set; }}

    // public class Project
    // { public int Id { get; set; } public string Title { get; set; } = ""; public int DepartmentId { get; set; }}

     //Q1
    // Show all employees with their department name.

    // Q2
    // Show all employees with their department name. If employee has no department, show "No Department".

    // Q3
    // Show department-wise employee count.

    // Q4
    // Show department-wise total salary.

    // Q5
    // Show all departments with their employees.

    // Q6
    // Show all departments even if they have no employees.

    // Q7
    // Show employee name, project title, and hours worked.

    // Q8
    // Show project-wise total hours worked.

    }
}