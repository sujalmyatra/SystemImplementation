using System.Collections.Specialized;

public abstract class BaseEntity
{
    public Guid Id {get; protected set;} = Guid.NewGuid();
}

public enum TransactionType
{
    Deposit = 1,
    Withdraw,
    InterestCredit
}

public class Transaction : BaseEntity
{
    public Guid AccountId {get; private set;}
    public Account Account {get; private set;} = null!;

    public TransactionType TransactionType {get; private set;}
    public decimal Amount {get; private set;}
    public decimal BalanceBefore {get; private set;}
    public decimal BalanceAfter {get; private set;}
    public DateTime TransactionDate{get; private set;}

    private Transaction()
    {}

    public Transaction(Guid acId, TransactionType type, decimal amt, decimal bb, decimal ba, DateTime tDate)
    {
        AccountId = acId;
        TransactionType = type;
        Amount = amt;
        BalanceBefore = bb;
        BalanceAfter = ba;
        TransactionDate = tDate;
    }
}
public abstract class Account : BaseEntity
{
    public string AccountNumber {get; private set;} = null!;
    public decimal Balance {get; private set;} 
    public decimal AnnualInterestRate {get; private set;}

    public bool IsActive {get;private set;} = true;

    public DateTime? LastInterestAddedOn{get; private set;}

    public ICollection<Transaction> Transactions {get; private set;}
    = new List<Transaction>();

    protected Account(string accNumber, decimal annualInterestRate)
    {
        AccountNumber = accNumber;
        AnnualInterestRate = annualInterestRate;
    }
    public decimal AddInterest(DateTime addOn)
    {
        CheckAccountIsActive();

        if(LastInterestAddedOn.HasValue && 
        LastInterestAddedOn.Value.Month == addOn.Month && 
        LastInterestAddedOn.Value.Year == addOn.Year)
        {
            throw new InterestAlreadyAddedException($"Interrest Already Added On {LastInterestAddedOn.Value.Date}");
        }

        var calculatedInterest = CalculateMonthlyInterest();

        var balanceBefore = Balance;

        Balance += calculatedInterest;  

        LastInterestAddedOn = addOn.Date;

        if(calculatedInterest > 0)
            AddTransaction(TransactionType.InterestCredit, calculatedInterest, balanceBefore, Balance);

        return calculatedInterest;

    }

    private decimal CalculateMonthlyInterest()
    {
        var yearlyInterest = Balance * AnnualInterestRate / 100;
        return yearlyInterest / 12m;

    }

    public void Withdraw(decimal amt)
    {
        CheckAccountIsActive();
        EnsureAmountValidity(amt);

        if(amt > Balance)
            throw new InSufficientBalanceException(Balance, amt);   

        decimal balanceBefore = Balance;

        Balance -= amt;

        AddTransaction(TransactionType.Withdraw, amt, balanceBefore, Balance);
    }

    public void Deposit(decimal amt)
    {
        CheckAccountIsActive();
        EnsureAmountValidity(amt);

        decimal balanceBefore = Balance;

        Balance += amt;

        AddTransaction( TransactionType.Deposit, amt, balanceBefore, 
        Balance );
    }

    private void AddTransaction(TransactionType transactionType, decimal amt, decimal bb, decimal ba, DateTime? transactionDate = null)
    {
        var transaction = new Transaction(Id, transactionType, amt, bb, ba, transactionDate ?? DateTime.Now);

        Transactions.Add(transaction);
    }
    private void EnsureAmountValidity(decimal amt)
    {
        if(amt<=0)
            throw new InvalidAmountException("this is invalid amount");
    }
    public void CheckAccountIsActive()
    {
        if(!IsActive)
            throw new InActiveAccountException("this Account is Inactive");
    }
}

public class SavingsAccount : Account
{
    private const decimal SavingsInterestRate = 4m;

    private SavingsAccount(){}

    public SavingsAccount(string accNumber) : base(accNumber, SavingsInterestRate)
    {}
}

public class CurrentAccount : Account
{
    private const decimal CurrentInterestRate = 2m;

    private CurrentAccount(){}

    public CurrentAccount(string accNumber) : base(accNumber, CurrentInterestRate)
    {}
}
public class SuspeciousActivity
{
    public Guid AccountId{get; set;}
    public DateTime TransactionDate{get;set;}
    public int TransactionCount{get;set;}
}
//Infrastructure
public class AppDbContext : Dbcontext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<Account> Accounts => DbSet<Account>();
    public DbSet<Transaction> Transactions => DbSet<Transaction>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Account>(e =>
        {
           e.HasKey(x => x.Id);
        });

        builder.Entity<Transaction>(e =>
        {
           e.HasKey(x => x.Id);

           e.HasOne(x => x.Account)
           .WithMany(x => x.Transactions)
           .HasForeignKey(x => x.AccountId)
           .OnDelete(DeleteBehaviour.Restrict);
 
        });
    }

    protected override Task<int> SaveChangesAsync()
    {
        base.SaveChangesAsync();
    }

}

//Repository
public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid accountId);
    Task<List<Account>> GetAllActiveAccountsAsync();
    Task AddAsync(Account acc);
}
public class AccountRepository(AppDbContext context) : IAccountRepository
{
    public async Task<Account?> GetByIdAsync(Guid accountId)
    {
        return await context.Accounts.FindAsync(accountId);
    }
    public Task<List<Account>> GetAllActiveAccountsAsync()
    {
        return context.Accounts.Where(x => x.IsActive).ToListAsync();
    }
    public async Task AddAsync(Account acc)
    {
        await context.Accounts.AddAsync(acc);
    }
}

public interface ITransactionRepository
{
    Task<List<Transaction>> GetPagedTransactionAsync(Guid accId, int pageNumber, int pagesize);
    Task<List<Transaction>> GetMiniStatement(Guid accId);
    Task<List<SuspeciousActivity>> GetSuspeciousActivitiesAsync(DateTime date); 

}
public class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public Task<List<Transaction>> GetPagedTransactionAsync(Guid accId, int pageNumber, int pagesize)
    {
        return context.Transactions
        .Where(x => x.AccountId == accId)
        .OrderByDescending(x => x.TransactionDate)
        .Skip((pageNumber - 1) * pagesize)
        .Take(pagesize)
        .ToListAsync();
    }
    public Task<List<Transaction>> GetMiniStatement(Guid accId)
    {
        return context.Transactions
        .Where(x => x.AccountId == accId)
        .OrderByDescending(x => x.TransactionDate)
        .Take(5)
        .ToListAsync();
    }
    public Task<List<SuspeciousActivity>> GetSuspeciousActivitiesAsync(DateTime from, DateTime to)
    {
        DateTime start = from.Date;
        DateTime endExec = to.AddDays(1);

        return context.Transactions
        .Where(x => x.TransactionDate >= start && x.TransactionDate < endExec)
        .GroupBy(x => new{x.AccountId, x.TransactionDate.Date})
        .Where(g => g.Count() > 5)
        .Select(g => new SuspeciousActivity
        {
            AccountId = g.Key.AccountId,
            TransactionDate = g.Key.Date,
            TransactionCount = x.Count()
        })
        .ToListAsync();

    }

}