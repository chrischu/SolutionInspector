using System.Xml.Linq;

namespace SolutionInspector.Configuration
{
  public abstract class ConfigurationDocument : ConfigurationBase
  {
    private XDocument _document;
    private string _path;

    //public static T Load<T>(string path) where T : ConfigurationDocument, new()
    //{
    //  var document = XDocument.Load(path);

    //  var configurationDocument = Load<T>(document.Root);
    //  configurationDocument._document = document;
    //  configurationDocument._path = path;
    //  return configurationDocument;
    //}

    public static T Load<T>(string path, XDocument xDocument) where T : ConfigurationDocument, new()
    {
      var configurationDocument = Load<T>(xDocument.Root);
      configurationDocument._document = xDocument;
      configurationDocument._path = path;
      return configurationDocument;
    }

    public void Save(string path = null)
    {
      _document.Save(path ?? _path);
    }
  }
}