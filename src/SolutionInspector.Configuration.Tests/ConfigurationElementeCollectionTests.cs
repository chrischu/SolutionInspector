using System.Collections;
using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace SolutionInspector.Configuration.Tests
{
  public class ConfigurationElementeCollectionTests
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
      var configElement = ConfigurationElement.Create<DummyConfigurationElement>("element");

      // ACT
      _sut.Add(configElement);

      // ASSERT
      _collectionElement.Elements().Should().HaveCount(1);
      _sut.Should().HaveCount(1);
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