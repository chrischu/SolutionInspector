using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace SolutionInspector.ConfigurationUi.Converters
{
  [ValueConversion (typeof(object), typeof(Visibility))]
  internal class ExpectedValueToVisibilityConverter : MarkupExtension, IValueConverter
  {
    private static ExpectedValueToVisibilityConverter s_instance;

    public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Equals(value, parameter) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public override object ProvideValue (IServiceProvider serviceProvider)
    {
      return s_instance ?? (s_instance = new ExpectedValueToVisibilityConverter());
    }
  }
}