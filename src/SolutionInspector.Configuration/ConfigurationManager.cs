using Wrapperator.Interfaces.Xml.Linq;

namespace SolutionInspector.Configuration
{
  /// <summary>
  ///   Provides methods to load <see cref="ConfigurationDocument" />s.
  /// </summary>
  public interface IConfigurationManager
  {
    /// <summary>
    ///   Loads and validates the <see cref="ConfigurationDocument" /> of type <typeparamref name="T" /> from the given
    ///   <paramref name="configurationFilePath" />.
    /// </summary>
    T LoadDocument<T> (string configurationFilePath)
      where T : ConfigurationDocument, new();
  }

  /// <summary>
  ///   Default implementation of <see cref="IConfigurationManager" />.
  /// </summary>
  public class ConfigurationManager : IConfigurationManager
  {
    private readonly IXDocumentStatic _xDocumentStatic;

    /// <summary>
    ///   Creates a new <see cref="ConfigurationManager" />.
    /// </summary>
    public ConfigurationManager (IXDocumentStatic xDocumentStatic)
    {
      _xDocumentStatic = xDocumentStatic;
    }

    public T LoadDocument<T> (string configurationFilePath)
      where T : ConfigurationDocument, new()
    {
      var xDocument = _xDocumentStatic.Load(configurationFilePath);

      return ConfigurationDocument.Load<T>(configurationFilePath, xDocument._XDocument);
    }
  }
}