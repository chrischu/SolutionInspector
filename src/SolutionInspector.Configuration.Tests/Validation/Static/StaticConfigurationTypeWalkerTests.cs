using System;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FakeItEasy.Configuration;
using NUnit.Framework;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration.Attributes;
using SolutionInspector.Configuration.Collections;
using SolutionInspector.Configuration.Validation.Static;

namespace SolutionInspector.Configuration.Tests.Validation.Static
{
  public class StaticConfigurationTypeWalkerTests
  {
    private IStaticConfigurationVisitor _staticConfigurationVisitor;
    private StaticConfigurationTypeWalker _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new StaticConfigurationTypeWalker();

      _staticConfigurationVisitor = A.Fake<IStaticConfigurationVisitor>();
    }

    [Test]
    public void Walk ()
    {
      // ACT
      _sut.Walk(typeof(Element), _staticConfigurationVisitor);

      // ASSERT
      AssertTypeVisit(
        "",
        typeof(Element),
        AssertValueVisit("Value", typeof(Element).GetProperty("Value").AssertNotNull()),
        AssertSubelementVisit("Subelement", typeof(Element).GetProperty("Subelement").AssertNotNull()),
        AssertCollectionVisit("Collection", typeof(Element).GetProperty("Collection").AssertNotNull())
      );

      AssertTypeVisit(
        "Subelement",
        typeof(SubElement),
        AssertValueVisit("Subelement.Value", typeof(SubElement).GetProperty("Value").AssertNotNull()),
        AssertSubelementVisit("Subelement.Subelement", typeof(SubElement).GetProperty("Subelement").AssertNotNull()),
        AssertCollectionVisit("Subelement.Collection", typeof(SubElement).GetProperty("Collection").AssertNotNull())
      );

      AssertTypeVisit(
        "Collection",
        typeof(CollectionElement),
        AssertValueVisit("Collection.Value", typeof(CollectionElement).GetProperty("Value").AssertNotNull()),
        AssertSubelementVisit("Collection.Subelement", typeof(CollectionElement).GetProperty("Subelement").AssertNotNull()),
        AssertCollectionVisit("Collection.Collection", typeof(CollectionElement).GetProperty("Collection").AssertNotNull())
      );
    }

    [Test]
    public void Walk_DirectlyRecursiveType_LimitsRecursion ()
    {
      // ACT
      _sut.Walk(typeof(RecursiveElement), _staticConfigurationVisitor);

      // ASSERT
      AssertTypeVisit(
        "",
        typeof(RecursiveElement),
        AssertSubelementVisit("Recursive", typeof(RecursiveElement).GetProperty("Recursive").AssertNotNull()));
    }

    [Test]
    public void Walk_IndirectlyRecursiveType_LimitsRecursion ()
    {
      // ACT
      _sut.Walk(typeof(IndirectRecursiveElement), _staticConfigurationVisitor);

      // ASSERT
      AssertTypeVisit(
        "",
        typeof(IndirectRecursiveElement),
        AssertSubelementVisit("Subelement", typeof(IndirectRecursiveElement).GetProperty("Subelement").AssertNotNull()));

      AssertTypeVisit(
        "Subelement",
        typeof(InbetweenRecursiveElement),
        AssertSubelementVisit("Subelement.Subelement", typeof(InbetweenRecursiveElement).GetProperty("Subelement").AssertNotNull()));
    }

    private void AssertTypeVisit (string propertyPath, Type type, params UnorderedCallAssertion[] memberAssertions)
    {
      IOrderableCallAssertion assertion =
          A.CallTo(() => _staticConfigurationVisitor.BeginTypeVisit(propertyPath, type)).MustHaveHappened(Repeated.Exactly.Once);

      assertion = memberAssertions.Aggregate(assertion, (current, memberAssertion) => current.Then(memberAssertion));

      assertion.Then(A.CallTo(() => _staticConfigurationVisitor.EndTypeVisit(propertyPath, type)).MustHaveHappened(Repeated.Exactly.Once));
    }

    private UnorderedCallAssertion AssertValueVisit (string propertyPath, PropertyInfo property)
    {
      return A.CallTo(
            () => _staticConfigurationVisitor.VisitValue(propertyPath, property, property.GetCustomAttribute<ConfigurationValueAttribute>()))
          .MustHaveHappened(Repeated.Exactly.Once);
    }

    private UnorderedCallAssertion AssertCollectionVisit (string propertyPath, PropertyInfo property)
    {
      return A.CallTo(
            () => _staticConfigurationVisitor.VisitCollection(propertyPath, property, property.GetCustomAttribute<ConfigurationCollectionAttribute>()))
          .MustHaveHappened(Repeated.Exactly.Once);
    }

    private UnorderedCallAssertion AssertSubelementVisit (string propertyPath, PropertyInfo property)
    {
      return A.CallTo(
            () => _staticConfigurationVisitor.VisitSubelement(propertyPath, property, property.GetCustomAttribute<ConfigurationSubelementAttribute>()))
          .MustHaveHappened(Repeated.Exactly.Once);
    }

    // ReSharper disable UnusedMember.Local
    private class Element : ConfigurationElement
    {
      [ConfigurationValue]
      public string Value { get; set; }

      [ConfigurationSubelement]
      public SubElement Subelement { get; set; }

      [ConfigurationCollection]
      public IConfigurationElementCollection<CollectionElement> Collection { get; set; }
    }

    private class SubElement : ConfigurationElement
    {
      [ConfigurationValue]
      public string Value { get; set; }

      [ConfigurationSubelement]
      public EmptyElement Subelement { get; set; }

      [ConfigurationCollection]
      public IConfigurationElementCollection<EmptyElement> Collection { get; set; }
    }

    private class CollectionElement : ConfigurationElement
    {
      [ConfigurationValue]
      public string Value { get; set; }

      [ConfigurationSubelement]
      public EmptyElement Subelement { get; set; }

      [ConfigurationCollection]
      public IConfigurationElementCollection<EmptyElement> Collection { get; set; }
    }

    private class EmptyElement : ConfigurationElement
    {
    }

    private class RecursiveElement : ConfigurationElement
    {
      [ConfigurationSubelement]
      public RecursiveElement Recursive { get; set; }
    }

    private class IndirectRecursiveElement : ConfigurationElement
    {
      [ConfigurationSubelement]
      public InbetweenRecursiveElement Subelement { get; set; }
    }

    private class InbetweenRecursiveElement : ConfigurationElement
    {
      [ConfigurationSubelement]
      public IndirectRecursiveElement Subelement { get; set; }
    }

    // ReSharper restore UnusedMember.Local
  }
}