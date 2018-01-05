using System;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Configuration;
using SolutionInspector.Configuration.Attributes;
using SolutionInspector.Configuration.Collections;
using SolutionInspector.SchemaGenerator.SchemaModel;

namespace SolutionInspector.SchemaGenerator.Tests
{
  [TestFixture]
  public class SchemaInfoRetrieverTests
  {
    private ISchemaInfoRetriever _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new SchemaInfoRetriever();
    }

    [Test]
    public void GetSchemaInfo ()
    {
      // ACT
      var result = _sut.GetSchemaInfo(typeof(SomeConfigurationDocument));

      // ASSERT
      result.ShouldBeEquivalentTo(
          new ConfigurationDocumentSchemaInfo(
              new ConfigurationElementSchemaInfo(
                  "rootElement",
                  minOccurs: 1,
                  maxOccurs: 1,
                  possibleAttributes: new[]
                                      {
                                          new ConfigurationAttributeSchemaInfo("value", typeof(string), isRequired: true, defaultValue: null),
                                          new ConfigurationAttributeSchemaInfo("optValue", typeof(string), isRequired: false, defaultValue: "default")
                                      },
                  possibleSubelements: new[]
                                       {
                                           new ConfigurationElementSchemaInfo(
                                               "subelement",
                                               minOccurs: 1,
                                               maxOccurs: 1,
                                               possibleAttributes: new[]
                                                                   {
                                                                       new ConfigurationAttributeSchemaInfo(
                                                                           "subValue",
                                                                           typeof(string),
                                                                           isRequired: true,
                                                                           defaultValue: null)
                                                                   },
                                               possibleSubelements: new ConfigurationElementSchemaInfo[0]),
                                           new ConfigurationElementSchemaInfo(
                                               "optSubelement",
                                               minOccurs: 0,
                                               maxOccurs: 1,
                                               possibleAttributes: new[]
                                                                   {
                                                                       new ConfigurationAttributeSchemaInfo(
                                                                           "subValue",
                                                                           typeof(string),
                                                                           isRequired: true,
                                                                           defaultValue: null)
                                                                   },
                                               possibleSubelements: new ConfigurationElementSchemaInfo[0]),
                                           new ConfigurationElementSchemaInfo(
                                               "collection",
                                               minOccurs: 1,
                                               maxOccurs: 1,
                                               possibleAttributes: new ConfigurationAttributeSchemaInfo[0],
                                               possibleSubelements: new[]
                                                                    {
                                                                        new ConfigurationElementSchemaInfo(
                                                                            "item",
                                                                            0,
                                                                            int.MaxValue,
                                                                            new[]
                                                                            {
                                                                                new ConfigurationAttributeSchemaInfo(
                                                                                    "collValue",
                                                                                    typeof(string),
                                                                                    isRequired: true,
                                                                                    defaultValue: null)
                                                                            },
                                                                            new ConfigurationElementSchemaInfo[0])
                                                                    }),
                                           new ConfigurationElementSchemaInfo(
                                               "optColl",
                                               minOccurs: 0,
                                               maxOccurs: 1,
                                               possibleAttributes: new ConfigurationAttributeSchemaInfo[0],
                                               possibleSubelements: new[]
                                                                    {
                                                                        new ConfigurationElementSchemaInfo(
                                                                            "optCollItem",
                                                                            4,
                                                                            7,
                                                                            new[]
                                                                            {
                                                                                new ConfigurationAttributeSchemaInfo(
                                                                                    "collValue",
                                                                                    typeof(string),
                                                                                    isRequired: true,
                                                                                    defaultValue: null)
                                                                            },
                                                                            new ConfigurationElementSchemaInfo[0])
                                                                    }),
                                           new ConfigurationElementSchemaInfo(
                                               "defaultCollItem",
                                               minOccurs: 0,
                                               maxOccurs: int.MaxValue,
                                               possibleAttributes: new[]
                                                                   {
                                                                       new ConfigurationAttributeSchemaInfo(
                                                                           "collValue",
                                                                           typeof(string),
                                                                           isRequired: true,
                                                                           defaultValue: null)
                                                                   },
                                               possibleSubelements: new ConfigurationElementSchemaInfo[0])
                                       })));
    }

    // ReSharper disable ClassNeverInstantiated.Local
    // ReSharper disable UnusedMember.Local
    private class SomeConfigurationDocument : ConfigurationDocument
    {
      public SomeConfigurationDocument () : base("rootElement")
      {
        
      }

      [ConfigurationValue]
      public string Value { get; set; }

      [ConfigurationValue(AttributeName = "optValue", IsOptional = true, DefaultValue = "default")]
      public string OptionalValue { get; set; }

      [ConfigurationSubelement]
      public Subelement Subelement { get; set; }

      [ConfigurationSubelement(ElementName = "optSubelement", IsOptional = true)]
      public Subelement OptionalSubelement { get; set; }

      [ConfigurationCollection]
      public IConfigurationElementCollection<CollectionItem> Collection { get; set; }

      [ConfigurationCollection(CollectionName = "optColl", ElementName = "optCollItem", IsOptional = true, MinimumElementCount = 4, MaximumElementCount = 7)]
      public IConfigurationElementCollection<CollectionItem> OptionalCollection { get; set; }

      [ConfigurationCollection(IsDefaultCollection = true, ElementName = "defaultCollItem")]
      public IConfigurationElementCollection<CollectionItem> DefaultCollection { get; set; }
    }

    private class Subelement : ConfigurationElement
    {
      [ConfigurationValue(AttributeName = "subValue")]
      public string Value { get; set; }
    }

    private class CollectionItem : ConfigurationElement
    {
      [ConfigurationValue(AttributeName = "collValue")]
      public string Value { get; set; }
    }
    // ReSharper restore ClassNeverInstantiated.Local
    // ReSharper restore UnusedMember.Local
  }
}