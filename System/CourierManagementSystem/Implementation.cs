public abstract class BaseEntity
{
    public Guid Id {get; set;} = Guid.NewGuid();
}

public class Customer : BaseEntity
{
    public string Name {get; set;}
    public string PhoneNumber{get; set;}
    public CustomerType CustomerType{get; set;}
    public ICollection<Parcel> Parcels {get; set;} = new List<Parcel>();
    public ICollection<ShipmentTrackHistory> ShipmentTrackHistories {get; set;} = new List<ShipmentTrackHistory>();
}
public enum CustomerType
{
    Sender = 1,
    Receiver
}

public class Parcel : BaseEntity
{
    public Guid SenderId {get; set;}
    public Customer Sender {get; set;}

    public Guid ReceiverId {get; set;}
    public Customer Receiver {get; set;}

    public Guid ShipmentId {get; set;}
    public Shipment Shipment {get; set;}

    public string PickUpAddress{get; set;}
    public string DeliveryAddress{get; set;}
    public decimal Weight{get; set;}
    public int Distance{get; set;}

    public decimal? BaseCharge{get; set;}
    public decimal? WeightCharge{get; set;}
    public decimal? DistanceCharge{get; set;}

    public decimal? TotalCharge{get; set;}

}
public enum ShipmentStatus
{
    Created,
    PickedUp,
    InTransit,
    OutForDelivery,
    Delivered,
    Cancelled,
    Returned

}
public class Shipment : BaseEntity
{
    public ShipmentStatus Status{get; set;}

    public Guid ParcelId {get; set;}
    public Parcel Parcel {get; set;}
    public Guid DeliveryAgentlId {get; set;}
    public DeliveryAgent DeliveryAgent {get; set;}
    public ICollection<ShipmentTrackHistory> ShipmentTrackHistories {get; set;} = new List<ShipmentTrackHistory>();

    public void PickUp()
    {
        
    }
    public void InTransit()
    {
        
    }
    public void OutForDelivery()
    {
        
    }
    public void Deliver()
    {
        
    }
    public void Cancel()
    {
        
    }
    public void Return()
    {
        
    }

}
public class DeliveryAgent : BaseEntity
{
    public string Name{get; set;}
    public string PhoneNumber{get; set;}

    public bool IsActive{get; set;}

    public ICollection<Shipment> Shipments {get; set;}
    = new List<Shipment>();

}

public class ShipmentTrackHistory : BaseEntity
{
    public Guid CustomerId{get; set;}
    public Customer Customer{get; set;}
    public Guid ShipmentId {get; set;}
    public Shipment Shipment {get; set;}

    public ShipmentStatus CurrentStatus{get; set;}

    public string Location{get; set;}
    public DateTime Date{get; set;}
    public string? Remarks{get; set;}
    public string TrackingStatement{get; set;}
}