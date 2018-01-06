using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Configuration;
using SolutionInspector.Configuration.Attributes;
using SolutionInspector.Configuration.Collections;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.TestInfrastructure;
using SolutionInspector.TestInfrastructure.AssertionExtensions;
using Wrapperator.Interfaces.Reflection;

namespace SolutionInspector.BuildTool.Tests
{
  [TestFixture]
  public class RuleAssemblySchemaCreatorTests
  {
    private IAssembly _assembly;
    private string _assemblyName;

    private RuleAssemblySchemaCreator _sut;

    [SetUp]
    public void SetUp ()
    {
      _assembly = A.Fake<IAssembly>();
      _assemblyName = "AssemblyName";

      var assemblyName = A.Fake<IAssemblyName>();
      A.CallTo(() => _assembly.GetName()).Returns(assemblyName);
      A.CallTo(() => assemblyName.Name).Returns(_assemblyName);

      _sut = new RuleAssemblySchemaCreator(new SchemaInfoRetriever());
    }

    [Test]
    public void CreateSchema ()
    {
      A.CallTo(() => _assembly.GetTypes()).Returns(new[] { typeof(SomeRule) });
      var baseNamespace = "http://chrischu.github.io/SolutionInspector/schema/base_v1.xsd";

      // ACT
      var result = _sut.CreateSchema(_assembly, baseNamespace);

      // ASSERT
      var schema = GetSchemaAsString(result);
      
      schema.Should().BeWithDiff(
          $@"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns=""{_assemblyName}"" xmlns:rb=""{baseNamespace}"" elementFormDefault=""qualified"" targetNamespace=""{_assemblyName}"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:import schemaLocation=""{baseNamespace}"" namespace=""{baseNamespace}"" />
  <xs:element name=""someRule"" substitutionGroup=""rb:rule"" type=""SomeRule"" />
  <xs:complexType name=""SomeRule"">
    <xs:complexContent mixed=""false"">
      <xs:extension base=""rb:RuleBase"">
        <xs:sequence>
          <xs:element minOccurs=""1"" maxOccurs=""1"" name=""subelement"">
            <xs:complexType>
              <xs:attribute name=""subValue"" type=""xs:string"" use=""required"" />
            </xs:complexType>
          </xs:element>
          <xs:element minOccurs=""0"" maxOccurs=""1"" name=""optSubelement"">
            <xs:complexType>
              <xs:attribute name=""subValue"" type=""xs:string"" use=""required"" />
            </xs:complexType>
          </xs:element>
          <xs:element minOccurs=""1"" maxOccurs=""1"" name=""collection"">
            <xs:complexType>
              <xs:sequence>
                <xs:element minOccurs=""0"" maxOccurs=""unbounded"" name=""item"">
                  <xs:complexType>
                    <xs:attribute name=""collValue"" type=""xs:int"" use=""required"" />
                  </xs:complexType>
                </xs:element>
              </xs:sequence>
            </xs:complexType>
          </xs:element>
          <xs:element minOccurs=""0"" maxOccurs=""1"" name=""optColl"">
            <xs:complexType>
              <xs:sequence>
                <xs:element minOccurs=""4"" maxOccurs=""7"" name=""optCollItem"">
                  <xs:complexType>
                    <xs:attribute name=""collValue"" type=""xs:int"" use=""required"" />
                  </xs:complexType>
                </xs:element>
              </xs:sequence>
            </xs:complexType>
          </xs:element>
          <xs:element minOccurs=""0"" maxOccurs=""unbounded"" name=""defaultCollItem"">
            <xs:complexType>
              <xs:attribute name=""collValue"" type=""xs:int"" use=""required"" />
            </xs:complexType>
          </xs:element>
        </xs:sequence>
        <xs:attribute name=""value"" type=""xs:string"" use=""required"" />
        <xs:attribute default=""default"" name=""optValue"" type=""xs:string"" use=""optional"" />
        <xs:attribute name=""type"" type=""xs:string"" use=""required"" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
</xs:schema>
");
    }

    [Test]
    public void CreateSchema_WithAssemblyWithoutRules_Throws ()
    {
      A.CallTo(() => _assembly.GetTypes()).Returns(new[] { typeof(AbstractRule) });

      // ACT
      Action act = () => _sut.CreateSchema(_assembly, Some.String);

      // ASSERT
      act.ShouldThrow<RuleAssemblyContainsNoRulesException>().WithMessage($"The assembly '{_assemblyName}' contains no rules.");
    }

    [Test]
    public void CreateSchema_WithInvalidRuleType_Throws ()
    {
      A.CallTo(() => _assembly.GetTypes()).Returns(new[] { typeof(InvalidRule) });
      
      // ACT
      Action act = () => _sut.CreateSchema(_assembly, Some.String);

      // ASSERT
      act.ShouldThrow<ConfigurationValidationException>();
    }

    [Test]
    public void CreateSchema_WithInvalidSchema_Throws ()
    {
      A.CallTo(() => _assembly.GetTypes()).Returns(new[] { typeof(SomeRule) });
      
      // ACT
      Action act = () => _sut.CreateSchema(_assembly, "DoesNotExist");

      // ASSERT
      act.ShouldThrow<XmlSchemaException>();
    }
    
    private string GetSchemaAsString (XmlSchema schema)
    {
      using (var stringWriter = new StringWriter())
      using (var xmlWriter = XmlWriter.Create(
          stringWriter,
          new XmlWriterSettings { NewLineChars = Environment.NewLine, Indent = true }))
      {
        schema.Write(xmlWriter);
        return stringWriter.ToString();
      }
    }

    // ReSharper disable ClassNeverInstantiated.Local
    // ReSharper disable UnusedMember.Local
    private class SomeRule : RuleConfigurationElement
    {
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
      public int Value { get; set; }
    }

    private abstract class AbstractRule : RuleConfigurationElement
    {
    }

    private class InvalidRule : RuleConfigurationElement
    {
      [ConfigurationValue(AttributeName = "value", DefaultValue = "")]
      public string Value { get; set; }
    }
    // ReSharper restore ClassNeverInstantiated.Local
    // ReSharper restore UnusedMember.Local
  }
}