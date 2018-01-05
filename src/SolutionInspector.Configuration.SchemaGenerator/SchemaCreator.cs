using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration;
using SolutionInspector.SchemaGenerator.SchemaModel;

namespace SolutionInspector.SchemaGenerator
{
  /// <summary>
  ///   Creates a <see cref="XmlSchema" /> from a <see cref="Type" /> representing a <see cref="ConfigurationDocument" />.
  /// </summary>
  public class SchemaCreator
  {
    private readonly ISchemaInfoRetriever _schemaInfoRetriever;

    public SchemaCreator (ISchemaInfoRetriever schemaInfoRetriever)
    {
      _schemaInfoRetriever = schemaInfoRetriever;
    }

    public XmlSchema CreateSchema (Type configurationDocumentType)
    {
      var schema = new XmlSchema { TargetNamespace = configurationDocumentType.Assembly.GetName().Name };
      schema.ElementFormDefault = XmlSchemaForm.Qualified;
      schema.Namespaces.Add("", schema.TargetNamespace);

      var schemaInfo = _schemaInfoRetriever.GetSchemaInfo(configurationDocumentType);

      ProcessDocument(schemaInfo, schema);

      return schema;
    }

    private void ProcessDocument (ConfigurationDocumentSchemaInfo documentSchemaInfo, XmlSchema schema)
    {
      ProcessElement(documentSchemaInfo.RootElement, schema, isRoot: true);
    }

    private XmlSchemaElement CreateElement (ConfigurationElementSchemaInfo elementSchemaInfo, bool isRoot)
    {
      var element = new XmlSchemaElement();
      element.Name = elementSchemaInfo.Name;

      if (!isRoot)
      {
        element.MinOccurs = elementSchemaInfo.MinOccurs;
        if (elementSchemaInfo.MaxOccurs == int.MaxValue)
          element.MaxOccursString = "unbounded";
        else
          element.MaxOccurs = elementSchemaInfo.MaxOccurs;
      }

      return element;
    }

    private XmlSchemaComplexType ProcessElement (ConfigurationElementSchemaInfo elementSchemaInfo, XmlSchema schema, bool isRoot)
    {
      var element = CreateElement(elementSchemaInfo, isRoot);

      var type = new XmlSchemaComplexType();
      type.Name = elementSchemaInfo.Name.ToFirstCharUpper();

      element.SchemaTypeName = new XmlQualifiedName(type.Name, schema.TargetNamespace);

      if (isRoot)
        schema.Items.Add(element);

      schema.Items.Add(type);

      foreach (var attributeSchemaInfo in elementSchemaInfo.PossibleAttributes)
      {
        var attribute = new XmlSchemaAttribute();

        attribute.Name = attributeSchemaInfo.Name;
        attribute.SchemaTypeName = MapType(attributeSchemaInfo.Type);
        attribute.Use = attributeSchemaInfo.IsRequired ? XmlSchemaUse.Required : XmlSchemaUse.Optional;
        attribute.DefaultValue = attributeSchemaInfo.DefaultValue;

        type.Attributes.Add(attribute);
      }

      if (elementSchemaInfo.PossibleSubelements.Any())
      {
        var sequence = new XmlSchemaSequence();

        foreach (var subElementSchemaInfo in elementSchemaInfo.PossibleSubelements)
        {
          var subElementType = ProcessElement(subElementSchemaInfo, schema, isRoot: false);

          var subElement = CreateElement(subElementSchemaInfo, isRoot: false);
          subElement.SchemaTypeName = new XmlQualifiedName(subElementType.Name, schema.TargetNamespace);

          sequence.Items.Add(subElement);
        }

        type.Particle = sequence;
      }

      return type;
    }

    private static readonly Dictionary<Type, string> s_typeMap = new Dictionary<Type, string>
                                                                 {
                                                                     { typeof(bool), "boolean" },
                                                                     { typeof(int), "int" },
                                                                     { typeof(short), "short" },
                                                                     { typeof(long), "long" },
                                                                     { typeof(float), "long" },
                                                                     { typeof(double), "double" },
                                                                     { typeof(decimal), "decimal" },
                                                                     { typeof(DateTime), "dateTime" },
                                                                     { typeof(TimeSpan), "duration" },
                                                                     { typeof(Uri), "anyUri" }
                                                                 };

    private XmlQualifiedName MapType (Type type)
    {
      const string xs = "http://www.w3.org/2001/XMLSchema";
      if (s_typeMap.TryGetValue(type, out string name))
        return new XmlQualifiedName(name, xs);

      return new XmlQualifiedName("string", xs);
    }
  }
}