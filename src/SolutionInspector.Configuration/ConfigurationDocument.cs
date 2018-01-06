using System;
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

    internal static T Create<T> (XName elementName)
        where T : ConfigurationDocument, new()
    {
      var document = new XDocument(new XElement(elementName));
      var configurationDocument = Load<T>(document.Root.AssertNotNull());
      configurationDocument._document = document;
      return configurationDocument;
    }

    internal static T Load<T> (string path, XDocument xDocument) where T : ConfigurationDocument, new()
    {
      var configurationDocument = Load<T>(xDocument.Root.AssertNotNull());
      configurationDocument._document = xDocument;
      configurationDocument._path = path;
      return configurationDocument;
    }

    protected ConfigurationDocument(string rootElementName)
    {
      RootElementName = rootElementName;
    }

    public string RootElementName { get; }

    /// <summary>
    ///   Save the <see cref="ConfigurationDocument" /> at the given <paramref name="path" /> or if no path is given, at the last path the document was saved
    ///   at.
    /// </summary>
    public void Save (string path = null)
    {
      path = path ?? _path;

      if(path == null)
      {
        throw new ArgumentException(
            "The configuration document was not loaded from a path and therefore it is necessary to specify the save path explicitly.",
            nameof(path));
      }

      _document.Save(path);
    }
  }
}