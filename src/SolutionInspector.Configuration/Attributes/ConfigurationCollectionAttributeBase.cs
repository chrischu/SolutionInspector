using System;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration.Attributes
{
  /// <summary>
  ///   Base class for configuration collection attributes.
  /// </summary>
  public abstract class ConfigurationCollectionAttributeBase : ConfigurationPropertyAttribute
  {
    /// <summary>
    ///   The name of the XML element representing the collection.
    /// </summary>
    [CanBeNull]
    public string CollectionName { get; set; }

    /// <summary>
    ///   The minimum number of collection items necessary for the configuration to be valid.
    /// </summary>
    public int MinimumElementCount { get; set; }

    /// <summary>
    ///   The maximum number of collection items allowed for the configuration to be valid.
    /// </summary>
    public int MaximumElementCount { get; set; } = int.MaxValue;

    [CanBeNull]
    internal override string XmlName => CollectionName;
  }
}