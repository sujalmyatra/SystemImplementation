using System.Net;

public enum AssetStatus
{
    
    Available,
    Assigned,
    UnderMaintenance,
    Damaged,
    Retired
}
public enum AssetType
{
    
    Laptop,
    Mobile,
    KeyBoard,
    Mouse
}
public enum AssetCondition
{ 
    Good,
    NeedsMaintenance,
    Damaged
}
public abstract class BaseEntity
{
    public Guid Id {get; protected set;} =  Guid.NewGuid();
     public bool IsDeleted{get; set;} = false;
    public void MarkAsDeleted()
    {
        IsDeleted = true;
    }
}

public class Employee : BaseEntity
{
    public string Name{get; set;}
    public string Department{get; set;}

    public ICollection<AssetAssignment> AssetAssignments {get; private set;} = new List<AssetAssignment>();
    public ICollection<AssignmentHistory> AssignmentHistories {get; private set;} = new List<AssignmentHistory>();
}
public class Asset : BaseEntity
{
    public string AssetCode {get; set;}
    public AssetType AssetType{get; set;}
    public AssetStatus Status{get; set;} = AssetStatus.Available;
    public AssetCondition Condition{get; set;} = AssetCondition.Good;
    public bool CanBeAssigned()
    {
        return Status == AssetStatus.Available && !IsDeleted;
    }
   

    public ICollection<AssetAssignment> AssetAssignments {get; private set;} = new List<AssetAssignment>();
    public ICollection<AssignmentHistory> AssignmentHistories {get; private set;} = new List<AssignmentHistory>();
    public ICollection<MaintenanceRecord> MaintenanceRecords {get; private set;} = new List<MaintenanceRecord>();

}
public class AssetAssignment : BaseEntity
{
    public Guid EmployeeId{get; set;}
    public Employee Employee{get; set;} = null!;

    public Guid AssetId{get; set;}
    public Asset Asset{get; set;} = null!;

    public DateTime AssignedAt{get; set;}
    public bool IsActive{get; set;} = true;
    public void Deactivate()
    {
        IsActive = false;
    }

}
public class AssignmentHistory : BaseEntity
{
    public Guid EmployeeId{get; set;}
    public Employee Employee{get; set;}

    public Guid AssetId{get; set;}
    public Asset Asset{get; set;}

    public DateTime AssignedAt{get; set;}
    public DateTime ReturnedAt{get; set;}

    public AssetCondition ReturnCondition{get; set;}

}
public class MaintenanceRecord : BaseEntity
{
     public Guid AssetId{get; set;}
    public Asset Asset{get; set;}

    public string Issue{get; set;}
    public decimal MaintenanceCost{get; set;}
    public DateTime MaintenanceDate{get; set;}
    public DateTime? CompletedAt{get; set;}

}

//Infrastrucuture
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Asset> Assets => Set<Asset>(); 
    public DbSet<AssetAssignment> AssetAssignments => Set<AssetAssignment>(); 
    public DbSet<AssignmentHistory> AssignmentHistorys => Set<AssignmentHistory>(); 
    public DbSet<MaintenanceRecord> MaintenanceRecords => Set<MaintenanceRecord>(); 


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

}

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasKey(x => x.Id);

        builder.Property(e => e.IsDeleted).HasDefaultValue(false);
    }
}

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets");
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.AssetCode);
        builder.Property(e => e.IsDeleted).HasDefaultValue(false);
    }
}

public class AssetAssignmentConfiguration : IEntityTypeConfiguration<AssetAssignment>
{
    public void Configure(EntityTypeBuilder<AssetAssignment> builder)
    {
        builder.ToTable("Assets");
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasKey(x => x.Id);
        builder.Property(e => e.IsDeleted).HasDefaultValue(false);

        builder.HasOne(x => x.Employee).WithMany(x => x.AssetAssignments).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehaviour.Restrict);
         builder.HasOne(x => x.Asset).WithMany(x => x.AssetAssignments).HasForeignKey(x => x.AssetId).OnDelete(DeleteBehaviour.Restrict);
    }
}

public class AssignmentHistoryConfiguration : IEntityConfiguration<AssignmentHistory>
{
    public void Configure(EntityTypeBuilder<AssignmentHistory> builder)
    {
        builder.ToTable("AssignmentHistories");
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasKey(x => x.Id);
        builder.Property(e => e.IsDeleted).HasDefaultValue(false);

       builder.HasOne(x => x.Employee).WithMany(x => x.AssignmentHistories).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehaviour.Restrict);
         builder.HasOne(x => x.Asset).WithMany(x => x.AssignmentHistories).HasForeignKey(x => x.AssetId).OnDelete(DeleteBehaviour.Restrict);
    }
}

public class MaintenanceRecordConfiguration : IEntityConfiguration<MaintenanceRecord>
{
    public void Configure(EntityTypeBuilder<MaintenanceRecord> builder)
    {

        builder.ToTable("MaintenanceRecords");
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasKey(x => x.Id);
        builder.Property(e => e.IsDeleted).HasDefaultValue(false);
        builder.HasOne(x => x.Asset).WithMany(x => x.MaintenanceRecords).HasForeignKey(x => x.AssetId).OnDelete(DeleteBehaviour.Restrict);
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
public class GenericRepository<T>(AppDbContext context) : IGenericRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _set = context.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _set.FindAsync(id);
    }
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _set.AsNoTracking().ToListAsync()(id); 
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

public interface IUnitOfWork
{
    IGenericRepository<Employee> Employees {get;}
    IGenericRepository<Asset> Assets {get;} 
    IGenericRepository<AssetAssignment> AssetAssignments {get;} 
    IGenericRepository<AssignmentHistory> AssignmentHistorys {get;} 
    IGenericRepository<MaintenanceRecord> MaintenanceRecords {get;} 

    Task<int> SaveChangesAsync();
}
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public IGenericRepository<Employee> Employees {get;} = new GenericRepository<Employee>(context);
    public IGenericRepository<Asset> Assets {get;}  = new GenericRepository<Employee>(Asset);
    public IGenericRepository<AssetAssignment> AssetAssignments {get;}  = new GenericRepository<AssetAssignment>(context);
    public IGenericRepository<AssignmentHistory> AssignmentHistorys {get;}  = new GenericRepository<AssignmentHistory>(context);
    public IGenericRepository<MaintenanceRecord> MaintenanceRecords {get;}  = new GenericRepository<MaintenanceRecord>(context);

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}

public interface IAssetAssignmentRepository : IGenericRepository<AssetAssignment>
{
    
}
public class AssetAssignmentRepository(AppDbContext context) : GenericRepository<AssetAssignment>(context), IAssetAssignmentRepository
{
    
}
public interface IAssignmentHistoryRepository : IGenericRepository<AssignmentHistory>
{
    
}

public class AssignmentHistoryRepository(AppDbContext context) : GenericRepository<AssignmentHistory>(context), IAssignmentHistoryRepository
{
    
}


public interface IMaintenanceRecordRepository : IGenericRepository<MaintenanceRecord>
{
    
}

public class MaintenanceRecordRepository(AppDbContext context) : GenericRepository<MaintenanceRecord>(context), IMaintenanceRecordRepository
{
    
}

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            _logger.LogInformation($"{context.Request.Method},{context.Request.Path} ");
            await _next(context);
            _logger.LogInformation($"Request Completed{context.Request.Method}, {context.Request.Path}, {context.Request.StatusCode}");

        }
        catch(Exception ex)
        {
            _logger.LogError(ex.Message );

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var res = new {StatusCode = 500, Message = "Error occured"};

            await context.Response.WriteAsJsonAsync(res); 
        }
    }
    //app.UseMiddleware<GlobalExceptionMiddleware>();
}