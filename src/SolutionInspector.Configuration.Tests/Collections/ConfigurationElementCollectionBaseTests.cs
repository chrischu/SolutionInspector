using System;
using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Configuration.Collections;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Configuration.Tests.Collections
{
  public class ConfigurationElementCollectionBaseTests : ConfigurationElementCollectionTestsBase
  {
    private ConfigurationElementCollectionBase<DummyConfigurationElement> _sut;

    [SetUp]
    public new void SetUp ()
    {
      _sut = new DummyConfigurationElementCollection(CollectionElement);
    }

    [Test]
    public void Add ()
    {
      var configElement = ConfigurationElement.Create<DummyConfigurationElement>();

      // ACT
      _sut.Add(configElement);

      // ASSERT
      CollectionElement.Elements().Should().HaveCount(1);
      _sut.Should().HaveCount(1);
    }

    [Test]
    public void Add_Null_Throws ()
    {
      // ACT
      Action act = () => _sut.Add(null);

      // ASSERT
      act.ShouldThrow<ArgumentNullException>();
    }

    [Test]
    public void Insert()
    {
      _sut.Add(ConfigurationElement.Create<DummyConfigurationElement>());

      var configElement = ConfigurationElement.Create<DummyConfigurationElement>();

      // ACT
      _sut.Insert(0, configElement);

      // ASSERT
      CollectionElement.Elements().Should().HaveCount(2);
      _sut.Should().HaveCount(2);
      _sut[0].Should().BeSameAs(configElement);
    }

    [Test]
    public void Insert_Null_Throws ()
    {
      // ACT
      Action act = () => _sut.Insert(0, null);

      // ASSERT
      act.ShouldThrow<ArgumentNullException>();
    }

    [Test]
    public void Remove ()
    {
      var element = ConfigurationElement.Create<DummyConfigurationElement>();
      _sut.Add(element);

      // ACT
      _sut.Remove(element);

      // ASSERT
      CollectionElement.Elements().Should().HaveCount(0);
      _sut.Should().HaveCount(0);
    }

    [Test]
    public void Remove_Null_Throws ()
    {
      // ACT
      Action act = () => _sut.Remove(null);

      // ASSERT
      act.ShouldThrow<ArgumentNullException>();
    }

    [Test]
    public void RemoveAt()
    {
      _sut.Add(ConfigurationElement.Create<DummyConfigurationElement>());
      var element2 = ConfigurationElement.Create<DummyConfigurationElement>();
      _sut.Add(element2);

      // ACT
      _sut.RemoveAt(0);

      // ASSERT
      CollectionElement.Elements().Should().HaveCount(1);
      _sut.Should().HaveCount(1);
      _sut[0].Should().BeSameAs(element2);
    }

    [Test]
    public void Clear ()
    {
      _sut.Add(ConfigurationElement.Create<DummyConfigurationElement>());
      _sut.Add(ConfigurationElement.Create<DummyConfigurationElement>());

      // ACT
      _sut.Clear();

      // ASSERT
      CollectionElement.Elements().Should().HaveCount(0);
      _sut.Should().HaveCount(0);
    }

    [Test]
    public void GetEnumerator ()
    {
      _sut.Add(ConfigurationElement.Create<DummyConfigurationElement>());
      _sut.Add(ConfigurationElement.Create<DummyConfigurationElement>());

      // ACT
      using (var result = _sut.GetEnumerator())
      {
        // ASSERT
        result.MoveNext().Should().BeTrue();
        result.MoveNext().Should().BeTrue();
        result.MoveNext().Should().BeFalse();
      }
    }

    [Test]
    public void ItemGet ()
    {
      var element1 = ConfigurationElement.Create<DummyConfigurationElement>();
      _sut.Add(element1);
      var element2 = ConfigurationElement.Create<DummyConfigurationElement>();
      _sut.Add(element2);

      // ACT & ASSERT
      _sut[0].Should().BeSameAs(element1);
      _sut[1].Should().BeSameAs(element2);
    }

    [Test]
    public void ItemSet()
    {
      _sut.Add(ConfigurationElement.Create<DummyConfigurationElement>());
      var newElement = ConfigurationElement.Create<DummyConfigurationElement>(); 

      // ACT
      _sut[0] = newElement;

      // ASSERT
      _sut[0].Should().BeSameAs(newElement);
    }

    [Test]
    [TestCaseSource(nameof(ValidateNewElementTestCaseSource))]
    public void ValidateNewElement (ValidateNewElementTestCase testCase)
    {
      var thrownException = Some.Exception;
      var sut = new ThrowingConfigurationElementCollection(CollectionElement, thrownException) { ConfigurationElement.Create<DummyConfigurationElement>() };

      var configElement = ConfigurationElement.Create<DummyConfigurationElement>(elementName: "INVALID");

      sut.Throw = true;

      // ACT
      Action act = () => testCase.Execute(sut, configElement);

      // ASSERT
      sut.Should().HaveCount(1);

      act.ShouldThrow<Exception>().Which.Should().BeSameAs(thrownException);
    }

    private class DummyConfigurationElementCollection : ConfigurationElementCollectionBase<DummyConfigurationElement>
    {
      public DummyConfigurationElementCollection (XElement collectionElement)
          : base(collectionElement, Enumerable.Empty<DummyConfigurationElement>())
      {
      }
    }

    private class ThrowingConfigurationElementCollection : ConfigurationElementCollectionBase<DummyConfigurationElement>
    {
      private readonly Exception _exception;

      public ThrowingConfigurationElementCollection (XElement collectionElement, Exception exception)
          : base(collectionElement, Enumerable.Empty<DummyConfigurationElement>())
      {
        _exception = exception;
      }

      public bool Throw { private get; set; }

      protected override void ValidateNewElement (DummyConfigurationElement element)
      {
        if (Throw)
          throw _exception;
      }
    }
  }
}