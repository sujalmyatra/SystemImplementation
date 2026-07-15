

using System.Dynamic;

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
    Card,
    Stripe
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
    public DateTime CreatedAt{get; set;}
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
        if(Status != OrderStatus.Completed)
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
public enum PaymentActivity
{
    Retry,
    Succeed,
    Fail,
    DuplicateAttempt

}
public class PaymentAuditLog : BaseEntity
{
    public Guid OrderId {get; set;}
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
            .WithForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehaviour.Restrict);
        });
        
        builder.Entity<PaymentTransaction>(e =>
        {
            e.ToTable("PaymentTransactions");
            e.HasOne(e => e.Order)
            .WithMany(e => e.PaymentTransaction)
            .WithForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehaviour.Restrict);
        });
        
        builder.Entity<PaymentAuditLog>(e =>
        {
            e.ToTable("PaymentAuditLogs");
            
            e.HasOne(e => e.PaymentTransaction)
            .WithMany(e => e.PaymentAuditLogs)
            .WithForeignKey(e => e.PaymentTransactionId)
            .OnDelete(DeleteBehaviour.Restrict);
        });
    }
    public override Task<int> SaveChangesAsync()
    {
        base.SaveChangesAsync();
    }
}