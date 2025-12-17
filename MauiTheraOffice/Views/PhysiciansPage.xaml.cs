using Maui.TheraOffice.Models;
using Maui.TheraOffice.Services;

namespace Maui.TheraOffice.Views;

public partial class PhysiciansPage : ContentPage
{
    public PhysiciansPage()
    {
        InitializeComponent();
        BindingContext = DataService.Current.Physicians;
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(PhysicianDetailPage));
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Physician selected)
        {
            var navParams = new Dictionary<string, object>
            {
                { "Physician", selected }
            };

            await Shell.Current.GoToAsync(nameof(PhysicianDetailPage), navParams);
            ((CollectionView)sender).SelectedItem = null;
        }
    }
    private async void OnInlineEditClicked(object sender, EventArgs e)
{
    if (sender is Button btn && btn.CommandParameter is Physician physician)
    {
        var navParams = new Dictionary<string, object>
        {
            { "Physician", physician }
        };

        await Shell.Current.GoToAsync(nameof(PhysicianDetailPage), navParams);
    }
}

private async void OnInlineDeleteClicked(object sender, EventArgs e)
{
    if (sender is Button btn && btn.CommandParameter is Physician physician)
    {
        bool confirm = await DisplayAlert(
            "Delete Physician",
            $"Delete {physician.FullName}?",
            "Yes",
            "No");

        if (!confirm)
            return;

        try
        {
            DataService.Current.DeletePhysician(physician);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Cannot delete physician", ex.Message, "OK");
        }
    }
}

}
