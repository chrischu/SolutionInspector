using System.Reflection;
using FakeItEasy;
using NUnit.Framework;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.Configuration.Validation.Static.Validators;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Configuration.Tests.Validation.Static.Validators
{
  class ConfigurationSubelementTypeValidatorTests
  {
    private ConfigurationSubelementTypeValidator _sut;

    private PropertyInfo _propertyInfo;
    private ReportValidationError _reportValidationError;

    [SetUp]
    public void SetUp ()
    {
      _sut = new ConfigurationSubelementTypeValidator();

      _propertyInfo = A.Fake<TestablePropertyInfo>();
      _reportValidationError = A.Fake<ReportValidationError>();
    }

    [Test]
    public void ValidateSubElement_WithValidSubelementType_DoesNotReportError ()
    {
      A.CallTo(() => _propertyInfo.PropertyType).Returns(typeof(ConfigurationElement));

      // ACT
      _sut.ValidateSubelement(_propertyInfo, new ConfigurationSubelementAttribute(), _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateSubElement_WithInvalidSubelementType_ReportsError ()
    {
      A.CallTo(() => _propertyInfo.PropertyType).Returns(typeof(object));

      // ACT
      _sut.ValidateSubelement(_propertyInfo, new ConfigurationSubelementAttribute(), _reportValidationError);

      // ASSERT
      A.CallTo(
        () => _reportValidationError(
          _propertyInfo,
          $"'{_propertyInfo.PropertyType}' is not a valid type for a configuration sub element, only " +
          $"types derived from '{typeof(ConfigurationElement)}' are allowed.")).MustHaveHappened();
    }
  }
}