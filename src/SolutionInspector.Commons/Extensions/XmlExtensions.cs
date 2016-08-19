using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml;

namespace SolutionInspector.Commons.Extensions
{
  /// <summary>
  ///   Extension methods for XML classes.
  /// </summary>
  public static class XmlExtensions
  {
    /// <summary>
    ///   Adds an attribute with the given <paramref name="name" /> and <paramref name="value" /> to the <paramref name="element" />.
    /// </summary>
    public static void AddAttribute (this XmlElement element, string name, string value)
    {
      var attr = element.OwnerDocument.AssertNotNull().CreateAttribute(name);
      attr.Value = value;
      element.Attributes.Append(attr);
    }

    /// <summary>
    ///   Adds an element with the given <paramref name="name" /> and <paramref name="value" /> to the <paramref name="element" />.
    /// </summary>
    public static void AddElement (this XmlElement element, string name, string value)
    {
      var elem = element.OwnerDocument.AssertNotNull().CreateElement(name);
      elem.InnerText = value;
      element.AppendChild(elem);
    }

    /// <summary>
    ///   Removes all children from the <paramref name="xmlNode" />.
    /// </summary>
    public static void RemoveAllChildren (this XmlNode xmlNode)
    {
      var child = xmlNode.FirstChild;

      while (child != null)
      {
        var sibling = child.NextSibling;
        xmlNode.RemoveChild(child);
        child = sibling;
      }
    }

    /// <summary>
    ///   Removes all attributes that match the <paramref name="filter" /> from the <paramref name="xmlNode" />.
    /// </summary>
    public static void RemoveAttributesWhere (this XmlNode xmlNode, Func<XmlAttribute, bool> filter)
    {
      if (xmlNode.Attributes == null || xmlNode.Attributes.Count == 0)
        return;

      var toRemove = xmlNode.Attributes.Cast<XmlAttribute>().Where(filter).ToArray();

      foreach (var attribute in toRemove)
        xmlNode.Attributes.Remove(attribute);
    }

    /// <summary>
    ///   Creates an instance of <see cref="XmlReader" /> from the given <paramref name="xmlNode" />.
    /// </summary>
    [SuppressMessage ("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    public static XmlReader Read (this XmlNode xmlNode)
    {
      var reader = new XmlTextReader(new StringReader(xmlNode.OuterXml));
      reader.Read();
      return reader;
    }
  }
}