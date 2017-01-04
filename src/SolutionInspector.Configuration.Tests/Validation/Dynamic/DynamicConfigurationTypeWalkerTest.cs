using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Configuration.Validation.Dynamic;

namespace SolutionInspector.Configuration.Tests.Validation.Dynamic
{
  public class DynamicConfigurationTypeWalkerTest
  {
    private DynamicConfigurationTypeWalker _sut;

    private IDynamicConfigurationVisitor _dynamicConfigurationVisitor;

    [SetUp]
    public void SetUp ()
    {
      _sut = new DynamicConfigurationTypeWalker();

      _dynamicConfigurationVisitor = A.Fake<IDynamicConfigurationVisitor>();
    }

    [Test]
    public void Walk ()
    {
      // Element
      var element = new XElement("element");
      var elementValue = new XAttribute("value", "top");
      element.Add(elementValue);

      // Subelement
      var subelement = new XElement("subelement");

      var subelementValue = new XAttribute("value", "sub");
      subelement.Add(subelementValue);

      var subelementSubelement = new XElement("subelement");
      subelement.Add(subelementSubelement);

      var subelementCollectionElement = new XElement("collection");
      subelement.Add(subelementCollectionElement);

      // Collection
      var collectionElement = new XElement("collection");

      var collectionItemElement = new XElement("item");
      collectionElement.Add(collectionItemElement);

      var collectionItemElementValue = new XAttribute("value", "coll");
      collectionItemElement.Add(collectionItemElementValue);

      var collectionItemElementSubelement = new XElement("subelement");
      collectionItemElement.Add(collectionItemElementSubelement);

      var collectionItemElementCollectionElement = new XElement("collection");
      collectionItemElement.Add(collectionItemElementCollectionElement);

      element.Add(subelement);
      element.Add(collectionElement);

      // ACT
      _sut.Walk(typeof(ConfigurationTopElement), element, _dynamicConfigurationVisitor);

      // ASSERT
      AssertBeginTypeVisit("", typeof(ConfigurationTopElement), element);
      AssertValueVisit("Value", typeof(ConfigurationTopElement).GetProperty("Value"), elementValue);
      AssertCollectionVisit(
        "Collection",
        typeof(ConfigurationTopElement).GetProperty("Collection"),
        collectionElement,
        new[] { collectionItemElement });
      AssertSubelementVisit("Subelement", typeof(ConfigurationTopElement).GetProperty("Subelement"), subelement);
      AssertEndTypeVisit("", typeof(ConfigurationTopElement), element);

      AssertBeginTypeVisit("Subelement", typeof(ConfigurationSubElement), subelement);
      AssertValueVisit("Subelement.Value", typeof(ConfigurationSubElement).GetProperty("Value"), subelementValue);
      AssertCollectionVisit(
        "Subelement.Collection",
        typeof(ConfigurationSubElement).GetProperty("Collection"),
        subelementCollectionElement,
        new XElement[0]);
      AssertSubelementVisit("Subelement.Subelement", typeof(ConfigurationSubElement).GetProperty("Subelement"), subelementSubelement);
      AssertEndTypeVisit("Subelement", typeof(ConfigurationSubElement), subelement);

      AssertBeginTypeVisit("Collection[0]", typeof(ConfigurationCollectionElement), collectionItemElement);
      AssertValueVisit("Collection[0].Value", typeof(ConfigurationCollectionElement).GetProperty("Value"), collectionItemElementValue);
      AssertCollectionVisit(
        "Collection[0].Collection",
        typeof(ConfigurationCollectionElement).GetProperty("Collection"),
        collectionItemElementCollectionElement,
        new XElement[0]);
      AssertSubelementVisit(
        "Collection[0].Subelement",
        typeof(ConfigurationCollectionElement).GetProperty("Subelement"),
        collectionItemElementSubelement);
      AssertEndTypeVisit("Collection[0]", typeof(ConfigurationCollectionElement), collectionItemElement);
    }

    private void AssertBeginTypeVisit(string propertyPath, Type type, XElement element)
    {
      A.CallTo(() => _dynamicConfigurationVisitor.BeginTypeVisit(propertyPath, type, element)).MustHaveHappened(Repeated.Exactly.Once);
    }

    private void AssertEndTypeVisit(string propertyPath, Type type, XElement element)
    {
      A.CallTo(() => _dynamicConfigurationVisitor.EndTypeVisit(propertyPath, type, element)).MustHaveHappened(Repeated.Exactly.Once);
    }

    private void AssertValueVisit (string propertyPath, PropertyInfo property, XAttribute attribute)
    {
      A.CallTo(
            () =>
              _dynamicConfigurationVisitor.VisitValue(
                propertyPath,
                property,
                property.GetCustomAttribute<ConfigurationValueAttribute>(),
                attribute))
          .MustHaveHappened(Repeated.Exactly.Once);
    }

    private void AssertCollectionVisit (
      string propertyPath,
      PropertyInfo property,
      XElement element,
      IReadOnlyCollection<XElement> collectionItems)
    {
      A.CallTo(
            () =>
              _dynamicConfigurationVisitor.VisitCollection(
                propertyPath,
                property,
                property.GetCustomAttribute<ConfigurationCollectionAttribute>(),
                element,
                A<IReadOnlyCollection<XElement>>.That.Matches(c => Predicate(c, collectionItems), "")))
          .MustHaveHappened(Repeated.Exactly.Once);
    }

    private static bool Predicate (IReadOnlyCollection<XElement> collection, IReadOnlyCollection<XElement> expectedCollection)
    {
      try
      {
        collection.ShouldBeEquivalentTo(expectedCollection);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    private void AssertSubelementVisit (string propertyPath, PropertyInfo property, XElement element)
    {
      A.CallTo(
            () =>
              _dynamicConfigurationVisitor.VisitSubelement(
                propertyPath,
                property,
                property.GetCustomAttribute<ConfigurationSubelementAttribute>(),
                element))
          .MustHaveHappened(Repeated.Exactly.Once);
    }

    // ReSharper disable UnusedMember.Local
    private class ConfigurationTopElement : ConfigurationElement
    {
      [ConfigurationValue]
      public string Value { get; set; }

      [ConfigurationSubelement]
      public ConfigurationSubElement Subelement { get; set; }

      [ConfigurationCollection]
      public ConfigurationElementCollection<ConfigurationCollectionElement> Collection { get; set; }
    }

    private class ConfigurationSubElement : ConfigurationElement
    {
      [ConfigurationValue]
      public string Value { get; set; }

      [ConfigurationSubelement]
      public EmptyElement Subelement { get; set; }

      [ConfigurationCollection]
      public ConfigurationElementCollection<EmptyElement> Collection { get; set; }
    }

    private class ConfigurationCollectionElement : ConfigurationElement
    {
      [ConfigurationValue]
      public string Value { get; set; }

      [ConfigurationSubelement]
      public EmptyElement Subelement { get; set; }

      [ConfigurationCollection]
      public ConfigurationElementCollection<EmptyElement> Collection { get; set; }
    }

    private class EmptyElement : ConfigurationElement
    {
    }

    // ReSharper restore UnusedMember.Local
  }
}