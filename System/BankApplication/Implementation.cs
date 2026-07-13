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

    private void AddTransaction(TransactionType transactionType, 
    decimal amt, decimal bb, decimal ba, DateTime? transactionDate = null)
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