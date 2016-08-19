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
  [Subject (typeof(ConfigurationCollectionTypeValidator))]
  class ConfigurationCollectionTypeValidatorSpec
  {
    static ConfigurationCollectionTypeValidator SUT;

    static PropertyInfo PropertyInfo;
    static ConfigurationCollectionAttribute Attribute;

    static ReportValidationError ReportValidationError;

    Establish ctx = () =>
    {
      SUT = new ConfigurationCollectionTypeValidator();

      PropertyInfo = A.Fake<TestablePropertyInfo>();
      Attribute = new ConfigurationCollectionAttribute();
      ReportValidationError = A.Fake<ReportValidationError>();
    };

    class when_validating_with_valid_collection_type
    {
      Establish ctx = () => { A.CallTo(() => PropertyInfo.PropertyType).Returns(typeof(ConfigurationElementCollection<DummyConfigurationElement>)); };

      Because of = () => SUT.ValidateCollection(PropertyInfo, Attribute, ReportValidationError);

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    class when_validating_with_invalid_collection_type
    {
      Establish ctx = () => { A.CallTo(() => PropertyInfo.PropertyType).Returns(typeof(object)); };

      Because of = () => SUT.ValidateCollection(PropertyInfo, Attribute, ReportValidationError);

      It reports_error = () =>
        A.CallTo(
          () => ReportValidationError(
            PropertyInfo,
            $"'{PropertyInfo.PropertyType}' is not a valid type for a configuration collection, only " +
            $"'{typeof(ConfigurationElementCollection<>)}' is allowed.")).MustHaveHappened();
    }

    class DummyConfigurationElement : ConfigurationElement { }
  }
}