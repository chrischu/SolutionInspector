using System.Reflection;
using System.Xml.Linq;
using FakeItEasy;
using Machine.Specifications;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.Configuration.Validation.Dynamic.Validators;
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

namespace SolutionInspector.Configuration.Tests.Validation.Dynamic.Validators
{
  [Subject (typeof(RequiredValidator))]
  class RequiredValidatorSpec
  {
    static RequiredValidator SUT;

    static PropertyInfo PropertyInfo;

    static ReportValidationError ReportValidationError;

    Establish ctx = () =>
    {
      SUT = new RequiredValidator();

      PropertyInfo = A.Fake<TestablePropertyInfo>();
      ReportValidationError = A.Fake<ReportValidationError>();
    };

    class when_validating_optional_value_with_missing_attribute
    {
      Establish ctx = () => { Attribute = new ConfigurationValueAttribute { IsOptional = true }; };

      Because of = () => SUT.ValidateValue(PropertyInfo, Attribute, null, ReportValidationError);

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();

      static ConfigurationValueAttribute Attribute;
    }

    class when_validating_required_value_with_existing_attribute
    {
      Establish ctx = () => { Attribute = new ConfigurationValueAttribute { IsOptional = false }; };

      Because of = () => SUT.ValidateValue(PropertyInfo, Attribute, new XAttribute("DoNotCare", "DoNotCare"), ReportValidationError);

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();

      static ConfigurationValueAttribute Attribute;
    }

    class when_validating_required_value_with_missing_attribute
    {
      Establish ctx = () => { Attribute = new ConfigurationValueAttribute { AttributeName = "attribute", IsOptional = false }; };

      Because of = () => SUT.ValidateValue(PropertyInfo, Attribute, null, ReportValidationError);

      It reports_error = () =>
          A.CallTo(
            () => ReportValidationError(
              PropertyInfo,
              "The value is required but no corresponding attribute with the name 'attribute' could be found.")).MustHaveHappened();

      static ConfigurationValueAttribute Attribute;
    }

    class when_validating_optional_subelement_with_missing_element
    {
      Establish ctx = () => { Attribute = new ConfigurationSubelementAttribute { IsOptional = true }; };

      Because of = () => SUT.ValidateSubelement(PropertyInfo, Attribute, null, ReportValidationError);

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();

      static ConfigurationSubelementAttribute Attribute;
    }

    class when_validating_required_subelement_with_existing_element
    {
      Establish ctx = () => { Attribute = new ConfigurationSubelementAttribute { IsOptional = false }; };

      Because of = () => SUT.ValidateSubelement(PropertyInfo, Attribute, new XElement("DoNotCare"), ReportValidationError);

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();

      static ConfigurationSubelementAttribute Attribute;
    }

    class when_validating_required_subelement_with_missing_element
    {
      Establish ctx = () => { Attribute = new ConfigurationSubelementAttribute { ElementName = "element", IsOptional = false }; };

      Because of = () => SUT.ValidateSubelement(PropertyInfo, Attribute, null, ReportValidationError);

      It reports_error = () =>
          A.CallTo(
            () => ReportValidationError(
              PropertyInfo,
              "The subelement is required but no corresponding element with the name 'element' could be found.")).MustHaveHappened();

      static ConfigurationSubelementAttribute Attribute;
    }

    class when_validating_optional_collection_with_missing_element
    {
      Establish ctx = () => { Attribute = new ConfigurationCollectionAttribute { IsOptional = true }; };

      Because of = () => SUT.ValidateCollection(PropertyInfo, Attribute, null, null, ReportValidationError);

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();

      static ConfigurationCollectionAttribute Attribute;
    }

    class when_validating_required_collection_with_existing_element
    {
      Establish ctx = () => { Attribute = new ConfigurationCollectionAttribute { IsOptional = false }; };

      Because of = () => SUT.ValidateCollection(PropertyInfo, Attribute, new XElement("DoNotCare"), null, ReportValidationError);

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();

      static ConfigurationCollectionAttribute Attribute;
    }

    class when_validating_required_collection_with_missing_element
    {
      Establish ctx = () => { Attribute = new ConfigurationCollectionAttribute { CollectionName = "element", IsOptional = false }; };

      Because of = () => SUT.ValidateCollection(PropertyInfo, Attribute, null, null, ReportValidationError);

      It reports_error = () =>
          A.CallTo(
            () => ReportValidationError(
              PropertyInfo,
              "The collection is required but no corresponding element with the name 'element' could be found.")).MustHaveHappened();

      static ConfigurationCollectionAttribute Attribute;
    }
  }
}