using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration
{
  public class ConfigurationCollectionAttribute : ConfigurationPropertyAttribute
  {
    [CanBeNull]
    public string CollectionName { get; set; }

    public bool IsDefaultCollection { get; set; }

    public int MinimumElementCount { get; set; }
    public int MaximumElementCount { get; set; } = int.MaxValue;

    public string ElementName { get; set; } = "item";

    internal override string XmlName => CollectionName;

    [CanBeNull]
    public Type GetCollectionElementType (PropertyInfo property)
    {
      var genericArguments = property.PropertyType.GetGenericArguments();

      return genericArguments.Any() ? genericArguments.Single() : null;
    }
  }
}