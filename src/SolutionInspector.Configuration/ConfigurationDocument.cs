using System.Xml.Linq;
using SolutionInspector.Commons.Extensions;

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
      var configurationDocument = Load<T>(xDocument.Root.AssertNotNull());
      configurationDocument._document = xDocument;
      configurationDocument._path = path;
      return configurationDocument;
    }

    /// <summary>
    ///   Save the <see cref="ConfigurationDocument" /> at the given <paramref name="path" /> or if no path is given, at the last path the document was saved
    ///   at.
    /// </summary>
    public void Save (string path = null)
    {
      _document.Save(path ?? _path);
    }
  }
}