namespace Maui.TheraOffice.Models;

public class MedicalNote
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }

    public DateTime NoteDate { get; set; } = DateTime.Now;

    public string Diagnosis     { get; set; } = string.Empty;
    public string Prescription  { get; set; } = string.Empty;
    public string FreeTextNotes { get; set; } = string.Empty;
}
