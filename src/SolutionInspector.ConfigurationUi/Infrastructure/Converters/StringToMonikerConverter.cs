extern alias imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Imaging;

namespace SolutionInspector.ConfigurationUi.Infrastructure.Converters
{
  [ValueConversion (typeof(string), typeof(object))]
  internal class StringToMonikerConverter : IValueConverter
  {
    private readonly Lazy<Dictionary<string, object>> _imageMonikers;

    public StringToMonikerConverter ()
    {
      _imageMonikers = new Lazy<Dictionary<string, object>>(
        () =>
          typeof(KnownMonikers).GetProperties(BindingFlags.Public | BindingFlags.Static)
              .ToDictionary(p => p.Name, p => p.GetValue(null)));
    }

    public object Convert ([CanBeNull] object value, Type targetType, [CanBeNull] object parameter, CultureInfo culture)
    {
      if (value == null)
        return null;

      var monikerName = (string) value;

      return _imageMonikers.Value[monikerName];
    }

    [ExcludeFromCodeCoverage]
    public object ConvertBack ([CanBeNull] object value, Type targetType, [CanBeNull] object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}