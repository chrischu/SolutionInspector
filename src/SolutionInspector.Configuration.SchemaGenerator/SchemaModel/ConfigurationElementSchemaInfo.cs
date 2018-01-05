using System;
using System.Collections.Generic;

namespace SolutionInspector.SchemaGenerator.SchemaModel
{
  /// <summary>
  ///   Completion info for a rule-configuring XML element.
  /// </summary>
  public class ConfigurationElementSchemaInfo
  {
    public string Name { get; }
    public int MinOccurs { get; }
    public int MaxOccurs { get; }

    public IReadOnlyCollection<ConfigurationAttributeSchemaInfo> PossibleAttributes { get; }
    public IReadOnlyCollection<ConfigurationElementSchemaInfo> PossibleSubelements { get; }

    public ConfigurationElementSchemaInfo (
        string name,
        int minOccurs,
        int maxOccurs,
        IReadOnlyCollection<ConfigurationAttributeSchemaInfo> possibleAttributes,
        IReadOnlyCollection<ConfigurationElementSchemaInfo> possibleSubelements)
    {
      Name = name;
      MinOccurs = minOccurs;
      MaxOccurs = maxOccurs;
      PossibleAttributes = possibleAttributes;
      PossibleSubelements = possibleSubelements;
    }
  }
}