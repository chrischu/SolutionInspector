using System;
using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace SolutionInspector.Configuration.Tests
{
  public class ConfigurationElementCollectionTests
  {
    private XElement _collectionElement;
    private string _elementName;

    private DummyConfigurationElementCollection _sut;

    [SetUp]
    public void SetUp ()
    {
      _collectionElement = new XElement("collection");
      _elementName = "element";

      _sut = new DummyConfigurationElementCollection(_collectionElement, _elementName);
    }

    [Test]
    public void AddNew ()
    {
      // ACT
      var result = _sut.AddNew();

      // ASSERT
      result.Element.Name.LocalName.Should().Be(_elementName);
      _collectionElement.Elements().Should().HaveCount(1);
      _sut.Should().HaveCount(1);
    }

    [Test]
    public void Add ()
    {
      var configElement = ConfigurationElement.Create<DummyConfigurationElement>();

      // ACT
      _sut.Add(configElement);

      // ASSERT
      _collectionElement.Elements().Should().HaveCount(1);
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
      _sut.AddNew();

      var configElement = ConfigurationElement.Create<DummyConfigurationElement>();

      // ACT
      _sut.Insert(0, configElement);

      // ASSERT
      _collectionElement.Elements().Should().HaveCount(2);
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
      var element = _sut.AddNew();

      // ACT
      _sut.Remove(element);

      // ASSERT
      _collectionElement.Elements().Should().HaveCount(0);
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
      _sut.AddNew();
      var element2 = _sut.AddNew();

      // ACT
      _sut.RemoveAt(0);

      // ASSERT
      _collectionElement.Elements().Should().HaveCount(1);
      _sut.Should().HaveCount(1);
      _sut[0].Should().BeSameAs(element2);
    }

    [Test]
    public void Clear ()
    {
      _sut.AddNew();
      _sut.AddNew();

      // ACT
      _sut.Clear();

      // ASSERT
      _collectionElement.Elements().Should().HaveCount(0);
      _sut.Should().HaveCount(0);
    }

    [Test]
    public void GetEnumerator ()
    {
      _sut.AddNew();
      _sut.AddNew();

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
      var a = _sut.AddNew();
      var b = _sut.AddNew();

      // ACT & ASSERT
      _sut[0].Should().BeSameAs(a);
      _sut[1].Should().BeSameAs(b);
    }

    [Test]
    public void ItemSet()
    {
      _sut.AddNew();
      var newElement = new DummyConfigurationElement(); 

      // ACT
      _sut[0] = newElement;

      // ASSERT
      _sut[0].Should().BeSameAs(newElement);
    }

    private class DummyConfigurationElementCollection : ConfigurationElementCollection<DummyConfigurationElement>
    {
      public DummyConfigurationElementCollection (XElement collectionElement, string elementName)
        : base(collectionElement, elementName)
      {
      }
    }

    private class DummyConfigurationElement : ConfigurationElement
    {
    }
  }
}