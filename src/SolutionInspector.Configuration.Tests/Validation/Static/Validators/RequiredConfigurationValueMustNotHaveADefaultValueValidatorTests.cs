using System;
using System.Reflection;
using FakeItEasy;
using JetBrains.Annotations;
using NUnit.Framework;
using SolutionInspector.Configuration.Attributes;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.Configuration.Validation.Static.Validators;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Configuration.Tests.Validation.Static.Validators
{
  public class RequiredConfigurationValueMustNotHaveADefaultValueValidatorTests
  {
    private PropertyInfo _propertyInfo;
    private ReportValidationError _reportValidationError;
    private RequiredConfigurationValueMustNotHaveADefaultValueValidator _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new RequiredConfigurationValueMustNotHaveADefaultValueValidator();

      _propertyInfo = A.Fake<TestablePropertyInfo>();
      _reportValidationError = A.Fake<ReportValidationError>();
    }

    [Test]
    [TestCase(true, "", true, TestName = "Required_WithDefaultValue")]
    [TestCase(true, null, false, TestName = "Required_WithoutDefaultValue")]
    [TestCase(false, "", false, TestName = "Optional_WithDefaultValue")]
    [TestCase(false, null, false, TestName = "Optional_WithoutDefaultValue")]
    public void ValidateValue (bool isRequired, [CanBeNull] string defaultValue, bool reportsError)
    {
      // ACT
      _sut.ValidateValue(_propertyInfo, new ConfigurationValueAttribute { IsOptional = !isRequired, DefaultValue = defaultValue }, _reportValidationError);

      // ASSERT
      if (reportsError)
        A.CallTo(() => _reportValidationError(_propertyInfo, "A required property must not have a default value.")).MustHaveHappened();
      else
        A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }
  }
}