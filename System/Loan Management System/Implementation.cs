using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

public enum LoanStatus
{
    Pending,
    Approved,
    Active,
    Closed,
    Rejected,
    Defaulted
}
public enum EMIStatus
{
    Pending,
    PartiallyPaid,
    Paid,
    OverDue
}
public abstract class BaseEntity
{
    public Guid Id {get; protected set;} = Guid.NewGuid();
    public DateTime CreatedAt{get; set;}
    public DateTime? UpdatedAt{get; set;}
    public DateTime? DeletedAt{get; set;}


    public bool IsDeleted{get; set;} = false;

    public void SetCreatedAt(DateTime createdAt)
    {
        CreatedAt = createdAt;
    }
    public void SetUpdatedAt(DateTime updatedAt)
    {
        UpdatedAt = updatedAt;
    }
    public void MarkAsDeleted()
    {
        IsDeleted = true;
    }
}
public class Customer : BaseEntity
{
    public string Name{get; set;}
    public string PhoneNumber{get; set;}


    public ICollection<Loan> Loans {get; set;} = new List<Loan>();
}   
public class Loan : BaseEntity
{
    public LoanStatus Status{get; set;}

    public decimal Amount{get; set;}
    public decimal InterestRate{get; set;}
    public int DurationMonths{get; set;}


    public DateTime AppliedAt{get; set;} = DateTime.UtcNow;
    public DateTime? ApprovedAt{get; set;}
    public DateTime? DisbursedAt{get; set;}
    public DateTime? ClosedAt{get; set;}


    public Guid CustomerId{get; set;}
    public Customer Customer{get; set;}


    public ICollection<EMI> EMIs {get; set;} = new List<EMI>();
    
}
public class EMI : BaseEntity
{
    public int InstallMentNumber{get; set;}
    public EMIStatus Status{get; set;} = EMIStatus.Pending;
   
    public decimal TotalAmount{get; set;}
    public decimal PaidAmount{get; set;}
    public decimal PendingAmount{get; set;} => TotalAmount - PaidAmount;



    public DateTime DueDate{get; set;}
    public DateTime? FullyPaidAt{get; set;}

    
    public Guid LoanId{get; set;}
    public Loan Loan{get; set;}

    public ICollection<Repayment> Repayments {get; set;} = new List<Repayment>();
    public ICollection<OverdueRecord> OverdueRecords{get;set;} = new 
    List<OverdueRecord>();


}
public class Repayment  : BaseEntity
{
    public decimal Amount{get; set;}
    public DateTime RepaymentPaidAt{get; set;} = DateTime.UtcNow;

    public Guid EMIId{get; set;}
    public EMI EMI{get; set;}

}
public class OverdueRecord  : BaseEntity
{
    public DateTime MarkedOverDueAt{get; set;}
    public decimal PendingAmountAtOverDue{get; set;}

    public bool IsResolved{get; set;} = false;
    public DateTime? ResolvedAt{get; set;}


    public Guid EMIId{get; set;}
    public EMI EMI{get; set;} = null!;
   

}

//Infra
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<EMI> EMIs => Set<EMI>();
    public DbSet<Repayment> Repayments => Set<Repayment>();
    public DbSet<OverDueRecord> OverDueRecords => Set<OverDueRecord>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureLoan(builder);
        
        ConfigureEMI(builder);
    }

    private static void ConfigureLoan(ModelBuilder builder)
    {
        builder.Entity<Loan>(e => new
        {
            e.HasKey(x => x.Id);

            e.HasQueryFilter(x => !x.IsDeleted);

            e.HasOne(e => e.Customer)
            .WithMany(c => c.Loans)
            .HasForeignKey(l => l.CustomerId)
            .OnDelete(DeleteBahviour.Restrict);

             e.HasMany(e => e.EMIs)
            .WithOne(c => c.Loan)
            .HasForeignKey<EMI>(e => e.LoanId)
            .OnDelete(DeleteBahviour.Restrict);

            e.HasMany(e => e.OverDueRecords)
            .WithOne(c => c.Loan)
            .HasForeignKey<OverDueRecord>(e => e.LoanId)
            .OnDelete(DeleteBahviour.Restrict);
        });

    }
    private static void ConfigureEMI(ModelBuilder builder)
    {
        builder.Entity<EMI>(e => new
        {
            e.HasKey(x => x.Id);
            
            e.Property(x => x.Id).ValueGeneratedOnAdd();

            e.HasQueryFilter(x => !x.IsDeleted);

            e.HasMany(e => e.Repayments)
            .WithOne(c => c.EMI)
            .HasForeignKey<Repayment>(e => e.EMIId)
            .OnDelete(DeleteBahviour.Restrict);
        });
    }
    
    public override async Task<int> SaveChangesAsync()
    {

        ApplySoftDelete();
        await base.SaveChangesAsync();

    }
    private void ApplySoftDelete()
    {
        var deletedEntries = ChangeTracker.Entries<BaseEntity>()
        .Where(entry => entry.State == EntityState.Deleted);

        foreach(var entry in deletedEntries)
        {
            entry.State = EntityState.Modified;
            entry.MarkAsDeleted();
            entry.Propery(x => x.CreatedAt).IsModified = false;
        }
    }
    private void ApplyAuditFields()
    {
        var curretnTime = DateTime.UtcNow;

        var addedEntries = ChangeTracker.Entires<BaseEntity>()
            .Where(entry => entry.State = EntityState.Added);
        foreach(var entry in addedEntries)
        {
            entry.Entity.SetCreatedAt(curretnTime);
        }

        var modifiedEntries = ChangeTracker.Entires<BaseEntity>()
            .Where(entry => entry.State == EntityState.Modified);
        foreach(var entry in modifiedEntries)
        {
            entry.Entity.SetUpdatedAt(curretnTime);
            entry.Property(x => x.CreatedAt).IsModified = false;
        }
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
    IGenericRepository<Loan> Loans{get;}
    IGenericRepository<Repayment> Repayments{get;}
    IGenericRepository<EMI> EMIs{get;}
    IGenericRepository<OverDueRecord> OverDueRecords{get;}

    Task<int> SaveChangesAsync();
}
public class GenericRepository<T>(AppDbContext context) : IGenericRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _set = context.Set<T>();
    

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _set.FindAsync(id);
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
        _set.Remove(entity);
    }
}

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public IGenericRepository<Customer> Customers{get;}
    = new GenericRepository<Customer>(context);
    public IGenericRepository<Loan> Loans{get;}
    = new GenericRepository<Loan>(context);
    public IGenericRepository<Repayment> Repayments{get;}
    = new GenericRepository<Repayment>(context);
    public IGenericRepository<EMI> EMIs{get;}
    = new GenericRepository<EMI>(context);
    public IGenericRepository<OverDueRecord> OverDueRecords{get;}
    = new GenericRepository<OverDueRecord>(context);

    public async Task<int> SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}

public interface ILoanReportRepo
{
    Task<List<CustomerLoanCountDto>> GetCustomerWiseLoanAsync();
    
    Task<EmiCollectionDto> GetEMICollectionByMonthAsync(Guid loanId, DateTime month);

}
public record CustomerLoanCountDto(string CustomerName, int LoanCount);
public class LoanReportRepo(AppDbContext context) : ILoanReportRepo
{
    public async Task<List<CustomerLoanCountDto>> GetCustomerWiseLoanAsync()
    {
        return await context.Customers
        .Select(
            Customer => new CustomerLoanCountDto
            (
                CustomerName = Customer.Name,
                LoanCount = Customer.Loans.Count(loan => loan.Status == LoanStatus.Active)
            )
        )
        .AsNoTracking()
        .ToListAsync();
    }
    public async Task<EmiCollectionDto> GetEMICollectionByMonthAsync(DateTime date)
    {
        return await context.EMIs
        .Where(x => x.PaidAt.Month = date.Month)
        // .GroupBy(x => x.PaidAt.Month)
        .Select(x => EmiCollectionDto
        (
            Month = date.month,
            EmiSum = x.Sum(emi => emi.PaidAmount)
        ))
        .SingleOrDefaultAsync();
    }
}
public record EmiCollectionDto(string Month, int TotalRevenue);



//Guid -> https://chatgpt.com/share/6a5dac09-a700-83ee-9396-b1b6df3feff0
//builder.HasKey(x => x.Id)
//builder.Property(x => x.Id).ValueGeneratedOnAdd();
// builder.HasKey(x => x.Id);

// builder.Property(x => x.Id)
//     .ValueGeneratedOnAdd();

//One to One RelationShip 
// https://chatgpt.com/share/6a5dac17-e37c-83e8-94c2-b04156eea5de