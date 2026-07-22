

using System.Collections.Frozen;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

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
    protected readonly DbSet<T> _set = context.Set<T>();

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
    IAssetRepository Assets {get;} 
    IGenericRepository<AssetAssignment> AssetAssignments {get;} 
    IGenericRepository<AssignmentHistory> AssignmentHistorys {get;} 
    IGenericRepository<MaintenanceRecord> MaintenanceRecords {get;} 

    Task<int> SaveChangesAsync();
}
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public IGenericRepository<Employee> Employees {get;} = new GenericRepository<Employee>(context);
    public IAssetRepository Assets {get;}  = new AssetRepository(context);
    public IGenericRepository<AssetAssignment> AssetAssignments {get;}  = new GenericRepository<AssetAssignment>(context);
    public IGenericRepository<AssignmentHistory> AssignmentHistorys {get;}  = new GenericRepository<AssignmentHistory>(context);
    public IGenericRepository<MaintenanceRecord> MaintenanceRecords {get;}  = new GenericRepository<MaintenanceRecord>(context);

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}
public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<Employee?> GetWithActiveAssignmentsAsync(Guid empId);
    Task<Employee?> GetWithActiveAssignmentsAndAssetsAsync(Guid empId);
}
public class  EmployeeRepository(AppDbContext context) : GenericRepository<Employee>(context),IEmployeeRepository 
{
    public async Task<Employee?> GetWithActiveAssignmentsAsync(Guid empId)
    {
        return await context.Employees.Include(e => e.AssetAssignments.Where(x => x.IsActive))
            .SingleOrDefaultAsync(x => x.Id == empId);
    }
    public async Task<Employee?> GetWithActiveAssignmentsAndAssetsAsync(Guid empId)
    {
        return await context.Employees.Include(e => e.AssetAssignments.Where(x => x.IsActive)).ThenInclude(x => x.Asset)
            .SingleOrDefaultAsync(x => x.Id == empId);
    }
}
public interface IAssetRepository : IGenericRepository<Asset>
{
    Task<Asset?> GetAvailableAssetAsync(AssetType type);
    Task<Asset?> GetWithActiveAssignmentsAsync(Guid assetId);
    Task<Asset?> GetWithActiveAssignmentsandEmployeesAsync(Guid assetId);
}
public class AssetRepository(AppDbContext context) : GenericRepository<Asset>(context),IAssetRepository
{
    public async Task<Asset?> GetAvailableAssetAsync(AssetType type)
    {
        return await _set.FirstOrDefaultAsync(x => x.AssetType == type && x.Status == AssetStatus.Available);
    }
    public async Task<Asset?> GetWithActiveAssignmentsAsync(Guid assetId)
    {
        return await _set.Include(x => x.AssetAssignments.Where(a => a.IsActive))
        .SingleOrDefaultAsync(x => x.Id == assetId);
    }
    public async Task<Asset?> GetWithActiveAssignmentsandEmployeesAsync(Guid assetId)
    {
        return await _set.Include(x => x.AssetAssignments.Where(a => a.IsActive))
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

public interface IAssetService
{
    Task<AssetAssignmentResponseDto> RequestAssetAsync(Guid employeeId, AssetType assetType);
}
public record AssetAssignmentResponseDto(Guid EmployeeId, Guid? AssetId,DateTime? AssignedTime, string Message);
public class AssetService(IUnitOfWork uow) : IAssetService
{
    public async Task<AssetAssignmentResponseDto> RequestAssetAsync(Guid employeeId, AssetType assetType)
    {
        var member = await uow.Employees.GetByIdAsync(employeeId);

        if(member is null)
            throw new KeyNotFoundException($"Employee with {employeeId} not Found");

        var asset = await uow.Assets.GetAvailableAssetAsync(type);

        if(asset is null)
             return new AssetAssignmentResponseDto(employeeId, null, null, $"Requested Asset of {type} is not available RightNow");

        DateTime AssignmentTime = DateTime.UtcNow;

        var assetAssignment = new AssetAssignment{EmployeeId = employeeId, AssetId = asset.Id, AssignedAt = AssignmentTime};

        asset.Status = AssetStatus.Assigned;

        await uow.AssetAssignments.AddAsync(assetAssignment);

        await uow.SaveChangesAsync();

        return new AssetAssignmentResponseDto(employeeId, asset.Id, AssignmentTime, "Asset Assignment Successfull" );
    }
}

public interface IEmployeeService
{
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto request);
    Task<EmployeeDto?> GetEmployeeByIdAsync(Guid employeeId);
    Task<List<EmployeeDto>> GetAllEmployeesAsync();
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto request);
    Task UpdateAsync(UpdateEmployeeDto request);
    Task DeleteAsync(Guid id);
}
public class  EmployeeService(IUnitOfWork uow, ILogger<EmployeeService> logger) : IEmployeeService
{
    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto request)
    {
        var emp = new Employee{Name = request.Name, Department = request.Department};

        await uow.Employees.AddAsync(emp);

        await uow.SaveChangesAsync();

        logger.LogInformation($"Employee Created {emp.Name}, Department : {emp.Department}");

        return new EmployeeDto(emp.Id, emp.Name, emp.Department);
    }
    public async Task<EmployeeDto?> GetEmployeeByIdAsync(Guid employeeId)
    {
        var emp = await uow.Employees.GetByIdAsync(employeeId);

        if(emp is null)
        {
            logger.LogError($"Employee with ID : {employeeId} Not Found");
            return null;
        }

        return new EmployeeDto(emp.Id,emp.Name,emp.Department);
    }
    public async Task<List<EmployeeDto>> GetAllEmployeesAsync()
    {
        var employees = await uow.Employees.GetAllAsync();

        return new employees.Select(x => new EmployeeDto(x.Id, x.Name, x.Department)).ToList();
    }
    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto request);
    public async Task UpdateAsync(Guid id, UpdateEmployeeDto request)
    {
        var emp = await uow.Employees.GetByIdAsync(id);

        if(emp is null)
        {
            logger.LogError($"Employee with ID : {employeeId} Not Found");
            throw new KeyNotFoundException($"Employee with {id} not found");
        }
        
        emp.Name = request.Name;
        emp.Department = request.Department;

        uow.Employees.Update(emp);

        await uow.SaveChangesAsync();

        logger.LogInformation($"Employee with ID : {employeeId} Updated");

    }
    public async Task DeleteAsync(Guid id)
    {
         var emp = await uow.Employees.GetByIdAsync(id);

        if(emp is null)
        {
            logger.LogError($"Employee with ID : {employeeId} Not Found");
            throw new KeyNotFoundException($"Employee with {id} not found");
        }

        uow.Employees.Delete(emp);

        await uow.SaveChangesAsync();

        logger.LogInformation($"Employee with ID : {employeeId} Deleted");
    }
}

public interface IAssetService
{
    Task<AssetReturnResponseDto> ReturnAssetAsync(Guid assignmentId, AssetCondition condition);
}

public class AssetService(IUnitOfWork uow): IAssetService
{
    public async Task<AssetReturnResponseDto> ReturnAssetAsync(Guid assignmentId, AssetCondition condition)
    {
        var assignment = await uow.AssetAssignments.Where(x => x.Id == assignmentId)
                    .Include(x => x.Asset)
                    .SingleOrDefaultAsync();

        if(assignment is null)
            throw new NotFoundException($"ssetAssignment with {assignmentId} not found");
        
        assignment.IsActive = false;

        assignment.Asset.Condition = condition;

        assignment.Asset.Status = condition switch
        {
           AssetCondition.Good => AssetStatus.Available,
           AssetCondition.NeedsMaintenance => AssetStatus.UnderMaintenance,
           AssetCondition.Damaged => AssetStatus.Damaged,
           _ => throw new InvalidArgumentException($"{condition} does not exist in system")

        } ;

        var ah = new AssignmentHistory
        {
            AssignedAt = assignment.AssignedAt,
            EmployeeId = assignment.EmployeeId,
            ReturnCondition = condition,
            AssetId = assignment.AssetId,
            ReturnedAt = DateTime.UtcNow
        };

        await uow.AssignmentHistorys.AddAsync(ah);

        await uow.SaveChangesAsync();
        return new AssetReturnResponseDto(ah.EmployeeId, ah.AssetId, ah.ReturnedAt);
    }
}
public record AssetReturnResponseDto(Guid EmployeeId, Guid AssetId, DateTime ReturnedAt);
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
public interface IAssetReportRepository
{
    Task<List<EmployeeAssetReportDto>> GetEmployeeWiseAssetReportAsync();
    Task GetAssetStatusReportAsync();
}
public record EmployeeAssetReportDto(Guid EmpId, string Name, List<string> Assets);

public class AssetReportRepository(AppDbContext context) : IAssetReportRepository
{
    public async Task<List<EmployeeAssetReportDto>> GetEmployeeWiseAssetReport()
    {
        //1. Employee-wise Asset Report

        var query = from e in context.Employees
            join a in context.AssetAssignments.Where(assignment => assignment.IsActive)
            on e.Id equals a.EmployeeId
            into employeeAssignments

            select new EmployeeAssetReportDto(
                e.Id,
                e.Name,
                employeeAssignments.Select(x => x.Asset.AssetCode).ToList()
            );

        var rows = context.Employees.LeftJoin(
            context.AssetAssignments.Where(x => x.IsActive),
            e => e.Id, a => a.EmployeeId, 
            (e, a) => new 
            (e.Id, e.Name, AssetCode = a == null ? null : a.Asset.AssetCode))
         .AsNoTracking()
         .ToListAsync();

         rows.GroupBy(row => new {row.Id, row.Name})
         .Select(e => new EmployeeAssetReportDto(e.Key.Id, e.Key.Name, e.Select(x => x.AssetCode).ToList()));

        //Projection
        return await context.Employees
        .Select(x => new EmployeeAssetReportDto(
            x.Id, x.Name,
            x.AssetAssignments
            .Where(x => x.IsActive)
            .Select(x => x.Asset.AssetCode).ToList()))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<AssetStatusReport>> GetAssetStatusReportAsync()
    {
        var statusCounts = await context.Assets
        .AsNoTracking()
        .GroupBy(asset => asset.Status)
        .ToDictionaryAsync(
            group => group.Key,
            group => group.Count()
        );

        return Enum.GetValues<AssetStatus>()
            .Select(st => new 
            AssetStatusReport(
                st, 
                statusCounts.GetValueOrDefault(st)))
            .ToList();
    }

    public async Task<AssetHistoryDto?> GetAssetHistoryReportByIdAsync(Guid assetId)
    {
        var asset =  await context.Assets
        .Where(asset => asset.Id == assetId)
        .Select(asset => new (
            Type = asset.AssetType,
            asset.AssetCode,
            Histories = asset.AssignmentHistories.Select(x => EmployeeName = x.Employee.Name, x.AssignedAt, x.ReturnedAt).ToList()
        )).SingleOrDefaultAsync();

        if(asset is null)
            return null;
        
        var history = asset.Histories.Select(history => GetConstructedStatement(history.EmployeeName, history.AssignedAt, history.ReturnedAt )).ToList();

        return new AssetHistoryDto(asset.Type, asset.AssetCode, history);
    }
    public record AssetHistoryDto(AssetType Type, string AssetCode, List<string> AssignmentHistories);
    private string GetConstructedStatement(string empName, DateTime from, DateTime to)
    {
        return $"Assigned to Employee :: {empName} from {from.Date} to {to.Date}";
    }
}

public record AssetStatusReport(AssetStatus Status, int AssetCount);

//1
