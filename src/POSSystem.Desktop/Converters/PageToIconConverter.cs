using System;
using System.Globalization;
using System.Windows.Data;

namespace POSSystem.Desktop.Converters;

public class PageToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            "dashboard" => "\uE80F",
            "pos" => "\uE7BF",
            "products" => "\uE7F8",
            "reports" => "\uE9F5",
            "permissions" => "\uE72E",
            _ => "\uE8F1"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
