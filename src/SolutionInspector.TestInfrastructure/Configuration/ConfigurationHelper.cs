using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Fasterflect;

namespace SolutionInspector.TestInfrastructure.Configuration
{
  /// <summary>
  ///   Utility class designed to deserialize xml-fragments into configuration elements.
  /// </summary>
  [ExcludeFromCodeCoverage]
  public static class ConfigurationHelper
  {
    public static void DeserializeElement (ConfigurationElement configurationElement, string xmlFragment)
    {
      using (var reader = new XmlTextReader(xmlFragment, XmlNodeType.Document, null))
      {
        reader.WhitespaceHandling = WhitespaceHandling.None;
        reader.IsStartElement();
        configurationElement.CallMethod("DeserializeElement", reader, false);
      }
    }

    public static void DeserializeSection (ConfigurationSection configurationSection, string xmlFragment)
    {
      using (var reader = new XmlTextReader(xmlFragment, XmlNodeType.Document, null))
      {
        reader.WhitespaceHandling = WhitespaceHandling.None;
        configurationSection.CallMethod("DeserializeSection", (XmlReader) reader);
      }
    }
  }
}