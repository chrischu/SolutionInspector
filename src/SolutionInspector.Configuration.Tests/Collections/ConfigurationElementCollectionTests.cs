using System;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Configuration.Collections;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

namespace SolutionInspector.Configuration.Tests.Collections
{
  public class ConfigurationElementCollectionTests : ConfigurationElementCollectionTestsBase
  {
    private string _elementName;

    private ConfigurationElementCollection<DummyConfigurationElement> _sut;

    [SetUp]
    public new void SetUp ()
    {
      _elementName = "element";

      _sut = new ConfigurationElementCollection<DummyConfigurationElement>(CollectionElement, _elementName);
    }

    [Test]
    public void AddNew ()
    {
      // ACT
      var result = _sut.AddNew();

      // ASSERT
      result.Element.Name.LocalName.Should().Be(_elementName);
      CollectionElement.Elements().Should().HaveCount(1);
      _sut.Should().HaveCount(1);
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
    public void Insert ()
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
    [TestCaseSource(nameof(ValidateNewElementTestCaseSource))]
    public void ValidateNewElement (ValidateNewElementTestCase testCase)
    {
      _sut.Add(ConfigurationElement.Create<DummyConfigurationElement>());

      var configElement = ConfigurationElement.Create<DummyConfigurationElement>(elementName: "INVALID");

      // ACT
      Action act = () => testCase.Execute(_sut, configElement);

      // ASSERT
      _sut.Should().HaveCount(1);

      act.ShouldThrowArgumentException(
          $"The given element is not compatible with this collection since it has an invalid name 'INVALID' and only elements named '{_elementName}' are allowed.",
          "element");
    }
  }
}