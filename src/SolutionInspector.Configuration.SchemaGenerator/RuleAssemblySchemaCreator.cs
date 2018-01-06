using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.BuildTool.SchemaModel;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration.Validation;
using Wrapperator.Interfaces.Reflection;

namespace SolutionInspector.BuildTool
{
  /// <summary>
  ///   Creates a <see cref="XmlSchema" /> from an <see cref="IAssembly" /> containing <see cref="RuleConfigurationElement" />s.
  /// </summary>
  public interface IRuleAssemblySchemaCreator
  {
    XmlSchema CreateSchema (IAssembly ruleAssembly, string baseNamespace);
  }

  internal class RuleAssemblySchemaCreator : IRuleAssemblySchemaCreator
  {
    private readonly ISchemaInfoRetriever _schemaInfoRetriever;

    public RuleAssemblySchemaCreator (ISchemaInfoRetriever schemaInfoRetriever)
    {
      _schemaInfoRetriever = schemaInfoRetriever;
    }

    public XmlSchema CreateSchema (IAssembly ruleAssembly, string baseNamespace)
    {
      var schema = new XmlSchema
                   {
                       TargetNamespace = ruleAssembly.GetName().Name,
                       ElementFormDefault = XmlSchemaForm.Qualified
                   };
      schema.Namespaces.Add("", schema.TargetNamespace);
      schema.Namespaces.Add("rb", baseNamespace);

      XmlSchemaImport baseImport = new XmlSchemaImport { Namespace = baseNamespace, SchemaLocation = baseNamespace };
      schema.Includes.Add(baseImport);

      var ruleTypes = ruleAssembly.GetTypes().Where(t => typeof(RuleConfigurationElement).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface).ToList();

      if (ruleTypes.Count == 0)
        throw new RuleAssemblyContainsNoRulesException($"The assembly '{ruleAssembly.GetName().Name}' contains no rules.");

      var ruleProcessor = new RuleElementSchemaInfoProcessor(schema, baseNamespace);

      foreach (var ruleType in ruleTypes)
      {
        ConfigurationValidator.Validate(ruleType);
        var schemaInfo = _schemaInfoRetriever.GetSchemaInfo(ruleType);
        ruleProcessor.Process(schemaInfo);
      }

      ValidateSchema(schema);

      return schema;
    }

    private void ValidateSchema (XmlSchema schema)
    {
      var schemaSet = new XmlSchemaSet();
      schemaSet.Add(schema);
      schemaSet.Compile();
    }

    private abstract class ElementSchemaInfoProcessor
    {
      protected XmlSchema Schema { get; }

      protected ElementSchemaInfoProcessor (XmlSchema schema)
      {
        Schema = schema;
      }

      public XmlSchemaComplexType Process (ConfigurationElementSchemaInfo elementSchemaInfo)
      {
        var element = CreateRootElement(elementSchemaInfo);
        var type = CreateRootElementType(elementSchemaInfo);

        element.SchemaTypeName = new XmlQualifiedName(type.Name, Schema.TargetNamespace);

        AddElementAndTypeToSchema(element, type);

        foreach (var attributeSchemaInfo in elementSchemaInfo.PossibleAttributes)
        {
          var attribute = new XmlSchemaAttribute
                          {
                              Name = attributeSchemaInfo.Name,
                              SchemaTypeName = MapType(attributeSchemaInfo.Type),
                              Use = attributeSchemaInfo.IsRequired ? XmlSchemaUse.Required : XmlSchemaUse.Optional,
                              DefaultValue = attributeSchemaInfo.DefaultValue
                          };

          AddContentAttribute(type, attribute);
        }

        if (elementSchemaInfo.PossibleSubelements.Any())
        {
          var sequence = new XmlSchemaSequence();

          foreach (var subElementSchemaInfo in elementSchemaInfo.PossibleSubelements)
          {
            var subElementType = ProcessSubElement(subElementSchemaInfo);

            var subElement = CreateSubElement(subElementSchemaInfo);
            subElement.SchemaType = subElementType;

            sequence.Items.Add(subElement);
          }

          SetContentParticle(type, sequence);
        }

        return type;
      }

      protected virtual void AddElementAndTypeToSchema (XmlSchemaElement element, XmlSchemaComplexType type)
      {
      }

      protected XmlSchemaElement CreateSubElement (ConfigurationElementSchemaInfo elementSchemaInfo)
      {
        var element = new XmlSchemaElement { Name = elementSchemaInfo.Name };

        element.MinOccurs = elementSchemaInfo.MinOccurs;

        if (elementSchemaInfo.MaxOccurs == int.MaxValue)
          element.MaxOccursString = "unbounded";
        else
          element.MaxOccurs = elementSchemaInfo.MaxOccurs;

        return element;
      }

      protected abstract XmlSchemaElement CreateRootElement (ConfigurationElementSchemaInfo elementSchemaInfo);

      protected abstract XmlSchemaComplexType CreateRootElementType (ConfigurationElementSchemaInfo elementSchemaInfo);

      protected abstract XmlSchemaComplexType ProcessSubElement (ConfigurationElementSchemaInfo elementSchemaInfo);

      protected abstract void AddContentAttribute (XmlSchemaComplexType type, XmlSchemaAttribute attribute);
      protected abstract void SetContentParticle (XmlSchemaComplexType type, XmlSchemaParticle particle);

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

    private class RuleElementSchemaInfoProcessor : ElementSchemaInfoProcessor
    {
      private readonly string _baseNamespace;

      public RuleElementSchemaInfoProcessor (XmlSchema schema, string baseNamespace) : base(schema)
      {
        _baseNamespace = baseNamespace;
      }

      protected override XmlSchemaElement CreateRootElement (ConfigurationElementSchemaInfo elementSchemaInfo)
      {
        return new XmlSchemaElement
               {
                   Name = elementSchemaInfo.Name,
                   SubstitutionGroup = new XmlQualifiedName("rule", _baseNamespace)
               };
      }

      protected override void AddElementAndTypeToSchema (XmlSchemaElement element, XmlSchemaComplexType type)
      {
        Schema.Items.Add(element);
        Schema.Items.Add(type);
      }

      protected override XmlSchemaComplexType CreateRootElementType (ConfigurationElementSchemaInfo elementSchemaInfo)
      {
        return new XmlSchemaComplexType
               {
                   Name = elementSchemaInfo.Name.ToFirstCharUpper(),
                   ContentModel = new XmlSchemaComplexContent
                                  {
                                      Content = new XmlSchemaComplexContentExtension
                                                {
                                                    BaseTypeName = new XmlQualifiedName(
                                                        "RuleBase",
                                                        _baseNamespace)
                                                }
                                  }
               };
      }

      protected override XmlSchemaComplexType ProcessSubElement (ConfigurationElementSchemaInfo elementSchemaInfo)
      {
        return new SubelementSchemaInfoProcessor(Schema).Process(elementSchemaInfo);
      }

      protected override void AddContentAttribute (XmlSchemaComplexType type, XmlSchemaAttribute attribute)
      {
        ((XmlSchemaComplexContentExtension) type.ContentModel.Content).Attributes.Add(attribute);
      }

      protected override void SetContentParticle (XmlSchemaComplexType type, XmlSchemaParticle particle)
      {
        ((XmlSchemaComplexContentExtension) type.ContentModel.Content).Particle = particle;
      }
    }

    private class SubelementSchemaInfoProcessor : ElementSchemaInfoProcessor
    {
      public SubelementSchemaInfoProcessor (XmlSchema schema)
          : base(schema)
      {
      }

      protected override XmlSchemaElement CreateRootElement (ConfigurationElementSchemaInfo elementSchemaInfo) => CreateSubElement(elementSchemaInfo);

      protected override XmlSchemaComplexType CreateRootElementType (ConfigurationElementSchemaInfo elementSchemaInfo) => new XmlSchemaComplexType();

      protected override XmlSchemaComplexType ProcessSubElement (ConfigurationElementSchemaInfo elementSchemaInfo) => Process(elementSchemaInfo);

      protected override void AddContentAttribute (XmlSchemaComplexType type, XmlSchemaAttribute attribute) => type.Attributes.Add(attribute);

      protected override void SetContentParticle (XmlSchemaComplexType type, XmlSchemaParticle particle) => type.Particle = particle;
    }
  }
}