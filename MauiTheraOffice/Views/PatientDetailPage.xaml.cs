using System;
using Maui.TheraOffice.Models;
using Maui.TheraOffice.Services;

namespace Maui.TheraOffice.Views;

public partial class PatientDetailPage : ContentPage
{
    public PatientDetailPage(Patient patient)
    {
        InitializeComponent();
        BindingContext = patient;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (BindingContext is not Patient patient)
            return;

        DataService.Current.AddOrUpdatePatient(patient);

        await DisplayAlert("Saved", "Patient information saved.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (BindingContext is not Patient patient)
            return;

        if (patient.Id == Guid.Empty)
        {
            await Navigation.PopAsync();
            return;
        }

        var confirm = await DisplayAlert(
            "Delete",
            "Are you sure you want to delete this patient?",
            "Yes", "No");

        if (!confirm)
            return;

        DataService.Current.DeletePatient(patient);

        await DisplayAlert("Deleted", "Patient deleted.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
