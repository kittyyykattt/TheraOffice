using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Maui.TheraOffice.Models;
using Library.eCommerce.Utilities;
using Newtonsoft.Json;

namespace Maui.TheraOffice.Services;

public class DataService
{
    public static DataService Current { get; } = new DataService();

    public ObservableCollection<Patient> Patients { get; } = new();
    public ObservableCollection<Physician> Physicians { get; } = new();
    public ObservableCollection<Appointment> Appointments { get; } = new();
    public ObservableCollection<MedicalNote> Notes { get; } = new();
    public ObservableCollection<Diagnosis> Diagnoses { get; } = new();
    public ObservableCollection<Treatment> Treatments { get; } = new();

    private readonly WebRequestHandler _web = new WebRequestHandler();

    private DataService()
    {
    }

    public async Task LoadPatientsFromApiAsync()
    {
        var json = await _web.Get("/api/patients");
        if (string.IsNullOrWhiteSpace(json))
            return;

        var list = JsonConvert.DeserializeObject<List<Patient>>(json) ?? new List<Patient>();

        Patients.Clear();
        foreach (var p in list)
            Patients.Add(p);
    }

    public async Task SearchPatientsAsync(string query)
    {
        var encoded = Uri.EscapeDataString(query ?? string.Empty);
        var json = await _web.Get($"/api/patients/search?query={encoded}");
        if (string.IsNullOrWhiteSpace(json))
            return;

        var list = JsonConvert.DeserializeObject<List<Patient>>(json) ?? new List<Patient>();

        Patients.Clear();
        foreach (var p in list)
            Patients.Add(p);
    }

    public void AddOrUpdatePatient(Patient patient)
    {
        var existing = Patients.FirstOrDefault(p => p.Id == patient.Id);
        bool isNew = existing == null;

        if (isNew)
        {
            Patients.Add(patient);
        }
        else
        {
            var index = Patients.IndexOf(existing!);
            Patients[index] = patient;
        }

        if (isNew)
            _ = CreatePatientApiAsync(patient);
        else
            _ = UpdatePatientApiAsync(patient);
    }

    public void DeletePatient(Patient patient)
    {
        if (patient == null)
            return;

        var relatedAppts = Appointments.Where(a => a.PatientId == patient.Id).ToList();
        foreach (var a in relatedAppts)
            DeleteAppointment(a);

        var relatedNotes = Notes.Where(n => n.PatientId == patient.Id).ToList();
        foreach (var n in relatedNotes)
            Notes.Remove(n);

        Patients.Remove(patient);

        _ = DeletePatientApiAsync(patient);
    }

    private async Task CreatePatientApiAsync(Patient patient)
    {
        try
        {
            await _web.Post("/api/patients", patient);
        }
        catch
        {
        }
    }

    private async Task UpdatePatientApiAsync(Patient patient)
    {
        try
        {
            await _web.Put($"/api/patients/{patient.Id}", patient);
        }
        catch
        {
        }
    }

    private async Task DeletePatientApiAsync(Patient patient)
    {
        try
        {
            await _web.Delete($"/api/patients/{patient.Id}");
        }
        catch
        {
        }
    }

    public void AddOrUpdatePhysician(Physician physician)
    {
        var existing = Physicians.FirstOrDefault(p => p.Id == physician.Id);
        if (existing == null)
        {
            Physicians.Add(physician);
        }
        else
        {
            var index = Physicians.IndexOf(existing);
            Physicians[index] = physician;
        }
    }

    public void DeletePhysician(Physician physician)
    {
        var hasAppointments = Appointments.Any(a => a.PhysicianId == physician.Id);
        if (hasAppointments)
            throw new InvalidOperationException("Cannot delete physician with scheduled appointments.");

        Physicians.Remove(physician);
    }

    public void AddOrUpdateAppointment(Appointment appt)
    {
        ValidateAppointment(appt);

        var existing = Appointments.FirstOrDefault(a => a.Id == appt.Id);
        if (existing == null)
        {
            Appointments.Add(appt);
        }
        else
        {
            var index = Appointments.IndexOf(existing);
            Appointments[index] = appt;
        }
    }

    public void DeleteAppointment(Appointment appt)
    {
        if (appt == null)
            return;

        var relatedDiagnoses = Diagnoses.Where(d => d.AppointmentId == appt.Id).ToList();
        foreach (var d in relatedDiagnoses)
            Diagnoses.Remove(d);

        var relatedTreatments = Treatments.Where(t => t.AppointmentId == appt.Id).ToList();
        foreach (var t in relatedTreatments)
            Treatments.Remove(t);

        Appointments.Remove(appt);
    }

    public void AddOrUpdateNote(MedicalNote note)
    {
        var existing = Notes.FirstOrDefault(n => n.Id == note.Id);
        if (existing == null)
        {
            Notes.Add(note);
        }
        else
        {
            var index = Notes.IndexOf(existing);
            Notes[index] = note;
        }
    }

    public void DeleteNote(MedicalNote note)
    {
        Notes.Remove(note);
    }

    public IEnumerable<Diagnosis> GetDiagnosesForAppointment(Guid appointmentId)
    {
        return Diagnoses.Where(d => d.AppointmentId == appointmentId).ToList();
    }

    public IEnumerable<Treatment> GetTreatmentsForAppointment(Guid appointmentId)
    {
        return Treatments.Where(t => t.AppointmentId == appointmentId).ToList();
    }

    public void AddOrUpdateDiagnosis(Diagnosis diagnosis)
    {
        if (diagnosis == null) return;

        var existing = Diagnoses.FirstOrDefault(d => d.Id == diagnosis.Id);
        if (existing == null)
        {
            Diagnoses.Add(diagnosis);
        }
        else
        {
            var index = Diagnoses.IndexOf(existing);
            Diagnoses[index] = diagnosis;
        }
    }

    public void DeleteDiagnosis(Diagnosis diagnosis)
    {
        if (diagnosis == null) return;
        Diagnoses.Remove(diagnosis);
    }

    public void AddOrUpdateTreatment(Treatment treatment)
    {
        if (treatment == null) return;

        var existing = Treatments.FirstOrDefault(t => t.Id == treatment.Id);
        if (existing == null)
        {
            Treatments.Add(treatment);
        }
        else
        {
            var index = Treatments.IndexOf(existing);
            Treatments[index] = treatment;
        }
    }

    public void DeleteTreatment(Treatment treatment)
    {
        if (treatment == null) return;
        Treatments.Remove(treatment);
    }

    private void ValidateAppointment(Appointment appt)
    {
        if (appt.StartTime >= appt.EndTime)
            throw new InvalidOperationException("Appointment end time must be after start time.");

        if (appt.StartTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            throw new InvalidOperationException("Appointments can only be scheduled Monday–Friday.");

        var start = appt.StartTime.TimeOfDay;
        var end = appt.EndTime.TimeOfDay;

        var open = new TimeSpan(8, 0, 0);
        var close = new TimeSpan(17, 0, 0);

        if (start < open || end > close)
            throw new InvalidOperationException("Appointments must be between 8am and 5pm.");

        var physicianConflict = Appointments.Any(existing =>
            existing.PhysicianId == appt.PhysicianId &&
            existing.Id != appt.Id &&
            existing.StartTime < appt.EndTime &&
            appt.StartTime < existing.EndTime);

        if (physicianConflict)
            throw new InvalidOperationException("This physician already has an appointment at that time.");

        var roomConflict = Appointments.Any(existing =>
            existing.Room == appt.Room &&
            existing.Id != appt.Id &&
            existing.StartTime < appt.EndTime &&
            appt.StartTime < existing.EndTime);

        if (roomConflict)
            throw new InvalidOperationException("This room is already booked during that time.");
    }
}
