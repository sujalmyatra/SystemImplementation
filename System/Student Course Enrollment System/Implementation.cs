public abstract class BaseEntity
{
    public Guid Id {get; protected set;} = Guid.NewGuid();
    public bool IsDeleted{get; set;} = false;

    public void MarkAsDeleted()
    {
        IsDeleted = true;
    }
}
public enum EnrollmentStatus
{
    Active = 1,
    Completed,
    Cancelled,
    Failed 
}
public enum CourseStatus
{
    Active = 1,
    InActive 
}
public enum AttendanceType
{
    Present = 1,
    Absent
}
public class Student : BaseEntity
{
    public string Name {get; set;}
    public ICollection<Enrollment> Enrollments {get; set;} = new List<Enrollment>();
}
public class Course : BaseEntity
{
    public int MaxCapacity {get; set;}
    public CourseStatus Status{get; set;}
    public string CourseName {get; set;}

    public Guid TeacherId{get; set;}
    public Teacher Teacher{get; set;}

    public ICollection<Enrollment> Enrollments {get; set;} = new List<Enrollment>();

    public bool IsActive()
    {
        return Status == CourseStatus.Active;
    }
    public void Increase()
    {
        if(CurrentCapacity == MaxCapacity)
            throw new CapacityExeededException("Coaurse Capacity is full");

        CurrentCapacity++;
    }
    public void Decrease()
    {
        if(CurrentCapacity == 0)
            throw new LowestCapacityException("Coaurse Capacity is full");
            
        CurrentCapacity--;
    }

}
public class Teacher : BaseEntity
{
    public string Name{get; set;}
    public ICollection<Course> Courses {get; set;} = new List<Course>();
}
public class Attendance : BaseEntity
{
    public Guid EnrollmentId{get; set;}
    public Enrollment Enrollment{get; set;}

    public DateTime AttendanceDate{get; set;}

    public AttendanceType Type {get; set;}
}


public class Marks : BaseEntity
{
    public Guid EnrollmentId{get; set;}
    public Enrollment Enrollment{get; set;} = null!;

    public int ObtainedMarks {get; set;}

    public void AddMarks(int marks)
    {
        if(marks < 0 || marks >100)
            throw new InvalidOperationException("Entered Marks are invalid.");
        ObtainedMarks = marks;
    }

    public bool IsPassed()
    {
        return ObtainedMarks >= 35;
    }
}

public class Enrollment  : BaseEntity
{
    public EnrollmentStatus Status {get; set;} = EnrollmentStatus.Active;
    public DateTime EnrollmentDate {get; set;} = DateTime.Now;

    public Guid StudentId{get; set;}
    public Student Student{get; set;}

    public Guid CourseId{get; set;}
    public Course Course{get; set;}

    public ICollection<Attendance> Attendances {get; set;}
    = new List<Attendance>();

    public Marks? Marks{get; set;}

    public bool IsActive()
    {
       return Status == EnrollmentStatus.Active;
    }
}

//Infrastructure

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {}

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Marks> Marks => Set<Marks>();


    protected override void OnModelCreating(ModelBuilderr builder)
    {
        base.OnModelCreating(builder);
        ConfigureStudent(builder);
        ConfigureTeacher(builder);
        ConfigureEnrollment(builder);
        ConfigureCourse(builder);
        ConfigureAttendance(builder);

    }
    private static void ConfigureStudent(ModelBuilderr builder)
    {
        builder.Entity<Student>(e =>
        {
           e.ToTable("Students");

            e.HasQueryFilter(s => !s.IsDeleted); 
        });
    }
    private static void ConfigureTeacher(ModelBuilderr builder)
    {
         builder.Entity<Teacher>(e =>
        {
           e.ToTable("Teachers");

            e.HasQueryFilter(t => !t.IsDeleted); 
        });    
    }
    private static void ConfigureCourse(ModelBuilderr builder)
    {
        builder.Entity<Course>(e =>
        {
            e.ToTable("Courses");

            e.HasOne(c => c.Teacher)
            .WithMany(e => e.Courses)
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehaviour.Restrict);

            e.HasQueryFilter(x => !x.IsDeleted);
        });
    }
    private static void ConfigureEnrollment(ModelBuilderr builder)
    {
        builder.Entity<Enrollment>(e =>
        {
            e.ToTable("Enrollments");

            e.HasOne(e => e.Student)
            .WithMany(e => e.Enrollments)
            .HasForeignKey(c => c.StudentId)
            .OnDelete(DeleteBehaviour.Restrict);

             e.HasOne(e => e.Course)
            .WithMany(e => e.Enrollments)
            .HasForeignKey(c => c.CourseId)
            .OnDelete(DeleteBehaviour.Restrict);

            e.HasIndex(e => new
            {
                e.StudentId, e.CourseId
            });
            e.HasIndex(e => new
            {
                e.CourseId, e.Status
            });

            e.HasQueryFilter(x => !x.IsDeleted);
        });
    }
   
    private static void ConfigureAttendance(ModelBuilderr builder)
    {
        builder.Entity<Attendance>(e =>
        {
            e.ToTable("Attendances");

            e.HasOne(e => e.Enrollment)
            .WithMany(e => e.Attendances)
            .HasForeignKey(c => c.EnrollmentIdId)
            .OnDelete(DeleteBehaviour.Restrict);


            e.HasQueryFilter(x => !x.IsDeleted);
        });
    }
    public override Task<int> SaveChangesAsync()
    {
        ConvertDeletesTosoftDelete();
        base.SaveChangesAsync();
    }

    private void ConvertDeletesTosoftDelete()
    {
        var deletedEntries = ChangeTracker
        .Entires<BaseEntity>()
        .Where(x => x.State == EntityState.Deleted);

        foreach(var e in deletedEntries)
        {
                e.State = EntityState.Modified;
                e.MarkAsDeleted();            
        }

        //Old way
        //     var entries = ChangeTracker.Entries();

        // foreach (var entry in entries)
        // {
        //     if (entry.Entity is BaseEntity entity &&
        //         entry.State == EntityState.Deleted)
        //     {
        //         entry.State = EntityState.Modified;
        //         entity.MarkAsDeleted();
        //     }
        // }
    }
}

public interface IEnrollmentRepository
{
    Task<bool> HasActiveEnrollmentsAsync(Guid studentId, Guid courseId);
    Task<int> CountActiveEnrollmentsAsync(Guid courseId);
    Task<List<Enrollment>> GetStudentEnrollmentsAsync(Guid studentId);
    Task<List<Enrollment>> GetStudentEnrollmentsAsync(Guid courseId);
}
public class EnrollmentRepository(AppDbContext context) :GenericRepository<Enrollment>(context), IEnrollmentRepository
{
    public async Task<bool> HasActiveEnrollmentsAsync(Guid studentId, Guid courseId)
    {
        return await context.Enrollments.AnyAsync(x => x.StudentId == studentId &&
        x.CourseId = courseId && x.Status == EnrollmentStatus.Active
        );
    }
    public async Task<int> CountActiveEnrollmentsAsync(Guid courseId)
    {
        return await context.Enrollments.CountAsync(x => x.CourseId == courseId  && x.Status == EnrollmentStatus.Active );
    }
    public async Task<List<Enrollment>> GetStudentEnrollmentsAsync(Guid studentId)
    {
        return await context.Enrollments
        .Where(e => e.StudentId == studentId)
        .Include(c => c.Course).ThenInclude(x => x.Teacher)
        .OrderByDescending(x => x.EnrollmentDate)
        .AsNoTracking()
        .ToListAsync();
    }
    public async Task<List<Enrollment>> GetCourseEnrollmentsAsync(Guid courseId)
    {
        return await context.Enrollments
        .Where(x => x.CourseId == courseId)
        .Include(x => x.Student)
        .OrderBy(x => x.Student.Name)
        .AsNoTracking()
        .ToListAsync();
    }
}
public interface IAttendanceRepository
{
    Task<bool> AttendanceExistsAsync(Guid enrollmentId, DateTime attendanceDate);
    Task<List<Attendance>> GetEnrollmentAttendanceAsync(Guid enrollmentId,DateTime fromDate, DateTime toDate);
}
public class AttendanceRepository(AppDbContext context) : GenericRepository<Attendance>(context), IAttendanceRepository
{
    public async Task<bool> AttendanceExistsAsync(Guid enrollmentId, DateTime attendanceDate)
    {
        return await context.Attendances.AnyAsync(x => x.EnrollmentId == enrollmentId && 
        x.AttendanceDate == attendanceDate
        );
    }
    public async Task<List<Attendance>> GetEnrollmentAttendanceAsync(Guid enrollmentId,DateTime fromDate, DateTime toDate)
    {
        DateTime startDate = fromDate;
        DateTime endDateExec = toDate.AddDays(1);

        return await context.Attendances
        .Where(x => x.EnrollmentId == enrollmentId && x.EnrollmentDate >= startDate && x.EnrollmentDate < endDateExec)
        .AsNoTracking()
        .ToListAsync();
    }
}
public interface IMarksRepository
{
    Task<Marks> GetByEnrollmentIdAsync(Guid enrollmentId);
    
}

public interface IAcademicReportRepository
{
    
}