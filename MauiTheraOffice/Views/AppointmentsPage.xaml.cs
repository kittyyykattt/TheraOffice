using System;
using System.Collections.Generic;
using System.Linq;
using Maui.TheraOffice.Models;
using Maui.TheraOffice.Services;
using Microsoft.Maui.Controls;

namespace Maui.TheraOffice.Views;

public partial class AppointmentsPage : ContentPage
{
    public AppointmentsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshAppointments();
    }

    private void RefreshAppointments()
    {
        var list = DataService.Current.Appointments.ToList();

        int sortIndex = SortPicker?.SelectedIndex ?? 0;
        bool desc = DescendingSwitch?.IsToggled ?? false;

        IEnumerable<Appointment> ordered = list;

        switch (sortIndex)
        {
            case 1: 
                ordered = desc
                    ? list.OrderByDescending(a => a.Reason)
                    : list.OrderBy(a => a.Reason);
                break;

            default: 
                ordered = desc
                    ? list.OrderByDescending(a => a.StartTime)
                    : list.OrderBy(a => a.StartTime);
                break;
        }

        AppointmentsCollection.ItemsSource = ordered.ToList();
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        var appt = new Appointment
        {
            StartTime = DateTime.Today.AddHours(9),
            EndTime   = DateTime.Today.AddHours(10)
        };

        var navParams = new Dictionary<string, object>
        {
            { "Appointment", appt }
        };

        await Shell.Current.GoToAsync(nameof(AppointmentDetailPage), navParams);
    }

    private void OnSortChanged(object sender, EventArgs e)
    {
        RefreshAppointments();
    }

    private async void OnInlineEditClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Appointment appt)
        {
            var navParams = new Dictionary<string, object>
            {
                { "Appointment", appt }
            };

            await Shell.Current.GoToAsync(nameof(AppointmentDetailPage), navParams);
        }
    }

    private async void OnInlineDeleteClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Appointment appt)
        {
            bool confirm = await DisplayAlert(
                "Delete Appointment",
                $"Delete appointment \"{appt.Reason}\"?",
                "Yes",
                "No");

            if (!confirm)
                return;

            DataService.Current.DeleteAppointment(appt);
            RefreshAppointments();
        }
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Appointment selected)
        {
            var navParams = new Dictionary<string, object>
            {
                { "Appointment", selected }
            };

            await Shell.Current.GoToAsync(nameof(AppointmentDetailPage), navParams);
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}
