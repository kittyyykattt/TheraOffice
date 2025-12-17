using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Maui.TheraOffice.Converters;

public class TodayAppointmentColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime start)
        {
            if (start.Date == DateTime.Today)
            {
                return Colors.LightGreen;
            }
        }

        return Colors.Transparent;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
