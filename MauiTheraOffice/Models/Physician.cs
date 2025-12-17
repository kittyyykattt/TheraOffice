namespace Maui.TheraOffice.Models;

public class Physician
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;

    public string LicenseNumber  { get; set; } = string.Empty;
    public DateTime GraduationDate { get; set; } = DateTime.Today;

    public string Specializations { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";
}
