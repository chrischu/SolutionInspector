using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolutionInspector.BuildTool.SchemaModel;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration;
using SolutionInspector.Configuration.Attributes;

namespace SolutionInspector.BuildTool
{
  /// <summary>
  ///   Retrieves schema info from a <see cref="Type" /> representing a <see cref="ConfigurationElement" />.
  /// </summary>
  public interface ISchemaInfoRetriever
  {
    ConfigurationElementSchemaInfo GetSchemaInfo (Type configurationElementType);
  }

  internal class SchemaInfoRetriever : ISchemaInfoRetriever
  {
    public ConfigurationElementSchemaInfo GetSchemaInfo (Type configurationElementType)
    {
      var elementName = configurationElementType.Name.ToFirstCharLower();
      var possibleEntities = GetPossibleEntities(configurationElementType);
      return new ConfigurationElementSchemaInfo(elementName, 0, int.MaxValue, possibleEntities.Attributes, possibleEntities.Elements);
    }

    private (IReadOnlyList<ConfigurationAttributeSchemaInfo> Attributes, IReadOnlyList<ConfigurationElementSchemaInfo> Elements)
        GetPossibleEntities (Type configurationType)
    {
      var configurationProperties = configurationType.GetProperties()
          .Select(p => new { Property = p, Attribute = p.GetCustomAttribute<ConfigurationPropertyAttribute>() })
          .Where(p => p.Attribute != null)
          .ToList();

      var configurationValues = configurationProperties.Where(x => x.Attribute is ConfigurationValueAttribute);
      var possibleAttributes = configurationValues.Select(
          x =>
          {
            var attribute = (ConfigurationValueAttribute) x.Attribute;
            var name = attribute.GetXmlName(x.Property.Name);

            return new ConfigurationAttributeSchemaInfo(name, x.Property.PropertyType, attribute.IsRequired, attribute.DefaultValue);
          });

      var configurationSubelements = configurationProperties.Where(x => x.Attribute is ConfigurationSubelementAttribute);
      var possibleSubelementsFromSubelements = configurationSubelements.Select(
          x =>
          {
            var attribute = (ConfigurationSubelementAttribute) x.Attribute;

            var possibleEntities = GetPossibleEntities(x.Property.PropertyType);
            return new ConfigurationElementSchemaInfo(
                attribute.GetXmlName(x.Property.Name),
                attribute.IsRequired ? 1 : 0,
                1,
                possibleEntities.Attributes,
                possibleEntities.Elements);

          });

      var configurationCollections = configurationProperties.Where(x => x.Attribute is ConfigurationCollectionAttribute);
      var possibleSubelementsFromCollections = configurationCollections.Select(
          x =>
          {
            var attribute = (ConfigurationCollectionAttribute) x.Attribute;
            var elementType = attribute.GetCollectionElementType(x.Property).AssertNotNull();
            var possibleEntities = GetPossibleEntities(elementType);

            if (attribute.IsDefaultCollection)
            {
              return new ConfigurationElementSchemaInfo(
                  attribute.ElementName,
                  attribute.MinimumElementCount,
                  attribute.MaximumElementCount,
                  possibleEntities.Attributes,
                  possibleEntities.Elements);
            }
            else
            {
              return new ConfigurationElementSchemaInfo(
                  attribute.GetXmlName(x.Property.Name),
                  attribute.IsRequired ? 1 : 0,
                  1,
                  new ConfigurationAttributeSchemaInfo[0],
                  new[]
                  {
                      new ConfigurationElementSchemaInfo(
                          attribute.ElementName,
                          attribute.MinimumElementCount,
                          attribute.MaximumElementCount,
                          possibleEntities.Attributes,
                          possibleEntities.Elements)
                  });
            }
          });

      return (possibleAttributes.ToList(), possibleSubelementsFromSubelements.Concat(possibleSubelementsFromCollections).ToList());
    }
  }
}