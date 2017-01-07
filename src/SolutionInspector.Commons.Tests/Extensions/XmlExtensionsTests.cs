using System;
using System.Xml;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.Commons.Tests.Extensions
{
  public class XmlExtensionsTests
  {
    [Test]
    public void AddAttribute_DefaultCase_Works ()
    {
      var element = CreateXmlElement("element");

      // ACT
      element.AddAttribute("attr", "value");

      // ASSERT
      var attribute = element.Attributes[0];
      attribute.Name.Should().Be("attr");
      attribute.Value.Should().Be("value");
    }

    [Test]
    public void AddAttribute_WithExistingAttribute_OverridesValue ()
    {
      var element = CreateXmlElement("element");
      element.AddAttribute("attr", "value");

      // ACT
      element.AddAttribute("attr", "newvalue");

      // ASSERT
      var attribute = element.Attributes[0];
      attribute.Name.Should().Be("attr");
      attribute.Value.Should().Be("newvalue");

      element.Attributes.Count.Should().Be(1);
    }

    [Test]
    public void AddElement_DefaultCase_Works ()
    {
      var element = CreateXmlElement("element");

      // ACT
      element.AddElement("ele", "value");

      // ASSERT
      var child = element.FirstChild;
      child.Name.Should().Be("ele");
      child.InnerText.Should().Be("value");
    }

    [Test]
    public void AddElement_WithExistingElement_AddsAnotherElement ()
    {
      var element = CreateXmlElement("element");
      element.AddElement("ele", "value");

      // ACT
      element.AddElement("ele", "newvalue");

      // ASSERT
      element.ChildNodes.Count.Should().Be(2);

      element.ChildNodes[0].Name.Should().Be("ele");
      element.ChildNodes[0].InnerText.Should().Be("value");

      element.ChildNodes[1].Name.Should().Be("ele");
      element.ChildNodes[1].InnerText.Should().Be("newvalue");
    }

    [Test]
    public void RemoveAllChildren_ElementWithAttributesAndElements_RemovesElementsButLeavesAttributes ()
    {
      var element = CreateXmlElement("element");
      element.AddAttribute("attr", "value");
      element.AddElement("ele", "value");

      // ACT
      element.RemoveAllChildren();

      // ASSERT
      element.Attributes.Count.Should().Be(1);
      element.ChildNodes.Should().BeEmpty();
    }

    [Test]
    public void RemoveAttributesWhere_WithMultipleAttributes_RemovesOnlyMatchingAttributes ()
    {
      var element = CreateXmlElement("element");
      element.AddAttribute("attr1", "value");
      element.AddAttribute("attr2", "value");

      // ACT
      element.RemoveAttributesWhere(a => a.Name == "attr1");

      // ASSERT
      element.Attributes.Count.Should().Be(1);
      element.Attributes[0].Name.Should().Be("attr2");
    }

    [Test]
    public void RemoveAttributesWhere_WithoutAttributes_DoesNotThrow ()
    {
      var element = CreateXmlElement("element");

      // ACT
      Action act = () => element.RemoveAttributesWhere(a => a.Name == "");

      // ASSERT
      act.ShouldNotThrow();
    }

    private XmlElement CreateXmlElement (string elementName)
    {
      var document = new XmlDocument();
      return document.CreateElement(elementName);
    }
  }
}