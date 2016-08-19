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
  [Subject (typeof(DefaultCollectionMustNotSpecifyCollectionNameValidator))]
  class DefaultCollectionMustNotSpecifyCollectionNameValidatorSpec
  {
    static DefaultCollectionMustNotSpecifyCollectionNameValidator SUT;

    static PropertyInfo PropertyInfo;
    static ConfigurationCollectionAttribute Attribute;

    static ReportValidationError ReportValidationError;

    Establish ctx = () =>
    {
      SUT = new DefaultCollectionMustNotSpecifyCollectionNameValidator();

      PropertyInfo = A.Fake<TestablePropertyInfo>();
      Attribute = new ConfigurationCollectionAttribute();
      ReportValidationError = A.Fake<ReportValidationError>();
    };

    class when_validating_non_default_collection_with_collection_name
    {
      Establish ctx = () => { Attribute = new ConfigurationCollectionAttribute { CollectionName = Some.String(), IsDefaultCollection = false }; };

      Because of = () => SUT.ValidateCollection(PropertyInfo, Attribute, ReportValidationError);

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    class when_validating_default_collection_without_collection_name
    {
      Establish ctx = () => { Attribute = new ConfigurationCollectionAttribute { CollectionName = null, IsDefaultCollection = true }; };

      Because of = () => SUT.ValidateCollection(PropertyInfo, Attribute, ReportValidationError);

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    class when_validating_default_collection_with_collection_name
    {
      Establish ctx = () => { Attribute = new ConfigurationCollectionAttribute { CollectionName = Some.String(), IsDefaultCollection = true }; };

      Because of = () => SUT.ValidateCollection(PropertyInfo, Attribute, ReportValidationError);

      It reports_error = () =>
        A.CallTo(
              () => ReportValidationError(
                PropertyInfo,
                "A configuration collection marked as the default collection must not specify a collection name."))
            .MustHaveHappened();
    }
  }
}