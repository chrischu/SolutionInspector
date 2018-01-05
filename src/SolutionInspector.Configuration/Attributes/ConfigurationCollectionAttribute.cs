using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration.Attributes
{
  /// <summary>
  ///   Marks a property as a configuration collection.
  /// </summary>
  public class ConfigurationCollectionAttribute : ConfigurationCollectionAttributeBase
  {
    private string _elementName = DefaultElementName;

    public const string DefaultElementName = "item";

    /// <summary>
    ///   Controls whether the collection is the default collection (i.e. there does not have to be a collection element).
    /// </summary>
    public bool IsDefaultCollection { get; set; }

    /// <summary>
    ///   The name of the XML elements representing the collection items.
    /// </summary>
    [NotNull]
    public string ElementName
    {
      get => _elementName ?? DefaultElementName;
      set => _elementName = value;
    }

    [CanBeNull]
    internal override string XmlName => IsDefaultCollection ? null : CollectionName;

    [CanBeNull]
    public Type GetCollectionElementType (PropertyInfo property)
    {
      var genericArguments = property.PropertyType.GetGenericArguments();

      return genericArguments.Any() ? genericArguments.Single() : null;
    }
  }
}