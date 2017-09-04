using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;
using JetBrains.Annotations;
using SolutionInspector.ConfigurationUi.Features.Dialogs.EditGroupFilter;

namespace SolutionInspector.ConfigurationUi.Infrastructure.Converters
{
  [ValueConversion (typeof(string), typeof(string))]
  internal class FileExtensionToIconMonikerConverter : IValueConverter
  {
    public object Convert ([CanBeNull] object value, [NotNull] Type targetType, [CanBeNull] object parameter, [NotNull] CultureInfo culture)
    {
      if (value == null || parameter == null)
        return null;

      var extension = (string) value;
      var type = (TreeViewModelType) Enum.Parse(typeof(TreeViewModelType), (string) parameter);

      switch (type)
      {
        case TreeViewModelType.Project:
          return GetImageMonikerForProject(extension);
        case TreeViewModelType.Folder:
          return "Folder";
        case TreeViewModelType.File:
          return GetImageMonikerForFile(extension);
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private string GetImageMonikerForProject (string extension)
    {
      switch (extension)
      {
        case ".csproj":
          return "CSApplication";

        default:
          return "Application";
      }
    }

    private string GetImageMonikerForFile (string extension)
    {
      switch (extension)
      {
        default:
          return "Document";
      }
    }

    [ExcludeFromCodeCoverage]
    public object ConvertBack ([CanBeNull] object value, [NotNull] Type targetType, [CanBeNull] object parameter, [NotNull] CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}