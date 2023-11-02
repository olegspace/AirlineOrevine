using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Common.Utilities;

public class DateOnlyFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateOnly date)
        {
            return date.ToString("dd.MM.yyyy");
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string dateString)
        {
            if (DateOnly.TryParseExact(dateString, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AllowInnerWhite, out DateOnly resultDate))
            {
                return resultDate;
            }
        }
        return DependencyProperty.UnsetValue;
    }
}