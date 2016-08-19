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
  [Subject (typeof(XmlNameValidator))]
  class XmlNameValidatorSpec
  {
    static XmlNameValidator SUT;

    static PropertyInfo PropertyInfo;

    static ReportValidationError ReportValidationError;

    Establish ctx = () =>
    {
      SUT = new XmlNameValidator();

      PropertyInfo = A.Fake<TestablePropertyInfo>();
      ReportValidationError = A.Fake<ReportValidationError>();
    };

    class when_validating_valid_value_name
    {
      Because of = () => SUT.ValidateValue(PropertyInfo, new ConfigurationValueAttribute { AttributeName = "valid" }, ReportValidationError);

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    class when_validating_invalid_value_name
    {
      Because of = () => SUT.ValidateValue(PropertyInfo, new ConfigurationValueAttribute { AttributeName = "in@valid" }, ReportValidationError);

      It reports_error = () =>
        A.CallTo(
          () => ReportValidationError(
            PropertyInfo,
            "'in@valid' is not a valid XML name.")).MustHaveHappened();
    }

    class when_validating_valid_subelement_name
    {
      Because of = () => SUT.ValidateSubelement(PropertyInfo, new ConfigurationSubelementAttribute { ElementName = "valid" }, ReportValidationError);

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    class when_validating_invalid_subelement_name
    {
      Because of =
          () => SUT.ValidateSubelement(PropertyInfo, new ConfigurationSubelementAttribute { ElementName = "in@valid" }, ReportValidationError);

      It reports_error = () =>
        A.CallTo(
          () => ReportValidationError(
            PropertyInfo,
            "'in@valid' is not a valid XML name.")).MustHaveHappened();
    }

    class when_validating_valid_collection_names
    {
      Because of =
          () =>
            SUT.ValidateCollection(
              PropertyInfo,
              new ConfigurationCollectionAttribute { CollectionName = "valid", ElementName = "valid" },
              ReportValidationError);

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    class when_validating_invalid_collection_names
    {
      Because of =
          () =>
            SUT.ValidateCollection(
              PropertyInfo,
              new ConfigurationCollectionAttribute { CollectionName = "in@validCollection", ElementName = "in@validElement" },
              ReportValidationError);

      It reports_error_for_collection_name = () =>
        A.CallTo(
          () => ReportValidationError(
            PropertyInfo,
            "'in@validCollection' is not a valid XML name.")).MustHaveHappened();

      It reports_error_for_element_name = () =>
        A.CallTo(
          () => ReportValidationError(
            PropertyInfo,
            "'in@validElement' is not a valid XML name.")).MustHaveHappened();
    }
  }
}