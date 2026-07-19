using System.Diagnostics;
using System.Security.Principal;
using Microsoft.VisualBasic;

public abstract class BaseEntity
{
    public Guid Id {get; protected set;}
}
public interface ISoftDeletable
{
    bool IsDeleted {get; set;}
}
public interface IParcelChargePolicy
{
    CalculatedCharges Calculate(decimal baseCharge,decimal WeightCharge,decimal DistanceCharge );
}
public record CalculatedCharges(decimal BaseCharge, decimal WeightCharge, decimal DistanceCharge, decimal TotalCharge);

public class ParcelChargePolicy : IParcelChargePolicy
{
    public CalculatedCharges Calculate(decimal baseCharge,decimal Weight,decimal Distance)
    {
         
        var  weight = CalculateGeneric<decimal, int>(Weight, 5, 30, 15);
        var  distance = CalculateGeneric(Distance, 10, 100, 60);

        var total = baseCharge + CalculateGeneric(Weight, 5, 30, 15) + CalculateGeneric(Distance, 10, 100, 60);

        return new CalculatedCharges(baseCharge, weight, distance, total);

    }
    private T CalculateGeneric<T, U>(T variable,U c,U a, U b)
    {
        var charge = variable > c ? a : b;

        return charge * variable;
    }
}



public enum ShipmentStatus
{
    Created ,
    PickedUp, 
    InTransit,
    OutForDelivery,
    Delivered,
    Cancelled,
    Returned
}
public class Customer : BaseEntity
{
    public string Name{get; set;}
    public string PhoneNumber{get; set;}
    public string Address{get; set;}

    public ICollection<Parcel> SentParcels {get; set; } = new List<Parcel>();
    public ICollection<Parcel> ReceivedParcels {get; set; } = new List<Parcel>();
}

public class Parcel : BaseEntity
{
    public DateTime CreatedAt{get; set;} = DateTime.UtcNow;

    public decimal WeightKg{get; set;}
    public decimal DistanceKm{get; set;}

    public string PickUpAddress{get; set;}
    public string DeliveryAddress{get; set;}


    public Guid SenderId{get; set;}
    public Customer Sender{get; set;} = null!;

     public Guid ReceiverId{get; set;}
    public Customer Receiver{get; set;} = null!;


    public Shipment? Shipment{get; set;}
    
}
public class Shipment : BaseEntity, ISoftDeletable
{
    public bool IsDeleted{get; set;} = false;

    public DateTime CreatedAt{get; set;} = DateTime.UtcNow;
    public DateTime? PickedUpAt{get; set;}
    public DateTime ExpectedDeliveryAt{get; set;}
    public DateTime? DeliveredAt{get; set;} = DateTime.UtcNow;




    public ShipmentStatus Status{get; set;} = ShipmentStatus.Created;
    

    public decimal BaseCharge{get; set;}
    public decimal WeightCharge{get; set;}
    public decimal DistanceCharge{get; set;}
    public decimal TotalCharge{get; set;}

    public Guid ParcelId{get; set;}
    public Parcel Parcel{get; set;} = null!;


    public Guid? DeliveryAgentId{get; set;}
    public DeliveryAgent DeliveryAgent{get; set;} = null!;

    public ICollection<ShipmentTracking> ShipmentTrackings {get; set; } = new List<ShipmentTracking>();

}
public class ShipmentTracking : BaseEntity
{
    public ShipmentStatus Status{get; set;}

    public string Location{get; set;} = string.Empty;
    public string? Remarks{get; set;}

    public DateTime UpdatedAt{get; set;}


    public Guid ShipmentId{get; set;}
    public Shipment Shipment{get; set;} = null!;
}

public class DeliveryAgent : BaseEntity
{
    public string Name{get; set;}
    public string PhoneNumber{get; set;}

    public bool IsActive{get; set;}

    public ICollection<Shipment> Shipments {get; set; } = new List<Shipment>();
}



// actual class
// public sealed class ParcelChargePolicy : IParcelChargePolicy
// {
//     public CalculatedCharges Calculate(
//         decimal baseCharge,
//         decimal weightKg,
//         decimal distanceKm)
//     {
//         if (weightKg <= 0)
//             throw new ArgumentOutOfRangeException(
//                 nameof(weightKg),
//                 "Weight must be greater than zero.");

//         if (distanceKm < 0)
//             throw new ArgumentOutOfRangeException(
//                 nameof(distanceKm),
//                 "Distance cannot be negative.");

//         var weightCharge = CalculateTierCharge(
//             value: weightKg,
//             threshold: 5m,
//             rateAboveThreshold: 30m,
//             rateAtOrBelowThreshold: 15m);

//         var distanceCharge = CalculateTierCharge(
//             value: distanceKm,
//             threshold: 10m,
//             rateAboveThreshold: 100m,
//             rateAtOrBelowThreshold: 60m);

//         var totalCharge =
//             baseCharge +
//             weightCharge +
//             distanceCharge;

//         return new CalculatedCharges(
//             baseCharge,
//             weightCharge,
//             distanceCharge,
//             totalCharge);
//     }

//     private static decimal CalculateTierCharge(
//         decimal value,
//         decimal threshold,
//         decimal rateAboveThreshold,
//         decimal rateAtOrBelowThreshold)
//     {
//         var selectedRate = value > threshold
//             ? rateAboveThreshold
//             : rateAtOrBelowThreshold;

//         return value * selectedRate;
//     }
// }

// generic 
// private static TResult CalculateCharge<TValue, TResult>(
//     TValue value,
//     TValue threshold,
//     TResult rateAboveThreshold,
//     TResult rateAtOrBelowThreshold)
//     where TValue : INumber<TValue>
//     where TResult : INumber<TResult>
// {
//     var selectedRate = value > threshold
//         ? rateAboveThreshold
//         : rateAtOrBelowThreshold;

//     var convertedValue = TResult.CreateChecked(value);

//     return convertedValue * selectedRate;
// }

// Generic numeric calculation
// public static T CalculateCharge<T>(
//     T value,
//     T threshold,
//     T highRate,
//     T lowRate)
//     where T : INumber<T>
// {
//     var rate = value > threshold
//         ? highRate
//         : lowRate;

//     return value * rate;
// }   

//Design chatgpt
// https://chatgpt.com/share/6a5cdd7f-488c-83e8-9520-c7c662ab0ddc

// static vs instance
// https://chatgpt.com/share/6a5cdd34-7964-83e8-b4ca-6e2a5ad7a01b