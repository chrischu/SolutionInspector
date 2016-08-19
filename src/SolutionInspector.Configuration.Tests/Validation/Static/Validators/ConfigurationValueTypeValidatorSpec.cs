using System.Reflection;
using FakeItEasy;
using Machine.Specifications;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.Configuration.Validation.Static.Validators;
using SolutionInspector.TestInfrastructure;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeModifiers
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.Configuration.Tests.Validation.Static.Validators
{
  [Subject (typeof(ConfigurationValueTypeValidator))]
  class ConfigurationValueTypeValidatorSpec
  {
    static ConfigurationValueTypeValidator SUT;

    static PropertyInfo PropertyInfo;
    static ConfigurationValueAttribute Attribute;

    static ReportValidationError ReportValidationError;

    Establish ctx = () =>
    {
      SUT = new ConfigurationValueTypeValidator();

      PropertyInfo = A.Fake<TestablePropertyInfo>();
      Attribute = new ConfigurationValueAttribute();
      ReportValidationError = A.Fake<ReportValidationError>();
    };

    class when_validating_with_valid_primitive_type
    {
      Establish ctx = () => { A.CallTo(() => PropertyInfo.PropertyType).Returns(typeof(int)); };

      Because of = () => SUT.ValidateValue(PropertyInfo, Attribute, ReportValidationError);

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    class when_validating_with_string
    {
      Establish ctx = () => { A.CallTo(() => PropertyInfo.PropertyType).Returns(typeof(string)); };

      Because of = () => SUT.ValidateValue(PropertyInfo, Attribute, ReportValidationError);

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    class when_validating_with_IConfigurationValue
    {
      Establish ctx = () => { A.CallTo(() => PropertyInfo.PropertyType).Returns(typeof(IConfigurationValue)); };

      Because of = () => SUT.ValidateValue(PropertyInfo, Attribute, ReportValidationError);

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }
    class when_validating_with_invalid_value_type
    {
      Establish ctx = () => { A.CallTo(() => PropertyInfo.PropertyType).Returns(typeof(object)); };

      Because of = () => SUT.ValidateValue(PropertyInfo, Attribute, ReportValidationError);

      It reports_error = () =>
        A.CallTo(
              () => ReportValidationError(
                PropertyInfo,
                $"'{PropertyInfo.PropertyType}' is not a valid type for a configuration value, " +
                $"only primitives (e.g. 'int', 'double'), 'string' and types deriving from '{typeof(IConfigurationValue)}' are allowed."))
            .MustHaveHappened();
    }
  }
}