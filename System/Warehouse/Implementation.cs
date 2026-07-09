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
	public Product Product {get;set;}
	
	public Guid WarehouseId {get;set;}
	public Warehouse Warehouse {get;set;}

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
	public Product Product {get; set;}

	public Guid? SupplierId {get; set;}
	public Supplier? Supplier {get; set;}

	public Guid? FromWarehouseId {get;set;}
	public Warehouse? FromWarehouse {get;set;}

	public Guid? ToWarehouseId {get;set;}
	public Warehouse? ToWarehouse {get;set;}
	
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