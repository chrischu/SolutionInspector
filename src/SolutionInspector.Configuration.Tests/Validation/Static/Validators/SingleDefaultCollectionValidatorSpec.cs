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
  [Subject (typeof(SingleDefaultCollectionValidator))]
  class SingleDefaultCollectionValidatorSpec
  {
    static SingleDefaultCollectionValidator SUT;

    static PropertyInfo PropertyInfo;
    static ConfigurationCollectionAttribute Attribute;

    static ReportValidationError ReportValidationError;

    Establish ctx = () =>
    {
      SUT = new SingleDefaultCollectionValidator();

      PropertyInfo = A.Fake<TestablePropertyInfo>();
      Attribute = new ConfigurationCollectionAttribute();
      ReportValidationError = A.Fake<ReportValidationError>();
    };

    class when_validating_type_with_single_default_collection
    {
      Because of = () =>
      {
        var type = Some.Type;
        SUT.BeginTypeValidation(type, ReportValidationError);
        SUT.ValidateCollection(PropertyInfo, new ConfigurationCollectionAttribute { IsDefaultCollection = true }, ReportValidationError);
        SUT.ValidateCollection(PropertyInfo, new ConfigurationCollectionAttribute { IsDefaultCollection = false }, ReportValidationError);
        SUT.EndTypeValidation(type, ReportValidationError);
      };

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    class when_validating_type_with_multiple_default_collection
    {
      Establish ctx = () =>
      {
        Property1 = A.Fake<TestablePropertyInfo>(o => o.ConfigureFake(info => A.CallTo(() => info.Name).Returns("prop1")));
        Property2 = A.Fake<TestablePropertyInfo>(o => o.ConfigureFake(info => A.CallTo(() => info.Name).Returns("prop2")));
        Property3 = A.Fake<TestablePropertyInfo>(o => o.ConfigureFake(info => A.CallTo(() => info.Name).Returns("prop3")));
      };

      Because of = () =>
      {
        var type = Some.Type;
        SUT.BeginTypeValidation(type, ReportValidationError);

        SUT.ValidateCollection(Property1, new ConfigurationCollectionAttribute { IsDefaultCollection = true }, ReportValidationError);
        SUT.ValidateCollection(Property2, new ConfigurationCollectionAttribute { IsDefaultCollection = true }, ReportValidationError);
        SUT.ValidateCollection(Property3, new ConfigurationCollectionAttribute { IsDefaultCollection = true }, ReportValidationError);

        SUT.EndTypeValidation(type, ReportValidationError);
      };

      It reports_error_for_property1 = () =>
        A.CallTo(
          () => ReportValidationError(
            Property1,
            "There can only be one default collection per configuration element and the following properties are already marked as " +
            "default collection: 'prop2', 'prop3'.")).MustHaveHappened();

      It reports_error_for_property2 = () =>
        A.CallTo(
          () => ReportValidationError(
            Property2,
            "There can only be one default collection per configuration element and the following properties are already marked as " +
            "default collection: 'prop1', 'prop3'.")).MustHaveHappened();

      It reports_error_for_property3 = () =>
        A.CallTo(
          () => ReportValidationError(
            Property3,
            "There can only be one default collection per configuration element and the following properties are already marked as " +
            "default collection: 'prop1', 'prop2'.")).MustHaveHappened();

      static TestablePropertyInfo Property1;
      static TestablePropertyInfo Property2;
      static TestablePropertyInfo Property3;
    }

    class when_validating_multiple_types_each_with_one_default_collection
    {
      Because of = () =>
      {
        var type1 = Some.Type;
        SUT.BeginTypeValidation(type1, ReportValidationError);
        SUT.ValidateCollection(PropertyInfo, new ConfigurationCollectionAttribute { IsDefaultCollection = true }, ReportValidationError);
        SUT.EndTypeValidation(type1, ReportValidationError);

        var type2 = Some.Type;
        SUT.BeginTypeValidation(type2, ReportValidationError);
        SUT.ValidateCollection(PropertyInfo, new ConfigurationCollectionAttribute { IsDefaultCollection = true }, ReportValidationError);
        SUT.EndTypeValidation(type2, ReportValidationError);
      };

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }
  }
}