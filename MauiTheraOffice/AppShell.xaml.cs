using Maui.TheraOffice.Views;

namespace Maui.TheraOffice;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(PatientDetailPage), typeof(PatientDetailPage));
        Routing.RegisterRoute(nameof(PhysicianDetailPage), typeof(PhysicianDetailPage));
        Routing.RegisterRoute(nameof(AppointmentDetailPage), typeof(AppointmentDetailPage));
    }
}
