using JetBrains.Annotations;

namespace SolutionInspector.Configuration
{
  public interface IConfigurationConverter
  {
  }

  public interface IConfigurationConverter<T> : IConfigurationConverter
  {
    string ConvertTo ([CanBeNull] T value);
    T ConvertFrom ([CanBeNull] string value);
  }
}