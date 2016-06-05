using System;
using System.Xml;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Extensions;
using SolutionInspector.TestInfrastructure;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.Api.Tests.Extensions
{
  [Subject (typeof(XmlExtensions))]
  class XmlExtensionsSpec
  {
    class when_adding_an_attribute
    {
      Establish ctx = () => { XmlElement = CreateXmlElement("element"); };

      Because of = () => XmlElement.AddAttribute("attr", "value");

      It adds_attribute = () =>
      {
        var attribute = XmlElement.Attributes[0];
        attribute.Name.Should().Be("attr");
        attribute.Value.Should().Be("value");
      };

      static XmlElement XmlElement;
    }

    class when_adding_an_element
    {
      Establish ctx = () => { XmlElement = CreateXmlElement("element"); };

      Because of = () => XmlElement.AddElement("ele", "value");

      It adds_element = () =>
      {
        var child = XmlElement.FirstChild;
        child.Name.Should().Be("ele");
        child.InnerText.Should().Be("value");
      };

      static XmlElement XmlElement;
    }

    class when_removing_all_children
    {
      Establish ctx = () =>
      {
        XmlElement = CreateXmlElement("element");
        XmlElement.AddElement("ele", "value");
        XmlElement.AddAttribute("attr", "value");
      };

      Because of = () => XmlElement.RemoveAllChildren();

      It removes_all_children = () => XmlElement.ChildNodes.Should().BeEmpty();

      It leaves_attributes = () =>
          XmlElement.Attributes.Count.Should().Be(1);

      static XmlElement XmlElement;
    }

    class when_removing_attributes_where
    {
      Establish ctx = () =>
      {
        XmlElement = CreateXmlElement("element");
        XmlElement.AddAttribute("attr1", "value");
        XmlElement.AddAttribute("attr2", "value");
      };

      Because of = () => XmlElement.RemoveAttributesWhere(a => a.Name == "attr1");

      It removes_matching_attributes = () => XmlElement.Attributes[0].Name.Should().Be("attr2");

      static XmlElement XmlElement;
    }

    class when_removing_attributes_where_and_there_are_none
    {
      Establish ctx = () =>
      {
        XmlElement = CreateXmlElement("element");
      };

      Because of = () => Exception = Catch.Exception(() => XmlElement.RemoveAttributesWhere(a => a.Name == Some.String()));

      It does_not_throw = () =>
          Exception.Should().BeNull();

      static XmlElement XmlElement;
      static Exception Exception;
    }

    static XmlElement CreateXmlElement (string elementName)
    {
      var document = new XmlDocument();
      return document.CreateElement(elementName);
    }
  }
}