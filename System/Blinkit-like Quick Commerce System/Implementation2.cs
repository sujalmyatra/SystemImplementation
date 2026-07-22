using System.Data.Common;
using System.Formats.Asn1;
using System.Linq.Expressions;

public abstract class BaseEntity
{
    public Guid Id{get; protected set;}
}

public abstract class SoftDeletableEntity : BaseEntity
{
    public bool IsDeleted{get; set;}
    public void MarkAsDeleted()
    {
        IsDeleted = true;
    }
}

public class Customer : SoftDeletableEntity
{
    public string Name{get; set;}
    public string PhoneNumber{get; set;}

    public Cart Cart{get; set;}
}
public class Product : SoftDeletableEntity
{
    public string ProductName{get; set;}
    public decimal Price{get; set;}

   
    public bool IsActive{get; set;} = true;

}
public class Store : SoftDeletableEntity
{
    public string Location{get; set;}

    public ICollection<Stock> Stocks {get; set;} = [];
    public ICollection<Order> Orders {get; set;} = [];
}

public enum StockStatus
{
    Available,
    LowStock,
    OutOfStock
}
public class Stock : BaseEntity
{
    public StockStatus Status{get; set;}
    public int Quantity{get; set;}

    public Guid ProductId{get; set;}
    public Product Product{get; set;}

    public Guid StoreId{get; set;}
    public Store Store{get; set;}
}
public class Cart : BaseEntity
{
    public Guid CustomerId{get; set;}
    public Customer Customer{get; set;}

    public ICollection<Order> Orders {get; set;} = [];
}
public enum OrderStatus
{
    Placed,
    confirmed, 
    OutforDelivery,
    FullFilled,
    Canceled
}
public class Order : SoftDeletableEntity
{
    public OrderStatus Status{get; set;} = Order.Pending;
    public DateTime PlacedAt{get; set;}
    public DateTime? FullFilledAt{get; set;}
    public DateTime? CanceledAt{get; set;}

    public Guid CartId{get; set;}
    public Cart Cart{get; set;}

    public Guid StoreId{get; set;}
    public Store Store{get; set;}

    public Guid? DeliveryPartnerId{get; set;}
    public DeliveryPartner? DeliveryPartner{get; set;}

    public Payment? Payment{get; set;}

    public ICollection<OrderItem> OrderItems {get; set;} = [];
}
public class OrderItem : BaseEntity
{
    public Guid OrderId{get; set;}
    public Order Order{get; set;}

    public Guid ProductId{get; set;}
    public Product Product{get; set;}

    public int Quantity{get; set;}
}
public enum PaymentStatus
{
    Pending, 
    UnPaid,
    Paid,
    Refunded
}
public class Payment : BaseEntity
{
    public PaymentStatus Status{get; set;} = PaymentStatus.Pending;
    public decimal TotalAmount{get; set;}
    public DateTime? PaidAt{get; set;}


    public int RetryCount{get; set;}
    public DateTime? LastAttemptAt{get; set;}

    public string? FailureReason{get; set;}

    public Guid OrderId{get; set;}
    public Order Order{get; set;}
}
public class DeliveryPartner : SoftDeletableEntity
{
    public string Name{get; set;}
    public string PhoneNumber{get; set;}
    public bool IsAvailable{get; set;} = true;
    public bool IsActive{get; set;} = false;

    
     public ICollection<Order> Orders {get; set;} = [];
}

//Infrastructure

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers  => Set<Customer>();
    public DbSet<Cart> Carts  => Set<Cart>();
    public DbSet<Store> Stores  => Set<Store>();

    public DbSet<Order> Orders  => Set<Order>();
    public DbSet<OrderItem> OrderItems  => Set<OrderItem>();
    public DbSet<Stock> Stocks  => Set<Stock>();
    public DbSet<Product> Products  => Set<Product>();
    public DbSet<Payment> Payments  => Set<Payment>();
    public DbSet<DeliveryPartner> DeliveryPartners  => Set<DeliveryPartner>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Customer>(e =>
        {
            e.HasKey(x => x.Id);

            e.HasQueryFilter(x => !x.IsDeleted);
        });

        builder.Entity<Product>(e =>
        {
            e.HasKey(x => x.Id);

            e.HasQueryFilter(x => !x.IsDeleted);
        });

        builder.Entity<Store>(e =>
        {
            e.HasKey(x => x.Id);

            e.HasQueryFilter(x => !x.IsDeleted);
        });

        builder.Entity<DeliveryPartner>(e =>
        {
            e.HasKey(x => x.Id);

            e.HasQueryFilter(x => !x.IsDeleted);
        });

        builder.Entity<Order>(e =>
        {
            e.HasKey(x => x.Id);

            e.HasQueryFilter(x => !x.IsDeleted);

            e.HasOne(x => x.Cart)
            .HasMany(c => c.Orders)
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehaviour.Restrict);

            e.HasOne(x => x.Store)
            .HasMany(c => c.Orders)
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehaviour.Restrict);

            e.HasOne(x => x.DeliveryPartner)
            .HasMany(c => c.Orders)
            .HasForeignKey(x => x.DeliveryPartnerId)
            .OnDelete(DeleteBehaviour.Restrict);

            e.HasMany(x => x.OrderItems)
            .WithOne(c => c.Order)
            .HasForeignKey<OrderItem>(x => x.OrderId)
            .OnDelete(DeleteBehaviour.Restrict);

            e.HasOne(x => x.Payment)
            .WithOne(c => c.Order)
            .HasForeignKey<Payment>(x => x.OrderId)
            .OnDelete(DeleteBehaviour.Restrict);
        });

    }   
}

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
public interface IUnitOfWork
{  
    IGenericRepository<Customer> Customers{get;}
    IGenericRepository<Cart> Carts{get;}
    IGenericRepository<Store> Stores{get;}
    IGenericRepository<Order> Orders{get;}
    IGenericRepository<OrderItem> OrderItems{get;}
    IGenericRepository<Stock> Stocks{get;}IGenericRepository<Product> Products{get;}
    IGenericRepository<Payment> Payments{get;}
    IGenericRepository<DeliveryPartner> DeliveryPartners{get;}

    Task<int> SaveChnagesAsync();
}
public class GenericRepository<T>(AppDbContext context):IGenericRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _set = context.Set<T>();
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _set.FindAsync();
    }
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _set.AsNoTracking().ToListAsync();
    }
    public async Task AddAsync(T entity)
    {
        await _set.AddAsync(entity);
    }
    public void Update(T entity)
    {
        _set.Update(entity);
    }
    public void Delete(T entity)
    {
        if(entity is SoftDeletableEntity softDeletable)
        {
            softDeletable.IsDeleted = true;
            _set.Update(entity);
        }
        else{
        _set.Remove(entity);
        }
    }
}
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{  
    IGenericRepository<Customer> Customers{get;}
    = new GenericRepository<Customer>(context);
    IGenericRepository<Cart> Carts{get;}
    = new GenericRepository<Cart>(context);
    IGenericRepository<Store> Stores{get;}
    = new GenericRepository<Store>(context);
    IGenericRepository<Order> Orders{get;}
    = new GenericRepository<Order>(context);
    IGenericRepository<OrderItem> OrderItems{get;}
    = new GenericRepository<OrderItem>(context);
    IGenericRepository<Stock> Stocks{get;}
    = new GenericRepository<Stock>(context);
    IGenericRepository<Product> Products{get;}
    = new GenericRepository<Product>(context);
    IGenericRepository<Payment> Payments{get;}
    = new GenericRepository<Payment>(context);
    IGenericRepository<DeliveryPartner> DeliveryPartners{get;}
    = new GenericRepository<DeliveryPartner>(context);

    public async Task<int> SaveChnagesAsync()
    {
        return await context.SaveChnagesAsync();
    }
}

//report
//5. Fast-Moving Product Report
// Show products ordered most frequently in last month.
var today = new DateTime();
var startOfthismonth = new DateTime(today.Year, today.Month, 1);
var startOflastmonth = startOfthismonth.AddMonths(1);

var lastmonthOrders = Orders
.Where(x => x.FullFilledAt >= startOflastmonth &&  x.FullFilledAt < startOfthismonth && OrderStatus.FullFilled);

var result = lastmonthOrders.Join(OrderItems, lm => lm.Id, oi => oi.OrderId,(lm, oi) => new {lm, oi})
.Join(Products, x => x.oi.ProductId, p => p.Id, (x, p) => new
 {
        ProductId = p.Id,
        p.ProductName,  

 })
 .GroupBy(x => new {x.ProductId, x.ProductName})
 .Where(g => g.Count() > 5)
 .Select(g => new
 {
     g.Key.ProductId, g.Key.ProductName,
     SellCount = g.Count()
 });
 public record PaymentResponseDto(Guid PaymentId, Guid OrderId, PaymentStatus Status, int RetryCount,  string? Note);
public interface IPaymentService
{    
    Task<PaymentResponseDto> ProcessAsync(Guid paymentId);
}
public class PaymentService(IUnitOfWork uow, IPaymentGateway gateway) : IPaymentService
{
    private const int MaximumAttempts = 6;

    public async Task<PaymentResponseDto> ProcessAsync(Guid paymentId)
    {
        var payment = await uow.Payments.GetByIdAsync(paymentId);

        if(payment is null)
            throw new KeyNotFoundException($"Payment wiht Id{paymentId} Not found");

        if(payment.Status == PaymentStatus.Paid)
        {
            return new PaymentResponseDto(paymentId, payment.OrderId, payment.Status, payment.RetryCount, "Payment has already done");
        }

        if(payment.Status == PaymentStatus.Refunded)
        {
            throw new InvalidOperationException("A refunded payment canot be processed");
        }

        if(payment.RetryCount > MaximumAttempts)
            throw new PAymentReteyLimitExecededException("max attempts reached");

        var order = await uow.Orders.GetByIdAsync(payment.OrderId);

        if(order is null)
            throw new KeyNotFoundException($"Order wiht Id{paymentId} Not found");
        
        while(payment.RetryCount < MaximumAttempts)
        {
            payment.RetryCount++;
            payment.LastAttemptAt = DateTime.UtcNow;

            payment.Status = PaymentStatus.Pending;

            payment.FailureReason = null;

            try
            {
                var isSuccess = await gateway.ProcessAsync(paymentId, payment.TotalAmount);

                if(isSuccess)
                {
                    payment.Status = PaymentStatus.Paid;
                    payment.PaidAt = DateTime.UtcNow;

                    order.Status = OrderStatus.confirmed;

                    await uow.SaveChnagesAsync();

                    return new PaymentResponseDto(payment.Id,payment.OrderId, payment.Status, payment.RetryCount, "successfull paymenyt");
                }
                payment.Status = PaymentStatus.Faild;
                payment.FailureReason = "PAymenyGateway rejected payment";
            }
            catch(Exception ex)
            {
                payment.Status = PaymentStatus.Faild;
                payment.FailureReason = ex.Message;
            }
            
        }
        throw new PaymentRetryLimitReachedException($"PAyment failed after maximum attempts :: {MaximumAttempts}");
        

    }
}