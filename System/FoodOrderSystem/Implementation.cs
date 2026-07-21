public abstract class BaseEntity
{
    public Guid Id {get; set;} = Guid.NewGuid();
}

public class Restaurant : BaseEntity
{
    public string Name{get; set;}
    public string Address{get; set;}
    public string Contact{get; set;}

    public bool IsActive {get; set;} = true;

    public ICollection<MenuItem> MenuItems{get; set;}
    = new List<MenuItem>();
    
    public ICollection<Order> Orders{get; set;}
    = new List<Order>();
}

public enum ItemCategory
{
    Chieneese,
    Punjabi,
    Italian
}

public class MenuItem : BaseEntity
{
     public string ItemName{get; set;}
    public decimal  Price {get; set;}
    public ItemCategory Category{get; set;}
     public MenuItemStatus Status {get; set;} = MenuItemStatus.Available;

    public Guid RestaurantId{get; set;}
    public Restaurant  Restaurant {get; set;}

    public ICollection<OrderItem> OrderItems{get; set;}
    = new List<OrderItem>();

}
public class Customer : BaseEntity
{
    public string Name{get; set;}
    public string Contact{get; set;}


     public ICollection<Order> Orders{get; set;}
    = new List<Order>();
}

public enum MenuItemStatus
{
    Available,
    UnAvailable
}
public enum OrderStatus
{
    Accepted,
    Rejected,
    Placed,
    Accepted,
    Preparing,
    ReadyForPickup,
    OutforDelivery,
    Delivered,
    Cancelled
}
public class Order : BaseEntity
{
    public Guid CustomerId{get; set;}
    public Customer  Customer {get; set;}= null!;

    public Guid RestaurantId{get; set;}
    public Restaurant  Restaurant {get; set;}= null!;

    public Guid? DeliveryPartnerId{get; set;}
    public DeliveryPartner?  DeliveryPartner {get; set;}

    public DateTime PlacedAt{get; set;}
    public DateTime? DeliveredAt{get; set;}

     public OrderStatus Status {get; set;} = OrderStatus.Placed;

    public decimal DistanceKm {get; set;}
public decimal SubTotal { get; set; }
public decimal DeliveryCharge { get; set; }
public decimal FinalAmount { get; set; }

    public Payment? Payment{get; set;}
    public ICollection<OrderItem> OrderItems{get; set;}
    = new List<OrderItem>();
}
public class OrderItem : BaseEntity
{
    public int Quantity{get; set;}
    public decimal UnitPrice { get; set; }

    public decimal LineTotal => UnitPrice * Quantity;

    public Guid OrderId{get; set;}
    public Order Order {get; set;}= null!;

    public Guid MenuItemId{get; set;}
    public MenuItem  MenuItem {get; set;}= null!;
}
public class DeliveryPartner : BaseEntity
{
     public string Name{get; set;}
    public string Contact{get; set;}

    public ICollection<Order> Orders{get; set;}
    = new List<Order>();

    public bool IsActive {get; set;}

}

public class Payment : BaseEntity
{
   public Guid OrderId{get; set;}
   public Order Order {get; set;}= null!;

   public decimal Amount { get; set; }

   public PaymentStatus Status {get; set;} = Payment.Pending;


   public string? TransactionRefrence{get; set;}
   public DateTime? PaidAt{get; set;}
   public DateTime? Refunded{get; set;}


}