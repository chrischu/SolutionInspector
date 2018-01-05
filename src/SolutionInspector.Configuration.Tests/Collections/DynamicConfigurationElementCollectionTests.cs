using System;
using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Configuration.Collections;
using SolutionInspector.Configuration.Dynamic;
using SolutionInspector.TestInfrastructure;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

namespace SolutionInspector.Configuration.Tests.Collections
{
  public class DynamicConfigurationElementCollectionTests : ConfigurationElementCollectionTestsBase
  {
    private IDynamicConfigurationElementTypeHelper _dynamicConfigurationElementTypeHelper;

    private IDynamicConfigurationElementCollection<DummyConfigurationElement> _sut;

    [SetUp]
    public new void SetUp ()
    {
      _sut = new DynamicConfigurationElementCollection<DummyConfigurationElement>(CollectionElement);

      _dynamicConfigurationElementTypeHelper = A.Fake<IDynamicConfigurationElementTypeHelper>();
      ConfigurationBase.DynamicConfigurationElementTypeHelper = _dynamicConfigurationElementTypeHelper;
    }

    [Test]
    [TestCaseSource(nameof(ValidateNewElementTestCaseSource))]
    public void ValidateNewElement (ValidateNewElementTestCase testCase)
    {
      _sut.Add(ConfigurationElement.Create<DummyConfigurationElement>());

      var thrownException = new DynamicElementTypeCompatibilityException(Some.String);
      A.CallTo(() => _dynamicConfigurationElementTypeHelper.ValidateElementCompatibility(A<XDocument>._, A<ConfigurationElement>._)).Throws(thrownException);

      var configElement = ConfigurationElement.Create<DummyConfigurationElement>();

      // ACT
      Action act = () => testCase.Execute(_sut, configElement);

      // ASSERT
      _sut.Should().HaveCount(1);

      act.ShouldThrowArgumentException(
          "The given element is not compatible with this collection (see inner exception for details).",
          "element",
          thrownException);

      A.CallTo(() => _dynamicConfigurationElementTypeHelper.ValidateElementCompatibility(Document, configElement)).MustHaveHappened();
    }
  }
}