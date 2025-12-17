using System;

namespace Maui.TheraOffice.Models;

public class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;
    public string Address   { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; } = DateTime.Today;

    public string Race   { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";
}