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
  [Subject (typeof(ConfigurationSubelementTypeValidator))]
  class ConfigurationSubelementTypeValidatorSpec
  {
    static ConfigurationSubelementTypeValidator SUT;

    static PropertyInfo PropertyInfo;
    static ConfigurationSubelementAttribute Attribute;

    static ReportValidationError ReportValidationError;

    Establish ctx = () =>
    {
      SUT = new ConfigurationSubelementTypeValidator();

      PropertyInfo = A.Fake<TestablePropertyInfo>();
      Attribute = new ConfigurationSubelementAttribute();
      ReportValidationError = A.Fake<ReportValidationError>();
    };

    class when_validating_with_valid_subelement_type
    {
      Establish ctx = () => { A.CallTo(() => PropertyInfo.PropertyType).Returns(typeof(ConfigurationElement)); };

      Because of = () => SUT.ValidateSubelement(PropertyInfo, Attribute, ReportValidationError);

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    class when_validating_with_invalid_subelement_type
    {
      Establish ctx = () => { A.CallTo(() => PropertyInfo.PropertyType).Returns(typeof(object)); };

      Because of = () => SUT.ValidateSubelement(PropertyInfo, Attribute, ReportValidationError);

      It reports_error = () =>
        A.CallTo(
          () => ReportValidationError(
            PropertyInfo,
            $"'{PropertyInfo.PropertyType}' is not a valid type for a configuration sub element, only " +
            $"types derived from '{typeof(ConfigurationElement)}' are allowed.")).MustHaveHappened();
    }
  }
}