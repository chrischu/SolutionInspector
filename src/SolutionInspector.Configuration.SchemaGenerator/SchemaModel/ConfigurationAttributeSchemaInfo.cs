using System;
using JetBrains.Annotations;

namespace SolutionInspector.SchemaGenerator.SchemaModel
{
  /// <summary>
  ///   Completion info for a rule-configuring XML attribute.
  /// </summary>
  public class ConfigurationAttributeSchemaInfo
  {
    public string Name { get; }
    public Type Type { get; }

    public bool IsRequired { get; }

    [CanBeNull]
    public string DefaultValue { get; }

    public ConfigurationAttributeSchemaInfo (string name, Type type, bool isRequired, [CanBeNull] string defaultValue)
    {
      Name = name;
      Type = type;
      IsRequired = isRequired;
      DefaultValue = defaultValue;
    }
  }
}