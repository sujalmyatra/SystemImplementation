using System.Data.Common;
using System.Dynamic;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;

https://chatgpt.com/share/6a4e4e67-a838-83e8-b8da-04c380411401

public abstract class BaseEntity
{
	public Guid Id {get; protected set;} = Guid.NewGuid();
}

public interface ISoftDeletable
{
	bool IsDeleted {get; set;}
}

public class Product : BaseEntity
{
	public string Name {get;set;}
	public int UnitPrice {get;set;}
	
	public ICollection<Stock> Stocks {get; set;} = new List<Stock>();
	public ICollection<StockMovement> StockMovements {get; set;} = new List<StockMovement>();
	
}

public class Stock : BaseEntity
{
	public Guid ProductId {get;set;}
	public Product Product {get;set;} = null!;
	
	public Guid WarehouseId {get;set;}
	public Warehouse Warehouse {get;set;} = null!;

	public int Quantity {get;set;}
	
	public StockStatus Status 
	{
		get 
		{
			if(Quantity == 0)
				return StockStatus.OutOfStock;
	
			if(Quantity < 20)
				return StockStatus.LowStock;
			
			return StockStatus.Available;
		}
	}
	
	public void IncreaseQuantity(int qty)
	{
		if(qty <= 0)
			throw new ArgumentException("Quantity must be greater than zero");
		Quantity += qty;
	}
	public void DecreaseQuantity(int qty)
	{
		if (qty <= 0)
            		throw new ArgumentException("Quantity must be greater than zero");

		if(Quantity < qty)
		{
			throw new NotEnoughStockException("Not enough stock available");
		}
		Quantity -= qty;
	}
	
}

public class Warehouse : BaseEntity
{
	public string Name {get;set;}
	public string Address {get;set;}
	public ICollection<Stock> Stocks {get; set;}= new List<Stock>();
	public ICollection<StockMovement> IncomingMovements {get;set;} = new List<StockMovement>();
	public ICollection<StockMovement> OutgoingMovements {get; set;} = new List<StockMovement>();
}

public class Supplier : BaseEntity
{
	public string Name {get;set;}
	public string PhoneNumber {get;set;}
	
	public ICollection<StockMovement> StockMovements {get; set;}= new List<StockMovement>();
}

public class StockMovement : BaseEntity, ISoftDeletable
{
	public MovementType Type {get; set;}
	public DateTime MovementDate {get; set;} = DateTime.Now;

	public Guid ProductId {get; set;}
	public Product Product {get; set;} = null!;

	public Guid? SupplierId {get; set;}
	public Supplier? Supplier {get; set;} = null!;

	public Guid? FromWarehouseId {get;set;}
	public Warehouse? FromWarehouse {get;set;} = null!;

	public Guid? ToWarehouseId {get;set;}
	public Warehouse? ToWarehouse {get;set;} = null!;
	
	public bool IsDeleted {get; set;}
	public int Quantity {get; set;}
}
public enum StockStatus
{
	Available,
	LowStock,
	OutOfStock
}
public enum MovementType
{
	SupplierToWarehouse,
	WarehouseToWarehouse,
	WarehouseToSupplier
}

//Infrastructure

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
	public DbSet<Product> Products => Set<Product>();
	public DbSet<Supplier> Suppliers => Set<Supplier>();
	public DbSet<Warehouse> Warehouses => Set<Warehouse>();
	public DbSet<Stock> Stocks => Set<Stock>();
	public DbSet<StockMovement> StockMovements => Set<StockMovement>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		ConfigureStock(builder);
		ConfigureStockMovement(builder);
	}
	protected override int SaveChangesAsync()
	{
		base.SaveChangesAsync();
		var entries = ChangeTracker.Entries();

		foreach(var entry in entries)
		{
			if(entry.Entity is ISoftDeletable softDeletable){
				if(entry.State == EntityState.Deleted)
				{
					entry.State = EntityState.Modified;
					softDeletable.IsDeleted = true;
				}
			}
		}
	}
	private static void ConfigureStock(ModelBuilder builder)
	{
		builder.Entity<Stock>(e =>
		{
			e.HasOne(s => s.Product)
			.WithMany(p => p.Stocks)
			.HasForeignKey(s => s.ProductId)
			.OnDelete(DeleteBehaviour.Restrict);

			e.HasOne(s => s.Warehouse)
			.WithMany(w => w.Stocks)
			.HasForeignKey(s => s.WarehouseId)
			.OnDelete(DeleteBehaviour.Restrict);

			e.HasIndex(s => new
			{
				s.ProductId,s.WarehouseId
			}).IsUnique();
		});
	}
	private static void ConfigureStockMovement(ModelBuilder builder)
	{
		builder.Entity<StockMovement>(e => 
		{
			e.HasQueryFilter(x => !x.IsDeleted);
			e.HasOne(m => m.Product)
			.WithMany(p => p.StockMovements)
			.HasForeignKey(m => m.ProductId)
			.OnDelete(DeleteBehaviour.Restrict);

			e.HasOne(m => m.Supplier)
			.WithMany(p => p.StockMovements)
			.HasForeignKey(m => m.SupplierId)
			.OnDelete(DeleteBehaviour.Restrict);

			
			e.HasOne(m => m.FromWarehouse)
			.WithMany(p => p.OutgoingMovements)
			.HasForeignKey(m => m.FromWarehouseId)
			.IsRequired(false)
			.OnDelete(DeleteBehaviour.Restrict);

			e.HasOne(m => m.ToWarehouse)
			.WithMany(p => p.IncomingMovements)
			.HasForeignKey(m => m.ToWarehouseId)
			.IsRequired(false)
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
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}

public interface IStockRepository : IGenericRepository<Stock>
{
	Task<Stock?> GetByProductAndWarehouseAsync(Guid productId,Guid warehouseId);
	Task<List<Stock>> GetLowStockAsync();
	Task<List<Stock>> GetOutOfStockAsync();
}
public interface IStockMovementRepository : IGenericRepository<StockMovement>
{
	Task<List<StockMovement>> GetMovementsByProductAsync(Guid productId);
	Task<List<StockMovement>> GetMovementsBetweenDates(DateTime startDate, DateTime endDate);
}
public interface IUnitOfWork
{
	IGenericRepository<Product> Products{get;}
	IGenericRepository<Supplier> Suppliers{get;}
	IGenericRepository<Warehouse> Warehouses{get;}

	IStockRepository Stocks{get;}
	IStockMovementRepository StockMovements{get;}
	Task<int> SaveChangesAsync();

}
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{

	IGenericRepository<Product> Products{get;} = new GenericRepository<Product>(context);
	IGenericRepository<Supplier> Suppliers{get;} = new GenericRepository<Supplier>(context);
	IGenericRepository<Warehouse> Warehouses{get;}
	 = new GenericRepository<Warehouse>(context);

	IStockRepository Stocks{get;} = new StockRepository(context);
	IStockMovementRepository StockMovements{get;}
	 = new StockMovementRepository(context);
	public async Task<int> SaveChangesAsync()
	{
		return await context.SaveChangesAsync();
	}
}

public class StockMovementRepository(AppDbContext context) : GenericRepository<StockMovement>(context), IStockMovementRepository
{
	private readonly AppDbContext _context = context;

	public async Task<List<StockMovement>> GetMovementsByProductAsync(Guid productId)
	{
		return await _context.StockMovements
		.Include(x => x.Product)
		.Include(x => x.Supplier)
		.Include(x => x.FromWarehouse)
		.Include(x => x.ToWarehouse)
		.Where(x => x.ProductId == productId)
		.ToListAsync();
	}
	public async Task<List<StockMovement>> GetMovementsBetweenDates(DateTime startDate, DateTime endDate)
	{
		return await _context.StockMovements
		.Include(x => x.Product)
		.Include(x => x.Supplier)
		.Include(x => x.FromWarehouse)
		.Include(x => x.ToWarehouse)
		.Where(x => x.MovementDate >= startDate && 
		x.MovementDate <= endDate)
		.ToListAsync();

	}
}

public class StockRepository(AppDbContext context) : GenericRepository<Stock>(context), IStockRepository
{
	private readonly AppDbContext _context = context;

	public async Task<Stock?> GetByProductAndWarehouseAsync(Guid productId,Guid warehouseId)
	{
		return await _context.Stocks
		.FirstOrDefaultAsync(x => x.ProductId == productId && x.WarehouseId == warehouseId);
	}
	public async Task<List<Stock>> GetLowStockAsync()
	{
		return await _context.Include(x=> x.Product).Include(w=>w.Warehouse)
		.Where(x => x.Quantity >0 && x.Quantity < 20)
		.ToListAsync();
	}
	public async Task<List<Stock>> GetOutOfStockAsync()
	{
		return await _context.Include(x => x.Product).Include(x => x.Warehouse)
		.Where(x => x.Quantity == 0)
		.ToListAsync();
	}
}






//Event  Triggers
public class StockLevelChangedEvent
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }

    public int OldQuantity { get; set; }
    public int NewQuantity { get; set; }

    public StockStatus NewStatus { get; set; }
}

public class ApplicationEvent
{
	public event Func<StockLevelChangedEvent, Task>? StockLevelChanged;

	public async Task RaiseStockLevelChangedAsync(StockLevelChangedEvent stockEvent)
	{
		if(StockLevelChanged != null)
		{
			await StockLevelChanged?.Invoke(stockEvent);
		}
	}
}
//Services
public interface IInventoryService
{
    Task ReceiveStockFromSupplierAsync(Guid productId, Guid supplierId, Guid warehouseId, int quantity);

    Task TransferStockAsync(Guid productId, Guid fromWarehouseId, Guid toWarehouseId, int quantity);

    Task ReturnStockToSupplierAsync(Guid productId, Guid warehouseId, Guid supplierId, int quantity);
}

public interface IReportService
{
    Task<List<StockValueReportDto>> GetStockValueReportAsync();

    Task<List<WarehouseStockReportDto>> GetWarehouseWiseStockReportAsync();

    Task<List<FastMovingProductReportDto>> GetFastMovingProductsAsync(DateTime startDate, DateTime endDate);

    Task<List<SlowMovingProductReportDto>> GetSlowMovingProductsAsync(DateTime startDate, DateTime endDate);
}


//REgister in Program.cs

builder.Services.AddSingleton(ApplicationEvent);

var app = builder.Buid();

var applicationEvent = app.GetRequiredServices<ApplicationEvent>();

applicationEvent.StockLevelChanged += 
async stockEvent =>
{
	if(stockEvent.NewQuantity == 0)
	{
		Console.WriteLine($"OUT of Stock Alert :: Product : {stockEvent.ProductId}, Warehouse :  {stockEvent.warehouseId}");
	}
	else if(stockEvent.NewQuantity < 20)
	{
		Console.WriteLine($"LowStock Stock Alert :: Product : {stockEvent.ProductId}, Warehouse :  {stockEvent.warehouseId}");
	}
};

public class Program
{
	public static void Main()
	{
		List<Product> products = new();
		List<Stock> stocks = new();
		List<Warehouse> warehouses = new();
		List<Supplier>  suppliers= new();
		List<StockMovement> stockMovements = new();


		//1
		var r = products.GroupJoin(stocks, p => p.Id, s => s.ProductId,(p, stockgrp) => 
		{
			var TotalQuantity = stockgrp.Sum(x => x.Quantity);

			return new {
			Product = p.Name,
			UnitPRice = p.UnitPrice,
			AvailableQuantitiy = TotalQuantity,
			TotalStockValue = p.UnitPrice * TotalQuantity
			};
		});
		
		//2
		var t  = stocks.Select(x => new
		{
			ProductName = x.Product.Name,
			WarehouseName = x.Warehouse.Name,
			x.Quantity
		});
		//result type 2
		var rr = stocks.GroupBy(x => new {x.ProductId, x.Product.Name})
		.Select( x => new
		{
			Product = x.Key.Name,
			WareHouses = x.GroupBy(w =>new { w.WarehouseId, w.Warehouse.Name})
			.Select(wGrp => new
			{
				WareHouseName = wGrp.Key.Name,
				ProductQuantity = wGrp.Sum(x => x.Quantity)
			}).ToList()
		}).ToList();

		//3
		var today = DateTime.Today;
		var firstDayThisMonth = new DateTime(today.Year, today.Month,1);
		var firstDayPrvsMonth = firstDayThisMonth.AddMonths(-1);

		var ttt = stockMovements.Where(x=> x.MovementDate >= firstDayPrvsMonth && x.MovementDate < firstDayThisMonth)
		.GroupBy(x => new {x.ProductId, x.Product.Name})
		.Select(x => new
		{
			x.Key.ProductId,
			x.Key.Name,
			MovementCount = x.Count(),
			TotalQuantity = x.Sum(q => q.Quantity)
		})
		.Where(x => x.MovementCount > 5 || x.TotalQuantity >= 100);
		
		//4

		var lastmonthstk = stockMovements.Where(x=> x.MovementDate >= firstDayPrvsMonth && x.MovementDate < firstDayThisMonth);

		var yyy = products.GroupJoin(lastmonthstk, p => p.Id, lm => lm.ProductId, (p, movementGrp) => new
		{
			ProductId = p.Id,
			ProductName = p.Name,
			MovementCount = movementGrp.Count(),
			TotalQuantity = movementGrp.Sum(x => x.Quantity)
		} ).
		where(x => x.MovementCount < 5)
		.ToList();

		//5

		var u = stocks
		.Where(x => x.Quantity > 0 && x.Quantity <= 20)
		.Select(x => new
		{
			Product = x.Product.Name,
			Warehouse = x.Warehouse.Name,
			x.Quantity,
			x.Status
		});

		//6
		var dd = stocks.GroupBy(x => new {x.WarehouseId, x.Warehouse.Name})
		.Select(g => new
		{
			Warehouse = g.Key.Name,
			TotalProduct = g.Sum(x => x.Quantity),
			TotalValue = g.Sum(x=> x.Product.UnitPrice * x.Quantity)
		});

		//7
		var suppToWh = lastmonthstk.Where(x => x.Type == MovementType.SupplierToWarehouse);

		var yuy = suppliers.GroupJoin(suppToWh, 
		s => (Guid?)s.Id, lm => lm.SupplierId, 
		(s, lm) => new
		{
			SupplierName = s.Name,
			Products = lm.GroupBy(m => new
			{
				m.ProductId,
				m.Product.Name
			}).Select(x => new
			{
				x.Key.Name,
				SupplyCount = x.Count(),
				TotalQty = x.Sum(x => x.Quantity)
			}).ToList()
		});
		//8
		var startdate = new DateTime(2026, 7, 1);
		var enddate = new DateTime(2026, 7, 5);
		var exclusiveEnd = enddate.AddDays(1);

		var o = stockMovements.Where(x => 
		x.Type == MovementType.WarehouseToWarehouse &&
		x.FromWarehouseId != null &&
		 x.ToWarehouseId != null &&
		 x.FromWarehouse != null &&
		 x.ToWarehouse != null &&
		 x.MovementDate >= startdate && 
		 x.MovementDate < exclusiveEnd
		 );

		var ty = o.GroupBy(x => new 
		{x.ProductId, ProductName = x.Product.Name,
		 x.FromWarehouseId,
		 FromWarehouseName = x.FromWarehouse.Name, 
		 x.ToWarehouseId, 
		 ToWarehouseName = x.ToWarehouse.Name})
		.Select(g =>
			new
			{
				Product = g.Key.ProductName,
				FromWarehouse = g.Key.FromWarehouseName,
				ToWarehouse = g.Key.ToWarehouseName,
				TotalQuantity = g.Sum(x => x.Quantity),
				TotalMovement = g.Count()
			}
		).ToList();

		//9
		var cutoffdays = today.AddDays(-60);

		var last = products.GroupJoin(stocks,
		p => p.Id, s => s.ProductId,
		(p, s) => new
		{
			Product = p,
			CurrentQty = s.Sum(s => s.Quantity)
		}
		)
		.GroupJoin(stockMovements,
		productdata => productdata.Product.Id,
		sm => sm.ProductId,
		(productdata, sm) => new
		{
			PId = productdata.Product.Id,
			PName = productdata.Product.Name,
			productdata.CurrentQty,
			LastMovement = sm.Select(x => (DateTime?)x.MovementDate).Max()
		}
		)
		.Where(x => x.LastMovement == null ||
		 x.LastMovement < cutoffdays)
		.Select(x => new
		{
			x.PId,
			x.PName,
			x.CurrentQty,
			x.LastMovement,
			LastActiveDays = x.LastMovement.HasValue ? (int?)(today 
			-x.LastMovement) : null 	
		}).ToList();
	}
}