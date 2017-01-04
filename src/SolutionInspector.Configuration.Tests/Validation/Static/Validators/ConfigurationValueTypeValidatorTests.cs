using System;
using System.Reflection;
using FakeItEasy;
using NUnit.Framework;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.Configuration.Validation.Static.Validators;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Configuration.Tests.Validation.Static.Validators
{
  public class ConfigurationValueTypeValidatorTests
  {
    private ConfigurationValueTypeValidator _sut;

    private PropertyInfo _propertyInfo;
    private ReportValidationError _reportValidationError;

    [SetUp]
    public void SetUp ()
    {
      _sut = new ConfigurationValueTypeValidator();

      _propertyInfo = A.Fake<TestablePropertyInfo>();
      _reportValidationError = A.Fake<ReportValidationError>();
    }

    [Test]
    [TestCase(typeof(int))]
    [TestCase(typeof(string))]
    [TestCase(typeof(IConfigurationValue))]
    public void ValidateValue_WithValidType_DoesNotReportError (Type type)
    {
      A.CallTo(() => _propertyInfo.PropertyType).Returns(type);

      // ACT
      _sut.ValidateValue(_propertyInfo, new ConfigurationValueAttribute(), _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateValue_WithInvalidType_ReportsError ()
    {
      A.CallTo(() => _propertyInfo.PropertyType).Returns(typeof(object));

      // ACT
      _sut.ValidateValue(_propertyInfo, new ConfigurationValueAttribute(), _reportValidationError);

      // ASSERT
      A.CallTo(
            () => _reportValidationError(
              _propertyInfo,
              $"'{_propertyInfo.PropertyType}' is not a valid type for a configuration value, " +
              $"only primitives (e.g. 'int', 'double'), 'string' and types deriving from '{typeof(IConfigurationValue)}' are allowed."))
          .MustHaveHappened();
    }
  }
}