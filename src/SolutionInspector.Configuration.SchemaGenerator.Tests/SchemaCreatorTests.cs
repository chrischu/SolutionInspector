using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Configuration;
using SolutionInspector.Configuration.Attributes;
using SolutionInspector.Configuration.Collections;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

namespace SolutionInspector.SchemaGenerator.Tests
{
  [TestFixture]
  public class SchemaCreatorTests
  {
    private SchemaCreator _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new SchemaCreator(new SchemaInfoRetriever());
    }

    [Test]
    public void CreateSchema ()
    {
      // ACT
      var result = _sut.CreateSchema(typeof(SomeConfigurationDocument));

      // ASSERT
      var schema = ValidateAndGetSchemaAsString(result);
      
      schema.Should().BeWithDiff(
          @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns=""SolutionInspector.Configuration.SchemaGenerator.Tests"" elementFormDefault=""qualified"" targetNamespace=""SolutionInspector.Configuration.SchemaGenerator.Tests"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element name=""rootElement"" type=""RootElement"" />
  <xs:complexType name=""RootElement"">
    <xs:sequence>
      <xs:element minOccurs=""1"" maxOccurs=""1"" name=""subelement"" type=""Subelement"" />
      <xs:element minOccurs=""0"" maxOccurs=""1"" name=""optSubelement"" type=""OptSubelement"" />
      <xs:element minOccurs=""1"" maxOccurs=""1"" name=""collection"" type=""Collection"" />
      <xs:element minOccurs=""0"" maxOccurs=""1"" name=""optColl"" type=""OptColl"" />
      <xs:element minOccurs=""0"" maxOccurs=""unbounded"" name=""defaultCollItem"" type=""DefaultCollItem"" />
    </xs:sequence>
    <xs:attribute name=""value"" type=""xs:string"" use=""required"" />
    <xs:attribute default=""default"" name=""optValue"" type=""xs:string"" use=""optional"" />
  </xs:complexType>
  <xs:complexType name=""Subelement"">
    <xs:attribute name=""subValue"" type=""xs:string"" use=""required"" />
  </xs:complexType>
  <xs:complexType name=""OptSubelement"">
    <xs:attribute name=""subValue"" type=""xs:string"" use=""required"" />
  </xs:complexType>
  <xs:complexType name=""Collection"">
    <xs:sequence>
      <xs:element minOccurs=""0"" maxOccurs=""unbounded"" name=""item"" type=""Item"" />
    </xs:sequence>
  </xs:complexType>
  <xs:complexType name=""Item"">
    <xs:attribute name=""collValue"" type=""xs:string"" use=""required"" />
  </xs:complexType>
  <xs:complexType name=""OptColl"">
    <xs:sequence>
      <xs:element minOccurs=""4"" maxOccurs=""7"" name=""optCollItem"" type=""OptCollItem"" />
    </xs:sequence>
  </xs:complexType>
  <xs:complexType name=""OptCollItem"">
    <xs:attribute name=""collValue"" type=""xs:string"" use=""required"" />
  </xs:complexType>
  <xs:complexType name=""DefaultCollItem"">
    <xs:attribute name=""collValue"" type=""xs:string"" use=""required"" />
  </xs:complexType>
</xs:schema>
");
    }
    
    private string ValidateAndGetSchemaAsString (XmlSchema schema)
    {
      var schemaSet = new XmlSchemaSet();
      schemaSet.Add(schema);
      schemaSet.Compile();

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
    private class SomeConfigurationDocument : ConfigurationDocument
    {
      public SomeConfigurationDocument ()
          : base("rootElement")
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