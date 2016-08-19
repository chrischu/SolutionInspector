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
  [Subject (typeof(CollectionElementCountValidator))]
  class CollectionElementCountValidatorSpec
  {
    static CollectionElementCountValidator SUT;

    static PropertyInfo PropertyInfo;
    static ConfigurationCollectionAttribute Attribute;

    static ReportValidationError ReportValidationError;

    Establish ctx = () =>
    {
      SUT = new CollectionElementCountValidator();

      PropertyInfo = A.Fake<TestablePropertyInfo>();
      Attribute = new ConfigurationCollectionAttribute();
      ReportValidationError = A.Fake<ReportValidationError>();
    };

    class when_validating_collection_with_no_elements_but_minimum_and_maximum_counts
    {
      Establish ctx = () => { Attribute = new ConfigurationCollectionAttribute { MinimumElementCount = 1, MaximumElementCount = 1 }; };

      Because of = () => SUT.ValidateCollection(PropertyInfo, Attribute, null, null, ReportValidationError);

      It does_not_report_errors = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    class when_validating_collection_with_too_few_elements
    {
      Establish ctx = () => { Attribute = new ConfigurationCollectionAttribute { MinimumElementCount = 1 }; };

      Because of = () => SUT.ValidateCollection(PropertyInfo, Attribute, new XElement("collection"), new XElement[0], ReportValidationError);

      It reports_error = () =>
        A.CallTo(
          () => ReportValidationError(
            PropertyInfo,
            "The collection needs to contain at least 1 element, but contains 0.")).MustHaveHappened();
    }

    class when_validating_collection_with_too_many_elements
    {
      Establish ctx = () => { Attribute = new ConfigurationCollectionAttribute { MaximumElementCount = 1 }; };

      Because of = () => SUT.ValidateCollection(PropertyInfo, Attribute, new XElement("collection"), new XElement[2], ReportValidationError);

      It reports_error = () =>
        A.CallTo(
          () => ReportValidationError(
            PropertyInfo,
            "The collection needs to contain at most 1 element, but contains 2.")).MustHaveHappened();
    }

    class when_validating_collection_with_null_elements
    {
      Establish ctx = () => { Attribute = new ConfigurationCollectionAttribute { MinimumElementCount = 0, MaximumElementCount = 0 }; };

      Because of = () => SUT.ValidateCollection(PropertyInfo, Attribute, new XElement("collection"), null, ReportValidationError);

      It treats_null_as_zero_elements = () =>
            A.CallTo(() => ReportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }
  }
}