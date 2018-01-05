using System.Reflection;
using FakeItEasy;
using NUnit.Framework;
using SolutionInspector.Configuration.Attributes;
using SolutionInspector.Configuration.Collections;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.Configuration.Validation.Static.Validators;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Configuration.Tests.Validation.Static.Validators
{
  public class ConfigurationCollectionTypeValidatorTests
  {
    private PropertyInfo _propertyInfo;
    private ReportValidationError _reportValidationError;
    private ConfigurationCollectionTypeValidator _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new ConfigurationCollectionTypeValidator();

      _propertyInfo = A.Fake<TestablePropertyInfo>();
      _reportValidationError = A.Fake<ReportValidationError>();
    }

    [Test]
    public void ValidateCollection_WithValidCollectionType_DoesNotReportError ()
    {
      A.CallTo(() => _propertyInfo.PropertyType).Returns(typeof(IConfigurationElementCollection<DummyConfigurationElement>));

      // ACT
      _sut.ValidateCollection(_propertyInfo, new ConfigurationCollectionAttribute(), _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateCollection_WithInvalidCollectionType_ReportsError ()
    {
      A.CallTo(() => _propertyInfo.PropertyType).Returns(typeof(object));

      // ACT
      _sut.ValidateCollection(_propertyInfo, new ConfigurationCollectionAttribute(), _reportValidationError);

      // ASSERT
      A.CallTo(
        () => _reportValidationError(
          _propertyInfo,
          $"'{_propertyInfo.PropertyType}' is not a valid type for a configuration collection, only " +
          $"'{typeof(IConfigurationElementCollection<>)}' is allowed.")).MustHaveHappened();
    }

    private class DummyConfigurationElement : ConfigurationElement
    {
    }
  }
}