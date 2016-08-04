using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace SolutionInspector.ConfigurationUi.Converters
{
  [ValueConversion (typeof(string), typeof(string))]
  internal class RatioConverter : MarkupExtension, IValueConverter
  {
    private static RatioConverter s_instance;

    public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
    {
      var size = System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
      return size.ToString("G0", CultureInfo.InvariantCulture);
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public override object ProvideValue (IServiceProvider serviceProvider)
    {
      return s_instance ?? (s_instance = new RatioConverter());
    }
  }
}