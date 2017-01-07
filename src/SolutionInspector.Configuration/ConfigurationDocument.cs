using System.Xml.Linq;

namespace SolutionInspector.Configuration
{
  /// <summary>
  ///   Base class for configuration documents (configuration classes that map to whole files).
  /// </summary>
  public abstract class ConfigurationDocument : ConfigurationBase
  {
    private XDocument _document;
    private string _path;

    internal static T Load<T> (string path, XDocument xDocument) where T : ConfigurationDocument, new()
    {
      var configurationDocument = Load<T>(xDocument.Root);
      configurationDocument._document = xDocument;
      configurationDocument._path = path;
      return configurationDocument;
    }

    internal void Save (string path = null)
    {
      _document.Save(path ?? _path);
    }
  }
}