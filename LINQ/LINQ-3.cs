namespace practce;
public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Code { get; set; } = ""; 
    // Example: CARD, ORTHO, DERM
}

public class Doctor
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int DepartmentId { get; set; }
    public decimal ConsultationFee { get; set; }
}

public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Email { get; set; }
    public DateTime RegisteredAt { get; set; }
}

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = "";
    // Booked, Completed, Cancelled
}

public class Payment
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "";
    // Paid, Failed, Refunded
    public DateTime? PaidAt { get; set; }
}

public class Prescription
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public string MedicineName { get; set; } = "";
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
}

//https://chatgpt.com/share/6a4e30b0-1bb0-83e8-85cc-7b4b3b249229
public class Program
{




public static void Main()
{
    
    var today = DateTime.Today;
    var tomorrow = today.AddDays(1);

    var last7Days = today.AddDays(-7);
    var last30Days = today.AddDays(-30);

    var firstDayThisMonth = new DateTime(today.Year, today.Month, 1);
    var firstDayLastMonth = firstDayThisMonth.AddMonths(-1);


//Question List
// Q1
// Show all appointments from the last 7 days with patient name, doctor name, department name, and appointment date. Oldest appointments should appear first.

// Q2
// Show latest 5 completed appointments with patient name, doctor name, and appointment date.

// Q3
// Show patient-wise appointment count for the last 30 days. Patients with higher appointment count should appear first.

// Q4
// Show department-wise paid revenue for last month.

// Q5
// Show all doctors with their last 30 days appointment count. Doctors with no appointments should show count 0.

// Q6
// Show all patients with their latest appointment date. If patient has no appointment, show null.

// Q7
// Show the first appointment of today with patient name and doctor name.

// Q8
// Show the next upcoming appointment for a given patient. If no upcoming appointment exists, return null.

// Q9
// Show the last appointment recorded in the system.

// Q10
// Show the last completed appointment of a given patient. If not found, return null.

// Q11
// Find the department whose code is "CARD". There must be exactly one matching department.

// Q12
// Find patient by email. If no patient exists, return null. If duplicate emails exist, throw error.

// Q13
// Show doctors who handled more than 10 appointments last month.

// Q14
// Show patients who have never made any payment.

// Q15
// Show medicine-wise total prescribed quantity for last month.

// Q16
// Show appointment-status-wise count for last month.
}
}
