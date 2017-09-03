using System.Reflection;
using FakeItEasy;
using JetBrains.Annotations;
using NUnit.Framework;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.Configuration.Validation.Static.Validators;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Configuration.Tests.Validation.Static.Validators
{
  public class ConfigurationConverterValidatorTests
  {
    private PropertyInfo _propertyInfo;
    private ReportValidationError _reportValidationError;
    private ConfigurationConverterValidator _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new ConfigurationConverterValidator();

      _propertyInfo = A.Fake<TestablePropertyInfo>();
      _reportValidationError = A.Fake<ReportValidationError>();
    }

    [Test]
    public void ValidateValue_WithValidConverterType_DoesNotReportError ()
    {
      var attribute = new ConfigurationValueAttribute { ConfigurationConverter = typeof(IntConverter) };
      A.CallTo(() => _propertyInfo.PropertyType).Returns(typeof(int));

      // ACT
      _sut.ValidateValue(_propertyInfo, attribute, _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateValue_WithConverterTypeWithInvalidConversionType_DoesNotReportError ()
    {
      var attribute = new ConfigurationValueAttribute { ConfigurationConverter = typeof(StringConverter) };
      A.CallTo(() => _propertyInfo.PropertyType).Returns(typeof(int));

      // ACT
      _sut.ValidateValue(_propertyInfo, attribute, _reportValidationError);

      // ASSERT
      A.CallTo(
        () => _reportValidationError(
          _propertyInfo,
          $"The type '{attribute.ConfigurationConverter}' is not a valid configuration converter " +
          $"type for a property of type '{_propertyInfo.PropertyType}'.")).MustHaveHappened();
    }

    [Test]
    public void ValidateValue_WithInvalidConverterType_DoesNotReportError ()
    {
      var attribute = new ConfigurationValueAttribute { ConfigurationConverter = typeof(object) };

      // ACT
      _sut.ValidateValue(_propertyInfo, attribute, _reportValidationError);

      // ASSERT
      A.CallTo(
        () => _reportValidationError(
          _propertyInfo,
          $"The type '{attribute.ConfigurationConverter}' is not a valid configuration converter " +
          $"type for a property of type '{_propertyInfo.PropertyType}'.")).MustHaveHappened();
    }

    private class IntConverter : IConfigurationConverter<int>
    {
      public string ConvertTo (int value) => "";
      public int ConvertFrom ([CanBeNull] string value) => 0;
    }

    private class StringConverter : IConfigurationConverter<string>
    {
      public string ConvertTo ([CanBeNull] string value) => "";
      public string ConvertFrom ([CanBeNull] string value) => "";
    }
  }
}