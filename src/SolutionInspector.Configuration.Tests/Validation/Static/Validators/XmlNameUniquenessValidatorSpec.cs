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
  [Subject (typeof(XmlNameUniquenessValidator))]
  class XmlNameUniquenessValidatorSpec
  {
    static XmlNameUniquenessValidator SUT;

    static PropertyInfo PropertyInfo;

    static ReportValidationError ReportValidationError;

    Establish ctx = () =>
    {
      SUT = new XmlNameUniquenessValidator();

      PropertyInfo = A.Fake<TestablePropertyInfo>();
      ReportValidationError = A.Fake<ReportValidationError>();
    };

    class when_validating_type_with_only_unique_names
    {
      Because of = () =>
      {
        var type = Some.Type;
        SUT.BeginTypeValidation(type, ReportValidationError);
        SUT.ValidateValue(PropertyInfo, new ConfigurationValueAttribute { AttributeName = "value" }, ReportValidationError);
        SUT.ValidateSubelement(PropertyInfo, new ConfigurationSubelementAttribute { ElementName = "subelement" }, ReportValidationError);
        SUT.ValidateCollection(PropertyInfo, new ConfigurationCollectionAttribute { CollectionName = "collection" }, ReportValidationError);
        SUT.EndTypeValidation(type, ReportValidationError);
      };

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    class when_validating_type_with_duplicate_attribute_names
    {
      Establish ctx = () =>
      {
        Property1 = A.Fake<TestablePropertyInfo>(o => o.ConfigureFake(info => A.CallTo(() => info.Name).Returns("prop1")));
        Property2 = A.Fake<TestablePropertyInfo>(o => o.ConfigureFake(info => A.CallTo(() => info.Name).Returns("prop2")));
      };

      Because of = () =>
      {
        var type = Some.Type;
        SUT.BeginTypeValidation(type, ReportValidationError);
        SUT.ValidateValue(Property1, new ConfigurationValueAttribute { AttributeName = "duplicate" }, ReportValidationError);
        SUT.ValidateValue(Property2, new ConfigurationValueAttribute { AttributeName = "duplicate" }, ReportValidationError);
        SUT.EndTypeValidation(type, ReportValidationError);
      };

      It reports_error_for_property1 = () =>
        A.CallTo(
          () => ReportValidationError(
            Property1,
            "The XML attribute name 'duplicate' is not unique (it is duplicated in 'prop2').")).MustHaveHappened();

      It reports_error_for_property2 = () =>
        A.CallTo(
          () => ReportValidationError(
            Property2,
            "The XML attribute name 'duplicate' is not unique (it is duplicated in 'prop1').")).MustHaveHappened();

      static TestablePropertyInfo Property1;
      static TestablePropertyInfo Property2;
    }

    class when_validating_type_with_duplicate_element_names
    {
      Establish ctx = () =>
      {
        Property1 = A.Fake<TestablePropertyInfo>(o => o.ConfigureFake(info => A.CallTo(() => info.Name).Returns("prop1")));
        Property2 = A.Fake<TestablePropertyInfo>(o => o.ConfigureFake(info => A.CallTo(() => info.Name).Returns("prop2")));
      };

      Because of = () =>
      {
        var type = Some.Type;
        SUT.BeginTypeValidation(type, ReportValidationError);
        SUT.ValidateSubelement(Property1, new ConfigurationSubelementAttribute { ElementName = "duplicate" }, ReportValidationError);
        SUT.ValidateCollection(Property2, new ConfigurationCollectionAttribute { CollectionName = "duplicate" }, ReportValidationError);
        SUT.EndTypeValidation(type, ReportValidationError);
      };

      It reports_error_for_property1 = () =>
        A.CallTo(
          () => ReportValidationError(
            Property1,
            "The XML element name 'duplicate' is not unique (it is duplicated in 'prop2').")).MustHaveHappened();

      It reports_error_for_property2 = () =>
        A.CallTo(
          () => ReportValidationError(
            Property2,
            "The XML element name 'duplicate' is not unique (it is duplicated in 'prop1').")).MustHaveHappened();

      static TestablePropertyInfo Property1;
      static TestablePropertyInfo Property2;
    }

    class when_validating_type_with_attribute_and_element_name_being_the_same
    {
      Because of = () =>
      {
        var type = Some.Type;
        SUT.BeginTypeValidation(type, ReportValidationError);
        SUT.ValidateValue(PropertyInfo, new ConfigurationValueAttribute { AttributeName = "duplicate" }, ReportValidationError);
        SUT.ValidateSubelement(PropertyInfo, new ConfigurationSubelementAttribute { ElementName = "duplicate" }, ReportValidationError);
        SUT.EndTypeValidation(type, ReportValidationError);
      };

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }
    
    class when_validating_multiple_types_with_same_xml_Names
    {
      Because of = () =>
      {
        var type1 = Some.Type;
        SUT.BeginTypeValidation(type1, ReportValidationError);
        SUT.ValidateValue(PropertyInfo, new ConfigurationValueAttribute { AttributeName = "duplicate" }, ReportValidationError);
        SUT.ValidateSubelement(PropertyInfo, new ConfigurationSubelementAttribute { ElementName = "duplicate2" }, ReportValidationError);
        SUT.EndTypeValidation(type1, ReportValidationError);

        var type2 = Some.Type;
        SUT.BeginTypeValidation(type2, ReportValidationError);
        SUT.ValidateValue(PropertyInfo, new ConfigurationValueAttribute { AttributeName = "duplicate" }, ReportValidationError);
        SUT.ValidateSubelement(PropertyInfo, new ConfigurationSubelementAttribute { ElementName = "duplicate2" }, ReportValidationError);
        SUT.EndTypeValidation(type2, ReportValidationError);
      };

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }
  }
}