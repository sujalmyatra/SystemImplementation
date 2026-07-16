public enum PaymentStatus
{
    Pending,
    Processing,
    Success,
    Failed,
    Refunded
}
public enum OrderStatus
{
    PendingPayment,
    Confirmed,
    Cancelled,
    Completed,
}

public enum PaymentMethod
{
    UPI,
    Cash,
    Card
}
public enum PaymentActivity
{
    Retry,
    Succeed,
    Fail,
    DuplicateAttempt
}

public interface IPaymentStrategy
{
    PaymentMethod PaymentMethod{get;}
    Task<PaymentResult> ProcessAsync(PaymentTransaction transaction);
}
public class UPIPaymentStrategy : IPaymentStrategy
{
    public PaymentMethod PaymentMethod{get;} => PaymentMethod.UPI;
    public async Task<PaymentResult> ProcessAsync(PaymentTransaction transaction)
    {
        await Task.Delay(100);

        return PaymentResult.Success("UPI Payment Successfull");
    }
}
public class CashPaymentStrategy : IPaymentStrategy
{
    public PaymentMethod PaymentMethod{get;} => PaymentMethod.Cash;
    public async Task<PaymentResult> ProcessAsync(PaymentTransaction transaction)
    {
        await Task.Delay(100);

        return PaymentResult.Success("Cash Payment Successfull");
    }
}
public class CardPaymentStrategy : IPaymentStrategy
{
    public PaymentMethod PaymentMethod{get;} => PaymentMethod.Card;
    public async Task<PaymentResult> ProcessAsync(PaymentTransaction transaction)
    {
        await Task.Delay(100);

        return PaymentResult.Success("Card Payment Successfull");
    }
}

public record PaymentResult(bool ISuccessfull, string? TransactionReference, string? ErrorCode, string? Errormessage)
{
    public static PaymentResult Success(string transactionReference)
    {
        return new PaymentResult(true, transactionReference, null, null);
    }
    public static PaymentResult Failure(string errorCode, string errorMessage)
    {
        return new PaymentResult(false, null, errorCode, errorMessage);
    }
}

public interface IPaymentStrategyFactory
{
    IPaymentStrategy GetStrategy(PaymentMethod method);
}
public class PaymentStrategyFactory(List<IPaymentStrategy> strategies) : IPaymentStrategyFactory
{
    private readonly Dictionary<PaymentMethodm, IPaymentStrategy> _strategies = strategies.ToDictionary(strategy => strategy.PaymentMethod);

    public IPaymentStrategy GetStrategy(PaymentMethod method)
    {
        if(!_strategies.TryGetValue(method, out var strategy))
            throw new InvalidOperationException("Payment Method does not Supported by system");

        return strategy;
    }
}


public abstract class BaseEntity
{
    public Guid Id {get; set;} = Guid.NewGuid();
}

public class Order : BaseEntity
{
    public OrderStatus Status{get; set;}
    public Guid CustomerId{get; set;}
    public decimal TotalAmount{get; set;}
    public DateTime CreatedAt{get; set;}//baseenitty
    public DateTime? ConfirmedAt{get; set;}
    public ICollection<PaymentTransaction> PaymentTransactions{get; set;}
    = new List<PaymentTransaction>();

    public void Confirm()
    {
        if(Status != OrderStatus.PendingPayment)
            throw new InvalidOperationException("An Order with pending payment can be confirmed");

        var hasTransaction = PaymentTransaction.Any(x => x.Status == PaymentStatus.Success);

        if(!hasTransaction)
            throw new InvalidOperationException("Order with only successfull payment can be confirmed");

        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
    }
    public void Cancel()
    {
        if(Status == OrderStatus.Completed)
            throw new InvalidOperationException("An Order already completed can not be canceled");

        Status = OrderStatus.Cancelled;

    }
}
public class PaymentTransaction  : BaseEntity
{
    public PaymentMethod PaymentMethod{get; set;}
    public PaymentStatus Status{get; set;} = PaymentStatus.Pending;
    public decimal Amount{get; set;}
    public DateTime CreatedAt{get; set;} = DateTime.UtcNow;
    public DateTime? CompletedAt{get; set;}
    public Guid OrderId {get; set;}
    public Order Order {get; set;}

     public string? FailureCode { get; set; }

    public string? FailureMessage { get; set; }

    public ICollection<PaymentAuditLog> paymentAuditLogs{get; set;} = new List<PaymentAuditLog>();

    public void StartProcessing()
    {
        if(Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only a pending payment can be started");
        
        SetStatus(PaymentStatus.Processing);
    }
    public void MarkAsFailed(string failureCode, string failureMessage)
    {
         if(Status is PaymentStatus.Success or PaymentStatus.Refunded)
            throw new InvalidOperationException("A successfullor refunded payment can not be marked as failed");
            
        SetStatus(PaymentStatus.Failed);

        FailureCode = failureCode;
        FailureMessage = failureMessage;
    }
    public void MarkAsSuccessfull()
    {
         if(Status != PaymentStatus.Processing)
            throw new InvalidOperationException("Only a Processing payment can be successfull");

        SetStatus(PaymentStatus.Success);
        CompletedAt = DateTime.UtcNow;
        FailureCode = null;
        FailureMessage = null;
    }
    private void SetStatus(PaymentStatus status)
    {
        Status = status;
    }
}

public class PaymentAuditLog : BaseEntity
{
    public Guid PaymentTransactionId {get; set;}
    public  PaymentTransaction  PaymentTransaction {get; set;} = null!;
    public PaymentMethod PaymentMethod{get; set;}
    public PaymentActivity Activity{get; set;}
    public DateTime TimeStamp{get; set;} = DateTime.UtcNow;
}



//Infrastructure

public class AppDbContext(DbContextOptions<AppDbContext> options ) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<PaymentAuditLog> PaymentAuditLogs => Set<PaymentAuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Order>(e =>
        {
            e.ToTable("Orders");

            e.HasMany(e => e.PaymentTransactions)
            .WithOne(e => e.Order)
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehaviour.Restrict);
        });
        
       
        
        builder.Entity<PaymentAuditLog>(e =>
        {
            e.ToTable("PaymentAuditLogs");
            
            e.HasOne(e => e.PaymentTransaction)
            .WithMany(e => e.PaymentAuditLogs)
            .HasForeignKey(e => e.PaymentTransactionId)
            .OnDelete(DeleteBehaviour.Restrict);
        });
    }
    public override Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }
}

//Reposotories
public interface IGenericRepository<T>
    where T : BaseEntity
{
    Task<T?> GetByIdAsync(
        Guid id);

    Task<IEnumerable<T>> GetAllAsync();

    Task AddAsync(T entity);

    void Update(T entity);

    void Delete(T entity);
}

public class GenericRepository<T> : IGenericRepository<T>
    where T : BaseEntity
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    public GenericRepository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(
        Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await DbSet
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        DbSet.Remove(entity);
    }
}

public interface IUnitOfWork
{
    IOrderRepository Orders {get;}
    IPaymentTransactionRepository PaymentTransactions {get;}
    IGenericRepository<PaymentAuditLog> PaymentAuditLogs {get;}

    Task<int> SaveChangesAsync();

}
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public IOrderRepository Orders {get;} = new OrderRepository(context);
    public IPaymentTransactionRepository PaymentTransactions {get;}
     = new PaymentTransactionRepository(context);
    public IGenericRepository<PaymentAuditLog> PaymentAuditLogs {get;}
     = new GenericRepository<PaymentAuditLog>(context);

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}

//specialized Reposotories
public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order?> GetWithTransactionsAsync(
        Guid orderId);
}

public class OrderRepository
    :GenericRepository<Order>, IOrderRepository
{
    public async Task<Order?> GetWithTransactionsAsync(
     Guid orderId)
    {
        return await context.Orders.Include(x => x.PaymentTransactions)
        .FirstOrDefaultAsync(x => x.OrderId == orderId);
    }
}

public interface IPaymentTransactionRepository : IGenericRepository<PaymentTransaction>
{
    Task<bool> HasSuccessfulPaymentAsync(
        Guid orderId);
}

public class PaymentTransactionRepository(AppDbContext context) : GenericRepository<PaymentTransaction>, IPaymentTransactionRepository
{
    public async Task<bool> HasSuccessfulPaymentAsync(
        Guid orderId)
    {
        return await context.PaymentTransactions.AnyAsync(x => x.OrderId == orderId &&
        x.Status == PaymentStatus.Success
        );
    }
}

//Service
public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(Guid orderId,PaymentMethod paymentMethod);
}
public class PaymentService(IUnitOfWork uow, IPaymentStrategyFactory factory) : IPaymentService
{
    public async Task<PaymentResult> ProcessPaymentAsync(Guid orderId,PaymentMethod paymentMethod)
    {
        var order = await uow.Orders.GetByIdAsync(orderId);

        if(order is null)
            return PaymentResult.Failure("ORDER_NOT_FOUND","Order was not found");
        
        bool alreadyPaidOrder = await uow.PaymentTransactions.HasSuccessfulPaymentAsync(order.Id);

        if(alreadyPaidOrder)
            return PaymentResult.Failure("Duplicate_Payment", "ORder has been already Paid");

        var transaction = new PaymentTransaction
        {
            OrderId =  order.Id,
            Amount = order.TotalAmount,
            PaymentMethod = paymentMethod
        };

        await uow.PaymentTransactions.AddAsync(transaction);

        transaction.StartProcessing();

        IPaymentStrategy strategy = factory.GetStrategy(paymentMethod);

        PaymentResult result = await strategy.ProcessAsync(transaction);


        if(result.ISuccessfull)
        {
            transaction.MarkAsSuccessfull();
            order.Confirm();

            transaction.paymentAuditLogs.Add(new PaymentAuditLog{PaymentMethod = paymentMethod,
            Activity = PaymentActivity.Succeed});
        }
        else
        {
            transaction.MarkAsFailed(result.ErrorCode, result.Errormessage);
            transaction.paymentAuditLogs.Add(new PaymentAuditLog{PaymentMethod = paymentMethod,
            Activity = PaymentActivity.Fail});
        }

        await uow.SaveChangesAsync();

        return result;
    }
}