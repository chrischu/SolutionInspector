using Wrapperator.Interfaces.Xml.Linq;

namespace SolutionInspector.Configuration
{
  public interface IConfigurationManager
  {
    T LoadDocument<T>(string configurationFilePath)
      where T : ConfigurationDocument, new();
  }

  public class ConfigurationManager : IConfigurationManager
  {
    private readonly IXDocumentStatic _xDocumentStatic;

    public ConfigurationManager (IXDocumentStatic xDocumentStatic)
    {
      _xDocumentStatic = xDocumentStatic;
    }

    public T LoadDocument<T>(string configurationFilePath)
      where T : ConfigurationDocument, new()
    {
      var xDocument = _xDocumentStatic.Load(configurationFilePath);

      return ConfigurationDocument.Load<T>(configurationFilePath, xDocument._XDocument);
    }
  }
}