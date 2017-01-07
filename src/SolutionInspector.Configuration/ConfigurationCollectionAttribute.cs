using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration
{
  /// <summary>
  ///   Marks a property as a configuration collection.
  /// </summary>
  public class ConfigurationCollectionAttribute : ConfigurationPropertyAttribute
  {
    /// <summary>
    ///   The name of the XML element representing the collection.
    /// </summary>
    [CanBeNull]
    public string CollectionName { get; set; }

    /// <summary>
    ///   Controls whether the collection is the default collection (i.e. there does not have to be a collection element).
    /// </summary>
    public bool IsDefaultCollection { get; set; }

    /// <summary>
    ///   The minimum number of collection items necessary for the configuration to be valid.
    /// </summary>
    public int MinimumElementCount { get; set; }

    /// <summary>
    ///   The maximum number of collection items allowed for the configuration to be valid.
    /// </summary>
    public int MaximumElementCount { get; set; } = int.MaxValue;

    /// <summary>
    ///   The name of the XML elements representing the collection items.
    /// </summary>
    public string ElementName { get; set; } = "item";

    internal override string XmlName => CollectionName;

    [CanBeNull]
    internal Type GetCollectionElementType (PropertyInfo property)
    {
      var genericArguments = property.PropertyType.GetGenericArguments();

      return genericArguments.Any() ? genericArguments.Single() : null;
    }
  }
}