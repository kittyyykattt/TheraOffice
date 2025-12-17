using System;
using System.Collections.ObjectModel;
using System.Linq;
using Maui.TheraOffice.Models;
using Maui.TheraOffice.Services;

namespace Maui.TheraOffice.Views;

[QueryProperty(nameof(Appointment), "Appointment")]
public partial class AppointmentDetailPage : ContentPage
{
    private bool _isNew = false;

    private Appointment _appointment = new();

    private readonly ObservableCollection<Diagnosis> _diagnosesForAppointment = new();
    private readonly ObservableCollection<Treatment> _treatmentsForAppointment = new();

    public Appointment Appointment
    {
        get => _appointment;
        set
        {
            _isNew = value == null;
            _appointment = value ?? new Appointment();

            BindingContext = _appointment;

            if (PatientPicker.ItemsSource == null)
                PatientPicker.ItemsSource = DataService.Current.Patients;

            if (PhysicianPicker.ItemsSource == null)
                PhysicianPicker.ItemsSource = DataService.Current.Physicians;

            if (!_isNew)
            {
                PatientPicker.SelectedItem = DataService.Current.Patients
                    .FirstOrDefault(p => p.Id == _appointment.PatientId);

                PhysicianPicker.SelectedItem = DataService.Current.Physicians
                    .FirstOrDefault(ph => ph.Id == _appointment.PhysicianId);

                if (_appointment.StartTime != default)
                {
                    DatePicker.Date      = _appointment.StartTime.Date;
                    StartTimePicker.Time = _appointment.StartTime.TimeOfDay;
                    EndTimePicker.Time   = _appointment.EndTime.TimeOfDay;
                }
            }
            else
            {
                if (_appointment.StartTime == default)
                {
                    var today = DateTime.Today;
                    DatePicker.Date      = today;
                    StartTimePicker.Time = new TimeSpan(9, 0, 0);
                    EndTimePicker.Time   = new TimeSpan(10, 0, 0);
                }
            }

            LoadRelatedData();
        }
    }

    public AppointmentDetailPage()
    {
        InitializeComponent();

        PatientPicker.ItemsSource   = DataService.Current.Patients;
        PhysicianPicker.ItemsSource = DataService.Current.Physicians;

        DiagnosesCollection.ItemsSource  = _diagnosesForAppointment;
        TreatmentsCollection.ItemsSource = _treatmentsForAppointment;
        UpdateTotalCostLabel();
    }

    private void LoadRelatedData()
    {
        _diagnosesForAppointment.Clear();
        _treatmentsForAppointment.Clear();

        if (_appointment == null)
        {
            UpdateTotalCostLabel();
            return;
        }

        var ds = DataService.Current;

        foreach (var d in ds.GetDiagnosesForAppointment(_appointment.Id))
            _diagnosesForAppointment.Add(d);

        foreach (var t in ds.GetTreatmentsForAppointment(_appointment.Id))
            _treatmentsForAppointment.Add(t);

        UpdateTotalCostLabel();
    }

    private void UpdateTotalCostLabel()
    {
        if (TotalCostLabel != null)
        {
            var total = _treatmentsForAppointment.Sum(t => t.Cost);
            TotalCostLabel.Text = $"Total Cost: {total:C}";
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        ErrorLabel.Text = string.Empty;

        if (PatientPicker.SelectedItem is not Patient patient ||
            PhysicianPicker.SelectedItem is not Physician physician)
        {
            ErrorLabel.Text = "Please select both a patient and physician.";
            ErrorLabel.IsVisible = true;
            return;
        }

        var date = DatePicker.Date;
        var start = StartTimePicker.Time;
        var end = EndTimePicker.Time;

        Appointment.PatientId = patient.Id;
        Appointment.PhysicianId = physician.Id;
        Appointment.StartTime = date.Date + start;
        Appointment.EndTime = date.Date + end;
        Appointment.Room       = RoomEntry.Text?.Trim();
    Appointment.Reason     = ReasonEntry.Text?.Trim();

        try
        {
            DataService.Current.AddOrUpdateAppointment(Appointment);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (!_isNew)
        {
            DataService.Current.DeleteAppointment(Appointment);
        }
        await Shell.Current.GoToAsync("..");
    }

    private async void OnAddDiagnosisClicked(object sender, EventArgs e)
    {
        if (_appointment == null)
        {
            await DisplayAlert("Error", "Please save the appointment before adding diagnoses.", "OK");
            return;
        }

        string text = await DisplayPromptAsync(
            "Diagnosis",
            "Enter diagnosis:",
            maxLength: 200);

        if (string.IsNullOrWhiteSpace(text))
            return;

        var diagnosis = new Diagnosis
        {
            AppointmentId = _appointment.Id,
            Text = text.Trim()
        };

        DataService.Current.AddOrUpdateDiagnosis(diagnosis);
        _diagnosesForAppointment.Add(diagnosis);
    }

    private async void OnDeleteDiagnosisClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is Diagnosis diagnosis)
        {
            bool confirm = await DisplayAlert(
                "Delete Diagnosis",
                $"Delete this diagnosis?\n\n{diagnosis.Text}",
                "Yes",
                "No");

            if (!confirm) return;

            DataService.Current.DeleteDiagnosis(diagnosis);
            _diagnosesForAppointment.Remove(diagnosis);
        }
    }

    private async void OnAddTreatmentClicked(object sender, EventArgs e)
    {
        if (_appointment == null)
        {
            await DisplayAlert("Error", "Please save the appointment before adding treatments.", "OK");
            return;
        }

        string description = await DisplayPromptAsync(
            "Treatment",
            "Enter treatment description:",
            maxLength: 200);

        if (string.IsNullOrWhiteSpace(description))
            return;

        string costStr = await DisplayPromptAsync(
            "Price",
            "Treatment Cost",
            keyboard: Keyboard.Numeric);

        if (string.IsNullOrWhiteSpace(costStr))
            return;

        if (!decimal.TryParse(costStr, out decimal cost) || cost < 0)
        {
            await DisplayAlert("Invalid Cost", "Please enter a valid non-negative number.", "OK");
            return;
        }

        var treatment = new Treatment
        {
            AppointmentId = _appointment.Id,
            Description   = description.Trim(),
            Cost          = cost
        };

        DataService.Current.AddOrUpdateTreatment(treatment);
        _treatmentsForAppointment.Add(treatment);
        UpdateTotalCostLabel();
    }

    private async void OnDeleteTreatmentClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is Treatment treatment)
        {
            bool confirm = await DisplayAlert(
                "Delete Treatment",
                $"Delete treatment \"{treatment.Description}\"?",
                "Yes",
                "No");

            if (!confirm) return;

            DataService.Current.DeleteTreatment(treatment);
            _treatmentsForAppointment.Remove(treatment);
            UpdateTotalCostLabel();
        }
    }

}
