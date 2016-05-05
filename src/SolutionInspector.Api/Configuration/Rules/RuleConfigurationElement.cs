using System;
using System.Configuration;
using System.IO;
using System.Xml;
using SolutionInspector.Api.Configuration.Infrastructure;
using SolutionInspector.Api.Extensions;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.Api.Configuration.Rules
{
  /// <summary>
  /// Configuration for a <see cref="IRule"/>.
  /// </summary>
  public interface IRuleConfiguration
  {
    /// <summary>
    /// The assembly-qualified type name of the <see cref="IRule"/>.
    /// </summary>
    string RuleType { get; }

    /// <summary>
    /// The configuration for the <see cref="IRule"/> as XML.
    /// </summary>
    XmlElement Configuration { get; }
  }

  internal class RuleConfigurationElement : KeyedConfigurationElement<string>, IRuleConfiguration
  {
    [ConfigurationProperty("type", IsRequired = true)]
    public string RuleType => Key;

    public XmlElement Configuration { get; private set; }

    public override string KeyName => "type";

    protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
    {
      var subReader = reader.ReadSubtree();
      subReader.Read();

      var doc = new XmlDocument();
      var node = doc.ReadNode(subReader).AssertNotNull();

      using (var baseReader = CreateReaderForBaseCall(node))
      {
        base.DeserializeElement(baseReader, serializeCollectionKey);
      }

      Configuration = CreateConfigurationElement(node);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    private XmlReader CreateReaderForBaseCall(XmlNode node)
    {
      var nodeForBase = node.CloneNode(true);
      nodeForBase.RemoveAllChildren();
      nodeForBase.RemoveAttributesWhere(a => a.LocalName != KeyName);

      var reader = new XmlTextReader(new StringReader(nodeForBase.OuterXml));
      reader.Read();

      return reader;
    }

    private XmlElement CreateConfigurationElement(XmlNode node)
    {
      var nodeForConfiguration = node.CloneNode(true);
      nodeForConfiguration.RemoveAttributesWhere(a => a.LocalName == KeyName);
      return (XmlElement) nodeForConfiguration;
    }
  }
}