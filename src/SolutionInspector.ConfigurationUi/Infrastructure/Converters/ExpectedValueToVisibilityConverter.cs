using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using JetBrains.Annotations;

namespace SolutionInspector.ConfigurationUi.Infrastructure.Converters
{
  [ValueConversion (typeof(object), typeof(Visibility))]
  internal class ExpectedValueToVisibilityConverter : IValueConverter
  {
    public object Convert ([CanBeNull] object value, Type targetType, [CanBeNull] object parameter, CultureInfo culture)
    {
      if (value == null && parameter == null)
        return Visibility.Visible;

      if (value == null || parameter == null)
        return Visibility.Collapsed;

      var valueType = value.GetType();
      if (valueType.IsEnum)
        parameter = Enum.Parse(valueType, parameter.ToString());

      return Equals(value, parameter) ? Visibility.Visible : Visibility.Collapsed;
    }

    [ExcludeFromCodeCoverage]
    public object ConvertBack ([CanBeNull] object value, Type targetType, [CanBeNull] object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}