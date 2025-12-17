using Maui.TheraOffice.Models;
using Maui.TheraOffice.Services;

namespace Maui.TheraOffice.Views;

[QueryProperty(nameof(Physician), "Physician")]
public partial class PhysicianDetailPage : ContentPage
{
    private bool _isNew = false;

    private Physician _physician = new();
    public Physician Physician
    {
        get => _physician;
        set
        {
            _isNew = value == null;

            _physician = value ?? new Physician();

            BindingContext = _physician;
        }
    }

    public PhysicianDetailPage()
    {
        InitializeComponent();

        Physician = null;    
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        DataService.Current.AddOrUpdatePhysician(Physician);
        await Shell.Current.GoToAsync("..");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_isNew)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        try
        {
            DataService.Current.DeletePhysician(Physician);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Cannot delete physician", ex.Message, "OK");
        }
    }
}
