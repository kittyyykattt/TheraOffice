namespace Maui.TheraOffice.Models;

public class Appointment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PatientId   { get; set; }
    public Guid PhysicianId { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime   { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string Room { get; set; } = string.Empty;
}
