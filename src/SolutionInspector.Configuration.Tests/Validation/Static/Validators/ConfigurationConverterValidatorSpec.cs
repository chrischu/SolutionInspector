using System;
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
  [Subject (typeof(ConfigurationConverterValidator))]
  class ConfigurationConverterValidatorSpec
  {
    static ConfigurationConverterValidator SUT;

    static PropertyInfo PropertyInfo;
    static ConfigurationValueAttribute Attribute;

    static ReportValidationError ReportValidationError;

    Establish ctx = () =>
    {
      SUT = new ConfigurationConverterValidator();

      PropertyInfo = A.Fake<TestablePropertyInfo>();
      Attribute = new ConfigurationValueAttribute();
      ReportValidationError = A.Fake<ReportValidationError>();
    };

    class when_validating_with_valid_converter_type
    {
      Establish ctx = () =>
      {
        Attribute = new ConfigurationValueAttribute { ConfigurationConverter = typeof(IntConverter) };
        A.CallTo(() => PropertyInfo.PropertyType).Returns(typeof(int));
      };

      Because of = () => SUT.ValidateValue(PropertyInfo, Attribute, ReportValidationError);

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    class when_validating_with_converter_type_with_invalid_conversion_type
    {
      Establish ctx = () =>
      {
        Attribute = new ConfigurationValueAttribute { ConfigurationConverter = typeof(StringConverter) };
        A.CallTo(() => PropertyInfo.PropertyType).Returns(typeof(int));
      };

      Because of = () => SUT.ValidateValue(PropertyInfo, Attribute, ReportValidationError);

      It reports_error = () =>
        A.CallTo(
          () => ReportValidationError(
            PropertyInfo,
            $"The type '{Attribute.ConfigurationConverter}' is not a valid configuration converter " +
            $"type for a property of type '{PropertyInfo.PropertyType}'.")).MustHaveHappened();
    }

    class when_validating_with_invalid_converter_type
    {
      Establish ctx = () =>
      {
        Attribute = new ConfigurationValueAttribute { ConfigurationConverter = typeof(object) };
      };

      Because of = () => SUT.ValidateValue(PropertyInfo, Attribute, ReportValidationError);

      It reports_error = () =>
        A.CallTo(
          () => ReportValidationError(
            PropertyInfo,
            $"The type '{Attribute.ConfigurationConverter}' is not a valid configuration converter " +
            $"type for a property of type '{PropertyInfo.PropertyType}'.")).MustHaveHappened();
    }

    class IntConverter : IConfigurationConverter<int>
    {
      public string ConvertTo (int value)
      {
        throw new NotImplementedException();
      }

      public int ConvertFrom (string value)
      {
        throw new NotImplementedException();
      }
    }

    class StringConverter : IConfigurationConverter<string>
    {
      public string ConvertTo (string value)
      {
        throw new NotImplementedException();
      }

      public string ConvertFrom (string value)
      {
        throw new NotImplementedException();
      }
    }
  }
}