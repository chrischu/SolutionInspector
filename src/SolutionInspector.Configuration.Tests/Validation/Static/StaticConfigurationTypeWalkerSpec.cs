using System.Reflection;
using FakeItEasy;
using Machine.Specifications;
using SolutionInspector.Configuration.Validation.Static;

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

namespace SolutionInspector.Configuration.Tests.Validation.Static
{
  [Subject (typeof(StaticConfigurationTypeWalker))]
  class StaticConfigurationTypeWalkerSpec
  {
    static StaticConfigurationTypeWalker SUT;

    static IStaticConfigurationVisitor StaticConfigurationVisitor;

    Establish ctx = () =>
    {
      SUT = new StaticConfigurationTypeWalker();

      StaticConfigurationVisitor = A.Fake<IStaticConfigurationVisitor>();
    };

    class when_walking
    {
      Because of = () => SUT.Walk(typeof(Element), StaticConfigurationVisitor);

      It calls_BeginTypeVisit_for_top_level_type = () =>
            A.CallTo(() => StaticConfigurationVisitor.BeginTypeVisit("", typeof(Element))).MustHaveHappened(Repeated.Exactly.Once);

      It calls_VisitValue_for_top_level_value = () =>
            AssertValueVisit("Value", typeof(Element).GetProperty("Value"));

      It calls_VisitCollection_for_top_level_collection = () =>
            AssertCollectionVisit("Collection", typeof(Element).GetProperty("Collection"));

      It calls_VisitSubelement_for_top_level_subelement = () =>
            AssertSubelementVisit("Subelement", typeof(Element).GetProperty("Subelement"));

      It calls_EndTypeVisit_for_top_level_type = () =>
            A.CallTo(() => StaticConfigurationVisitor.EndTypeVisit("", typeof(Element))).MustHaveHappened(Repeated.Exactly.Once);

      It calls_BeginTypeVisit_for_subelement_type = () =>
            A.CallTo(() => StaticConfigurationVisitor.BeginTypeVisit("Subelement", typeof(SubElement))).MustHaveHappened(Repeated.Exactly.Once);

      It calls_VisitValue_for_subelement_value = () =>
            AssertValueVisit("Subelement.Value", typeof(SubElement).GetProperty("Value"));

      It calls_VisitCollection_for_subelement_collection = () =>
            AssertCollectionVisit("Subelement.Collection", typeof(SubElement).GetProperty("Collection"));

      It calls_VisitSubelement_for_subelement_subelement = () =>
            AssertSubelementVisit("Subelement.Subelement", typeof(SubElement).GetProperty("Subelement"));

      It calls_EndTypeVisit_for_subelement_type = () =>
            A.CallTo(() => StaticConfigurationVisitor.EndTypeVisit("Subelement", typeof(SubElement))).MustHaveHappened(Repeated.Exactly.Once);

      It calls_BeginTypeVisit_for_collection_type = () =>
            A.CallTo(() => StaticConfigurationVisitor.BeginTypeVisit("Collection", typeof(CollectionElement))).MustHaveHappened(Repeated.Exactly.Once);

      It calls_VisitValue_for_collection_value = () =>
            AssertValueVisit("Collection.Value", typeof(CollectionElement).GetProperty("Value"));

      It calls_VisitCollection_for_collection_collection = () =>
            AssertCollectionVisit("Collection.Collection", typeof(CollectionElement).GetProperty("Collection"));

      It calls_VisitSubelement_for_collection_subelement = () =>
            AssertSubelementVisit("Collection.Subelement", typeof(CollectionElement).GetProperty("Subelement"));

      It calls_EndTypeVisit_for_collection_type = () =>
            A.CallTo(() => StaticConfigurationVisitor.EndTypeVisit("Collection", typeof(CollectionElement))).MustHaveHappened(Repeated.Exactly.Once);
    }

    class when_walking_directly_recursive_type
    {
      Because of = () => SUT.Walk(typeof(RecursiveElement), StaticConfigurationVisitor);

      It calls_BeginTypeVisit_for_top_level_type = () =>
            A.CallTo(() => StaticConfigurationVisitor.BeginTypeVisit("", typeof(RecursiveElement))).MustHaveHappened(Repeated.Exactly.Once);

      It calls_VisitSubelement_for_top_level_subelement = () =>
            AssertSubelementVisit("Recursive", typeof(RecursiveElement).GetProperty("Recursive"));

      It calls_EndTypeVisit_for_top_level_type = () =>
            A.CallTo(() => StaticConfigurationVisitor.EndTypeVisit("", typeof(RecursiveElement))).MustHaveHappened(Repeated.Exactly.Once);
    }

    class when_walking_indirectly_recursive_type
    {
      Because of = () => SUT.Walk(typeof(IndirectRecursiveElement), StaticConfigurationVisitor);

      It calls_BeginTypeVisit_for_top_level_type = () =>
            A.CallTo(() => StaticConfigurationVisitor.BeginTypeVisit("", typeof(IndirectRecursiveElement))).MustHaveHappened(Repeated.Exactly.Once);

      It calls_VisitSubelement_for_top_level_subelement = () =>
            AssertSubelementVisit("Subelement", typeof(IndirectRecursiveElement).GetProperty("Subelement"));

      It calls_EndTypeVisit_for_top_level_type = () =>
            A.CallTo(() => StaticConfigurationVisitor.EndTypeVisit("", typeof(IndirectRecursiveElement))).MustHaveHappened(Repeated.Exactly.Once);

      It calls_BeginTypeVisit_for_subelement_type = () =>
            A.CallTo(() => StaticConfigurationVisitor.BeginTypeVisit("Subelement", typeof(InbetweenRecursiveElement))).MustHaveHappened(Repeated.Exactly.Once);

      It calls_VisitSubelement_for_subelement_subelement = () =>
            AssertSubelementVisit("Subelement.Subelement", typeof(InbetweenRecursiveElement).GetProperty("Subelement"));

      It calls_EndTypeVisit_for_subelement_type = () =>
            A.CallTo(() => StaticConfigurationVisitor.EndTypeVisit("Subelement", typeof(InbetweenRecursiveElement))).MustHaveHappened(Repeated.Exactly.Once);
    }

    private static void AssertValueVisit (string propertyPath, PropertyInfo property)
    {
      A.CallTo(() => StaticConfigurationVisitor.VisitValue(propertyPath, property, property.GetCustomAttribute<ConfigurationValueAttribute>()))
          .MustHaveHappened(Repeated.Exactly.Once);
    }

    private static void AssertCollectionVisit (string propertyPath, PropertyInfo property)
    {
      A.CallTo(() => StaticConfigurationVisitor.VisitCollection(propertyPath, property, property.GetCustomAttribute<ConfigurationCollectionAttribute>()))
          .MustHaveHappened(Repeated.Exactly.Once);
    }

    private static void AssertSubelementVisit (string propertyPath, PropertyInfo property)
    {
      A.CallTo(() => StaticConfigurationVisitor.VisitSubelement(propertyPath, property, property.GetCustomAttribute<ConfigurationSubelementAttribute>()))
          .MustHaveHappened(Repeated.Exactly.Once);
    }

    private class Element : ConfigurationElement
    {
      [ConfigurationValue]
      public string Value { get; set; }

      [ConfigurationSubelement]
      public SubElement Subelement { get; set; }

      [ConfigurationCollection]
      public ConfigurationElementCollection<CollectionElement> Collection { get; set; }
    }

    private class SubElement : ConfigurationElement
    {
      [ConfigurationValue]
      public string Value { get; set; }

      [ConfigurationSubelement]
      public EmptyElement Subelement { get; set; }

      [ConfigurationCollection]
      public ConfigurationElementCollection<EmptyElement> Collection { get; set; }
    }

    private class CollectionElement : ConfigurationElement
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

    class RecursiveElement : ConfigurationElement
    {
      [ConfigurationSubelement]
      public RecursiveElement Recursive { get; set; }
    }

    class IndirectRecursiveElement : ConfigurationElement
    {
      [ConfigurationSubelement]
      public InbetweenRecursiveElement Subelement { get; set; }
    }

    class InbetweenRecursiveElement : ConfigurationElement
    {
      [ConfigurationSubelement]
      public IndirectRecursiveElement Subelement { get; set; }
    }
  }
}