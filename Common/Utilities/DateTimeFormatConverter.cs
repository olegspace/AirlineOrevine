using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Common.Utilities;

public class DateTimeFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime date)
        {
            return date.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string dateString)
        {
            if (DateTime.TryParseExact(dateString, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime resultDate))
            {
                return resultDate;
            }
            else
            {
                Console.WriteLine($"Failed to parse date: {dateString}");
            }
        }
        else
        {
            Console.WriteLine($"Invalid value type: {value.GetType()}");
        }
        return DependencyProperty.UnsetValue; // Return UnsetValue for invalid conversion
    }
}