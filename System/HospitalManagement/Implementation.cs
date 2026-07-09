public abstract class BaseEntity
{
    public Guid Id {get; protected set;} = Guid.NewGuid();
}

public class Patient : BaseEntity
{
    public string Name {get; set;}
    public ICollection<Appointment> Appointments {get; set;} = new List<Appointment>();
}
public class Doctor : BaseEntity
{
    public string Name {get; set;}
    public SpecializationType Type {get; set;}
    public bool IsOcupied {get; set;}
   
    public ICollection<Appointment> Appointments {get; set;} = new List<Appointment>();
}
public enum SpecializationType
{
    General,
    ENT,
    Cancelled
}
public enum AppointmentStatus
{
    Scheduled,
    Completed,
    Cancelled
}
public class Appointment : BaseEntity
{
    public AppointmentStatus Type {get; set;}
    public Guid PatientId {get; set;}
    public Patient Patient {get; set;}

    public Guid DoctorId {get; set;}
    public Doctor Doctor {get; set;}

     public Guid PrescriptionId {get; set;}
    public Prescription Prescription {get; set;}

     public Guid BillId {get; set;}
    public Bill Bill {get; set;}

     public DateTime? BookedFrom {get; set;}
    public DateTime? BookedTo {get; set;}


}
public class Prescription : BaseEntity
{

    public string Guidance {get; set;}
    public Guid AppointmentId {get; set;}
    public Appointment Appointment {get; set;}

    public ICollection<Medicine> Medicines {get; set;} = new List<Medicine>();
}
public class Bill 
{
    public int Amount{get; set;}
    public DateTime BilledAt {get; set;}
    public Guid AppointmentId {get; set;}
    public Appointment Appointment {get; set;}  
}

public class Medicine
{
    public string Name{get; set;}
    public string Dosage{get; set;}

    public Guid PrescriptionId {get; set;}
    public Prescription Prescription {get; set;}
}