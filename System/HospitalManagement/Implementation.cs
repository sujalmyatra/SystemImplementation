using System.Data.SqlTypes;
using System.Dynamic;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;

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
    public int ConsultationFee {get; set;}
    public bool IsActive {get; set;} = true;
   
    public ICollection<Appointment> Appointments {get; set;} = new List<Appointment>();
}
public enum SpecializationType
{
    General,
    ENT,
    Eye,
    Dental
}
public enum AppointmentStatus
{
    Scheduled,
    Completed,
    Cancelled
}
public class Appointment(Guid pid, Guid did, DateTime bf, DateTime bt) : BaseEntity
{
    private Appointment(){}
    public AppointmentStatus Status {get; private set;} = AppointmentStatus.Scheduled;
    public Guid PatientId {get; set;} = pid;
    public Patient Patient {get; set;} = null!;

    public Guid DoctorId {get; set;} = did;
    public Doctor Doctor {get; set;} = null!;

    public Prescription? Prescription {get; set;}

    public Bill? Bill {get; set;}
     public DateTime? BookedFrom {get; set;} = bf;
    public DateTime? BookedTo {get; set;} = bt;

    public void Cancel()
    {
        if(Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Only Scheduled Apt can be Cancelled");

        Status = AppointmentStatus.Cancelled;
    }
    public void Complete()
    {
        if(Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Only Scheduled Apt can be Completed");

        Status = AppointmentStatus.Completed;
    }

}
public class Prescription : BaseEntity
{
    public string Diagnosis {get; set;}
    public string Advice {get; set;}
    public Guid AppointmentId {get; set;}
    public Appointment Appointment {get; set;}

    public ICollection<Medicine> Medicines {get; set;} = new List<Medicine>();
}
public class Bill : BaseEntity
{
    public int ConsultationFee{get; set;}
    public int MedicineCharges{get; set;}
    public int TotalAmount => ConsultationFee + MedicineCharges;

    public DateTime BilledAt {get; set;}
    public Guid AppointmentId {get; set;}
    public Appointment Appointment {get; set;}  
}

public class Medicine : BaseEntity
{
    public string Name{get; set;}
    public string Dosage{get; set;}
    public int Price {get; set;}

    public Guid PrescriptionId {get; set;}
    public Prescription Prescription {get; set;}
}

//Infrastructure
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<Medicine> Medicines => Set<Medicine>();
    public DbSet<Bill> Bills => Set<Bill>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        ConfigurePatient(builder);
        ConfigureDoctor(builder);
        ConfigureAppointment(builder);
    }
    public override Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }
    private static void ConfigurePatient(ModelBuilder builder)
    {
        builder.Entity<Patient>(e =>
        {
            e.HasKey(p => p.Id);
        })
        
    }
     private static void ConfigureDoctor(ModelBuilder builder)
    {
        builder.Entity<Doctor>(e =>
        {
           e.HasKey(d => d.Id);
        });
    }
    private static void ConfigureAppointment(ModelBuilder builder)
    {
        builder.Entity<Appointment>(e =>
        {
            e.HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)   
            .OnDelete(DeleteBehaviour.Restrict);

            e.HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehaviour.Restrict);

            e.HasOne(a => a.Prescription)
            .WithOne(p => p.Appointment)
            .HasForeignKey<Prescription>(p => p.AppointmentId)
            .OnDelete(DeleteBehaviour.Restrict);

            e.HasOne(a => a.Bill)
            .WithOne(b => b.Appointment)
            .HasForeignKey<Bill>(b => b.AppointmentId)
            .OnDelete(DeleteBehaviuor.Restrict);

            e.HasIndex(e => new
            {
                e.DoctorId,
                e.BookedFrom
            });
            e.HasIndex(e => new
            {
                e.PatientId, 
                e.BookedFrom
            });
        });
    }

}

public interface IGenericRepository<T>
    where T : BaseEntity
{
    Task<T?> GetByIdAsync(
        Guid id);

    Task<IEnumerable<T>> GetAllAsync();

    Task AddAsync(T entity);

    void Update(T entity);

    void Delete(T entity);
}

public class GenericRepository<T> : IGenericRepository<T>
    where T : BaseEntity
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    public GenericRepository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(
        Guid id)
    {
        return await DbSet.FirstOrDefaultAsync(
            entity => entity.Id == id,
            cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await DbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(T entity)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        DbSet.Remove(entity);
    }
}
//Specialized Repos
public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    Task<bool> IsDoctorBusyAsync(Guid doctorId, DateTime bookedFrom, DateTime bookedTo);
    Task<bool> PatientHasActiveAppointmentAsync(Guid pId, Guid dId, DateTime appointmentDate);
    Task<Appointment?> GetWithDoctorAsync(Guid appointmentId);
}
public interface IDoctorReposotory : IGenericRepository<Doctor>
{
    Task<Doctor?> GetActiveDoctorAsync(Guid doctorId);
}

public interface IBillRepo : IGenericRepository<Bill>
{
    Task<bool> ExistsForAppointmentAsync(Guid appointmentId);
}
public interface IPrescriptionRepo : IGenericRepository<Prescription>
{
    Task<bool> ExistsForAppointmentAsync(Guid appointmentId);
}

public class DoctorReposotory(AppDbContext context) : GenericRepository<Doctor>, IDoctorReposotory
{
    public Task<Doctor?> GetActiveDoctorAsync(Guid doctorId)
    {
        return context.Doctors
        .FirstOrDefaultAsync(d => d.Id == doctorId && d.IsActive);
    }
    
}

public class AppointmentRepository(AppDbContext context) : GenericRepository<Appointment>, IAppointmentRepository
{
    public Task<bool> IsDoctorBusyAsync(Guid doctorId, DateTime bookedFrom, DateTime bookedTo)
    {
        return  context.Appointments.AnyAsync(x => 
        x.DoctorId == doctorId && 
        x.Status == AppointmentStatus.Scheduled &&
        x.BookedFrom < bookedTo &&
        bookedFrom < x.BookedTo 
        );
        //Logic is 
        // for date FirstStart <= SecondEnd && SecondStart <= FirstEnd
        // for time FirstStart < SecondEnd && SecondStart < FirstEnd
    }
    public async Task<bool> PatientHasActiveAppointmentAsync(Guid pId, Guid dId, DateTime appointmentDate)
    {
        var dayStart = appointmentDate.Date;
        var nextDay = appointmentDate.AddDays(1);

        return context.Appointments.AnyAsync(x => 
            x.PatientId == pId &&
            x.DoctorId == dId &&
            x.BookedFrom >= dayStart &&
            x.BookedFrom < nextDay
        );
    }
    public async Task<Appointment?> GetWithDoctorAsync(Guid appointmentId)
    {
        return context.Appointments.Include(x => x.Doctor)
        .FirstOrDefaultAsync(x => x.Id == appointmentId);
    }
}



public class BillRepository(AppDbContext context) : GenericRepository<Bill>, IBillRepository
{
    public Task<bool> ExistsForAppointmentAsync(Guid appointmentId)
    {
        return context.Bills.AnyAsync(x => x.AppointmentId == appointmentId);
    }
}

public class PrescriptionRepository(AppDbContext context) : GenericRepository<Prescription>, IPrescriptionRepository
{
    public Task<bool> ExistsForAppointmentAsync(Guid appointmentId)
    {
        return context.Prescriptions.AnyAsync(x => x.AppointmentId == appointmentId);
    }
    
}

public record DoctorAppointmentCountDto(Guid DoctorId, string DoctorName, int AppointmentCount);
public interface IReportRepository
{
    Task<DoctorAppointmentCountDto> GetDoctorAppointmentCountAsync(DateTime st, DateTime et);
}
public class ReportRepository(AppDbContext context) : IReportRepository
{
    //1
    public Task<DoctorAppointmentCountDto> GetDoctorAppointmentCountAsync(DateTime st, DateTime et)
    {
        var startDate = st;
        var endDateExec = et.AddDays(1);

        return context.Doctors.AsNoTracking()
            .Select(x => new DoctorAppointmentCountDto
            {
                DoctorId = x.Id,
                DoctorName = x.Name,
                AppointmentCount = x.Appointments.Count(x => x.BookedFrom >= startDate && x.BookedFrom < endDateExec)
            })
            .OrderBy(x => x.DoctorName)
            .ToListAsync();
    }
    //2
    public Task<PatientAppointmentHistoryDto?> GetPatientAppointmentHistoryAsync(Guid pId)
    {
        return context.Patients
        .AsNoTracking()
        .Where(x => x.Id == pId)
        .Select(x => new PatientAppointmentHistoryDto
        (
            x.Id,
            x.Name,
            x.Appointments
            .OrderBy(x => x.BookedFrom)
            .Select(x => new PatientAppointmentItemDto
            (
                x.Id,
                x.BookedFrom,
                x.Doctor.Name,
                x.Status
            )).ToList()
        ))
        .SingleOrDefaultAsync();
    }
    //3
    public Task<List<DoctorRevenueReportDto>> GetDoctorRevenueReportAsync()
    {
        return context.Doctors
        .AsNoTracking()
        .Select(x => new DoctorRevenueReportDto
        (
            x.Id,
            x.Name,
            x.Appointments.Count(x => x.Status == AppointmentStatus.Completed),
            x.ConsultationFee,
            x.Appointments
            .Count(x => x.Status == AppointmentStatus.Completed) * x.ConsultationFee
        ))
        .OrderByDescending(x => x.TotalRevenue)
        .ToListAsync();
    }
    //6
    public Task<List<AppointmentStatusReportItemDto>> GetAppointmentStatusReportAsync()
    {
        return context.Appointments
        .AsNoTracking()
        .GroupBy(x => x.Status)
        .Select(g => new AppointmentStatusReportItemDto(
            g.Key,
            g.Count()
        )).ToListAsync();
    }

}
public record PatientAppointmentItemDto(Guid AppointmentId,DateTime? AppointmentDAte, string DoctorName, AppointmentStatus Status);

public record PatientAppointmentHistoryDto
(Guid PatientId, string PatientName, List<PatientAppointmentItemDto> Appointments);

public record DoctorRevenueReportDto
(Guid DoctorId, string DoctorName, int TotalCompletedAppointments, int ConsultationFee, decimal TotalRevenue);


public record AppointmentStatusReportItemDto
(AppointmentStatus Status, int Count);

public interface IUnitOfWork
{
    IGenericRepository<Patient> Patients{get;}
    IGenericRepository<Medicine> Medicines{get;}
    IDoctorReposotory Doctors{get;}
    IAppointmentRepository Appointments {get;}
    IPrescriptionRepo Prescriptions {get;}
    IBillRepo Bills{get;}
    IReportRepository Reports {get;}
    Task<int> SaveChangesAsync();
}
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public IGenericRepository<Patient> Patients{get;} = new GenericRepository<Patient>(context);

    public IGenericRepository<Medicine> Medicines{get;} = new GenericRepository<Medicine>(context);
    public IDoctorReposotory Doctors{get;} = new DoctorRepository(context);
    public IAppointmentRepository Appointments {get;} =
     new AppointmentRepository(context);
    public IPrescriptionRepo Prescriptions {get;} = new PrescriptionRepository(context);
    public IBillRepo Bills{get;}  = new BillRepository(context);
    public IReportRepository Reports {get;} = new ReportRepository(context);

    public Task<int> SaveChangesAsync()
    {
        return context.SaveChangesAsync();
    }
}


//Service
//Dto

public record BookRequestDto(Guid PId, Guid DId, DateTime BFrom, DateTime BTo);
public record BookResponseDto(Guid AppointmentId);
public interface IAppointmentService
{
    Task<BookResponseDto> BookAsync(BookRequestDto req);
    Task CancleAsync(Guid appointmentId);
    Task CompleteAsync(Guid appointmentId);

}
public class AppointmentService(IUnitOfWork uow) : IAppointmentService
{
    public async Task<BookResponseDto> BookAsync(BookRequestDto req)
    {
        if(req.BFrom >= req.BTo)
        {
            throw new InvalidOperationException("BookedTo Must be after BookedFrom");
        }
        var patient = await uow.Patients.GetByIdAsync(req.PId);

        if(patient is null)
            throw new KeyNotFoundException("Patient does not exist");

        var doctor = await uow.Doctors.GetActiveDoctorAsync(req.DId);

        if(doctor is null)
            throw new KeyNotFoundException("Doctor does not exist or inactive");

        bool busy = await uow.Appointments.IsDoctorBusyAsync(req.DId, req.BFrom, req.BTo);

        if(busy)
            throw new InvalidOperationException("Doctor is already busy at this time");

        bool patienthasappointment = await uow.Appointments.PatientHasActiveAppointmentAsync(req.PId, req.DId, req.BFrom);

        if(patienthasappointment)
            throw new InvalidOperationException("Patient has already appointment at this time with this doctor");

        var appointment = new Appointment
        {
            PatientId = req.PId,DoctorId = req.DId,
            BookedFrom = req.BFrom, BookedTo = req.BTo            
        };

        await uow.Appointments.AddAsync(appointment);

        await uow.SaveChangesAsync();

        return new BookResponseDto(appointment.Id);
    }
    public async Task CancleAsync(Guid appointmentId)
    {
        var app = await uow.Appointments.GetByIdAsync(appointmentId);

        if(app is null)
            throw new KeyNotFoundException("Appointment does not exist");

        app.Cancel();

        await uow.SaveChangesAsync();
    }
    public async Task CompleteAsync(Guid appointmentId)
    {
        var app = await uow.Appointments.GetByIdAsync(appointmentId);
         if(app is null)
            throw new KeyNotFoundException("Appointment does not exist");

        app.Complete();

        await uow.SaveChangesAsync();
    }

}
public class Program
{
    public static List<Patient> patients = new();
    public static  List<Doctor> doctors = new();
    public static List<Appointment> appointments  = new();
    // public List<Patient> patients = new();
    // public List<Patient> patients = new();

    public static async Task Main()
    {
        //1
        var startDate = new DateTime(2026, 7, 1);
        var endDate = new DateTime(2026, 7, 5);
        var endDateExec = endDate.AddDays(1);


        var docapp = doctors.LeftJoin(appointments, d => d.Id, a => a.DoctorId, (d, a) => new
        {
            d, a
        })
        .Where(x => x.a.BookedFrom.Value.Date >= startDate && x.a.BookedTo.Value.Date <= endDate)
        .GroupBy(x => new{DocId = x.d.Id, DocName =  x.d.Name})
        .Select(g => new
        {
            DoctorId = g.Key.DocId,
            DoctorName = g.Key.DocName,
            Appointments = g.Count()
        });

        //best approach
        var doc = await context.doctors.
            AsNoTracking()
            .Select(x => new
            {
                x.Id,
                x.Name,
                AppointmentCount = x.Appointments.Count(appointment => appointment.BookedFrom >= startDate && appointment.BookedFrom < endDateExec)
            })
            .ToListAsync();
    }
}