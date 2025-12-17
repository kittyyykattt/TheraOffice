using System;
using Maui.TheraOffice.Models;
using Maui.TheraOffice.Services;

namespace Maui.TheraOffice.Views;

public partial class PatientsPage : ContentPage
{
    public PatientsPage()
    {
        InitializeComponent();
        BindingContext = DataService.Current;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await DataService.Current.LoadPatientsFromApiAsync();
    }

    private async void OnSearchButtonPressed(object sender, EventArgs e)
    {
        var text = PatientsSearchBar.Text ?? string.Empty;
        await DataService.Current.SearchPatientsAsync(text);
    }

    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            await DataService.Current.LoadPatientsFromApiAsync();
        }
    }

    private async void OnAddPatientClicked(object sender, EventArgs e)
    {
        var newPatient = new Patient(); 
        await Navigation.PushAsync(new PatientDetailPage(newPatient));
    }

    private async void OnPatientSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is not Patient patient)
            return;

        PatientsListView.SelectedItem = null;

        await Navigation.PushAsync(new PatientDetailPage(patient));
    }

private async void OnInlineEditClicked(object sender, EventArgs e)
{
    if (sender is Button btn && btn.CommandParameter is Patient patient)
    {
        await Navigation.PushAsync(new PatientDetailPage(patient));
    }
}

private async void OnInlineDeleteClicked(object sender, EventArgs e)
{
    if (sender is Button btn && btn.CommandParameter is Patient patient)
    {
        bool confirm = await DisplayAlert(
            "Delete Patient",
            $"Delete {patient.FullName}?",
            "Yes",
            "No");

        if (!confirm)
            return;

        DataService.Current.DeletePatient(patient);
    }
}

}

