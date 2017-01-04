using System;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FakeItEasy.Configuration;
using NUnit.Framework;
using SolutionInspector.Configuration.Validation.Static;

namespace SolutionInspector.Configuration.Tests.Validation.Static
{
  public class StaticConfigurationTypeWalkerTests
  {
    private StaticConfigurationTypeWalker _sut;

    private IStaticConfigurationVisitor _staticConfigurationVisitor;

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
        AssertValueVisit("Value", typeof(Element).GetProperty("Value")),
        AssertSubelementVisit("Subelement", typeof(Element).GetProperty("Subelement")),
        AssertCollectionVisit("Collection", typeof(Element).GetProperty("Collection"))
      );

      AssertTypeVisit(
        "Subelement",
        typeof(SubElement),
        AssertValueVisit("Subelement.Value", typeof(SubElement).GetProperty("Value")),
        AssertSubelementVisit("Subelement.Subelement", typeof(SubElement).GetProperty("Subelement")),
        AssertCollectionVisit("Subelement.Collection", typeof(SubElement).GetProperty("Collection"))
      );

      AssertTypeVisit(
        "Collection",
        typeof(CollectionElement),
        AssertValueVisit("Collection.Value", typeof(CollectionElement).GetProperty("Value")),
        AssertSubelementVisit("Collection.Subelement", typeof(CollectionElement).GetProperty("Subelement")),
        AssertCollectionVisit("Collection.Collection", typeof(CollectionElement).GetProperty("Collection"))
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
        AssertSubelementVisit("Recursive", typeof(RecursiveElement).GetProperty("Recursive")));
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
        AssertSubelementVisit("Subelement", typeof(IndirectRecursiveElement).GetProperty("Subelement")));

      AssertTypeVisit(
        "Subelement",
        typeof(InbetweenRecursiveElement),
        AssertSubelementVisit("Subelement.Subelement", typeof(InbetweenRecursiveElement).GetProperty("Subelement")));
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

    // ReSharper restore UnusedMember.Local
  }
}