using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Configuration.Validation.Dynamic;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeModifiers
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.Configuration.Tests.Validation.Dynamic
{
  [Subject (typeof(DynamicConfigurationTypeWalker))]
  class DynamicConfigurationTypeWalkerSpec
  {
    static DynamicConfigurationTypeWalker SUT;

    static IDynamicConfigurationVisitor DynamicConfigurationVisitor;

    Establish ctx = () =>
    {
      SUT = new DynamicConfigurationTypeWalker();

      DynamicConfigurationVisitor = A.Fake<IDynamicConfigurationVisitor>();
    };

    class when_walking
    {
      Establish ctx = () =>
      {
        // Element
        Element = new XElement("element");
        ElementValue = new XAttribute("value", "top");
        Element.Add(ElementValue);

        // Subelement
        Subelement = new XElement("subelement");

        SubelementValue = new XAttribute("value", "sub");
        Subelement.Add(SubelementValue);

        SubelementSubelement = new XElement("subelement");
        Subelement.Add(SubelementSubelement);

        SubelementCollectionElement = new XElement("collection");
        Subelement.Add(SubelementCollectionElement);

        // Collection
        CollectionElement = new XElement("collection");

        CollectionItemElement = new XElement("item");
        CollectionElement.Add(CollectionItemElement);

        CollectionItemElementValue = new XAttribute("value", "coll");
        CollectionItemElement.Add(CollectionItemElementValue);

        CollectionItemElementSubelement = new XElement("subelement");
        CollectionItemElement.Add(CollectionItemElementSubelement);

        CollectionItemElementCollectionElement = new XElement("collection");
        CollectionItemElement.Add(CollectionItemElementCollectionElement);

        Element.Add(Subelement);
        Element.Add(CollectionElement);
      };

      Because of = () => SUT.Walk(typeof(ConfigurationTopElement), Element, DynamicConfigurationVisitor);

      It calls_BeginTypeVisit_for_top_level_type = () =>
            A.CallTo(() => DynamicConfigurationVisitor.BeginTypeVisit("", typeof(ConfigurationTopElement), Element)).MustHaveHappened(Repeated.Exactly.Once);

      It calls_VisitValue_for_top_level_value = () =>
            AssertValueVisit("Value", typeof(ConfigurationTopElement).GetProperty("Value"), ElementValue);

      It calls_VisitCollection_for_top_level_collection = () =>
            AssertCollectionVisit("Collection", typeof(ConfigurationTopElement).GetProperty("Collection"), CollectionElement, new[] { CollectionItemElement });

      It calls_VisitSubelement_for_top_level_subelement = () =>
            AssertSubelementVisit("Subelement", typeof(ConfigurationTopElement).GetProperty("Subelement"), Subelement);

      It calls_EndTypeVisit_for_top_level_type = () =>
            A.CallTo(() => DynamicConfigurationVisitor.EndTypeVisit("", typeof(ConfigurationTopElement), Element)).MustHaveHappened(Repeated.Exactly.Once);

      It calls_BeginTypeVisit_for_subelement_type = () =>
        A.CallTo(() => DynamicConfigurationVisitor.BeginTypeVisit("Subelement", typeof(ConfigurationSubElement), Subelement))
            .MustHaveHappened(Repeated.Exactly.Once);

      It calls_VisitValue_for_subelement_value = () =>
            AssertValueVisit("Subelement.Value", typeof(ConfigurationSubElement).GetProperty("Value"), SubelementValue);

      It calls_VisitCollection_for_subelement_collection = () =>
        AssertCollectionVisit(
          "Subelement.Collection",
          typeof(ConfigurationSubElement).GetProperty("Collection"),
          SubelementCollectionElement,
          new XElement[0]);

      It calls_VisitSubelement_for_subelement_subelement = () =>
            AssertSubelementVisit("Subelement.Subelement", typeof(ConfigurationSubElement).GetProperty("Subelement"), SubelementSubelement);

      It calls_EndTypeVisit_for_subelement_type = () =>
        A.CallTo(() => DynamicConfigurationVisitor.EndTypeVisit("Subelement", typeof(ConfigurationSubElement), Subelement))
            .MustHaveHappened(Repeated.Exactly.Once);

      It calls_BeginTypeVisit_for_collection_type = () =>
        A.CallTo(() => DynamicConfigurationVisitor.BeginTypeVisit("Collection[0]", typeof(ConfigurationCollectionElement), CollectionItemElement))
            .MustHaveHappened(Repeated.Exactly.Once);

      It calls_VisitValue_for_collection_value = () =>
            AssertValueVisit("Collection[0].Value", typeof(ConfigurationCollectionElement).GetProperty("Value"), CollectionItemElementValue);

      It calls_VisitCollection_for_collection_collection = () =>
        AssertCollectionVisit(
          "Collection[0].Collection",
          typeof(ConfigurationCollectionElement).GetProperty("Collection"),
          CollectionItemElementCollectionElement,
          new XElement[0]);

      It calls_VisitSubelement_for_collection_subelement = () =>
            AssertSubelementVisit("Collection[0].Subelement", typeof(ConfigurationCollectionElement).GetProperty("Subelement"), CollectionItemElementSubelement);

      It calls_EndTypeVisit_for_collection_type = () =>
        A.CallTo(() => DynamicConfigurationVisitor.EndTypeVisit("Collection[0]", typeof(ConfigurationCollectionElement), CollectionItemElement))
            .MustHaveHappened(Repeated.Exactly.Once);

      static XElement Element;
      static XAttribute ElementValue;

      static XElement Subelement;
      static XAttribute SubelementValue;
      static XElement SubelementSubelement;
      static XElement SubelementCollectionElement;

      static XElement CollectionElement;
      static XElement CollectionItemElement;
      static XAttribute CollectionItemElementValue;
      static XElement CollectionItemElementSubelement;
      static XElement CollectionItemElementCollectionElement;
    }

    private static void AssertValueVisit (string propertyPath, PropertyInfo property, XAttribute attribute)
    {
      A.CallTo(
            () =>
              DynamicConfigurationVisitor.VisitValue(
                propertyPath,
                property,
                property.GetCustomAttribute<ConfigurationValueAttribute>(),
                attribute))
          .MustHaveHappened(Repeated.Exactly.Once);
    }

    private static void AssertCollectionVisit (
      string propertyPath,
      PropertyInfo property,
      XElement element,
      IReadOnlyCollection<XElement> collectionItems)
    {
      A.CallTo(
            () =>
              DynamicConfigurationVisitor.VisitCollection(
                propertyPath,
                property,
                property.GetCustomAttribute<ConfigurationCollectionAttribute>(),
                element,
                A<IReadOnlyCollection<XElement>>.That.Matches(c => Predicate(c, collectionItems), "")))
          .MustHaveHappened(Repeated.Exactly.Once);
    }

    static bool Predicate (IReadOnlyCollection<XElement> collection, IReadOnlyCollection<XElement> expectedCollection)
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

    private static void AssertSubelementVisit (string propertyPath, PropertyInfo property, XElement element)
    {
      A.CallTo(
            () =>
              DynamicConfigurationVisitor.VisitSubelement(
                propertyPath,
                property,
                property.GetCustomAttribute<ConfigurationSubelementAttribute>(),
                element))
          .MustHaveHappened(Repeated.Exactly.Once);
    }

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

    class EmptyElement : ConfigurationElement
    {
    }
  }
}