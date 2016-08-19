using System;
using System.Xml.Linq;

namespace SolutionInspector.Configuration
{
  public interface IConfigurationManager
  {
    T LoadSection<T>(string configurationFilePath)
      where T : ConfigurationDocument;

    T LoadSection<T>(XDocument xDocument)
      where T : ConfigurationDocument;
  }

  public class ConfigurationManager : IConfigurationManager
  {
    public T LoadSection<T>(string configurationFilePath)
      where T : ConfigurationDocument
    {
      return LoadSection<T>(XDocument.Load(configurationFilePath));
    }

    public T LoadSection<T>(XDocument xDocument)
      where T : ConfigurationDocument
    {
      return (T)Activator.CreateInstance(typeof(T), xDocument);
    }
  }
}