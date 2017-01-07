using System.Reflection;
using FakeItEasy;
using NUnit.Framework;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.Configuration.Validation.Static.Validators;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Configuration.Tests.Validation.Static.Validators
{
  public class SingleDefaultCollectionValidatorTests
  {
    private PropertyInfo _propertyInfo;
    private ReportValidationError _reportValidationError;
    private SingleDefaultCollectionValidator _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new SingleDefaultCollectionValidator();

      _propertyInfo = A.Fake<TestablePropertyInfo>();
      _reportValidationError = A.Fake<ReportValidationError>();
    }

    [Test]
    public void ValidateType_WithSingleDefaultCollection_DoesNotReportError ()
    {
      var type = Some.Type;

      // ACT
      _sut.BeginTypeValidation(type, _reportValidationError);

      _sut.ValidateCollection(_propertyInfo, new ConfigurationCollectionAttribute { IsDefaultCollection = true }, _reportValidationError);
      _sut.ValidateCollection(_propertyInfo, new ConfigurationCollectionAttribute { IsDefaultCollection = false }, _reportValidationError);

      _sut.EndTypeValidation(type, _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateType_WithMultipleDefaultCollections_ReportsErrorForEveryDefaultCollection ()
    {
      var type = Some.Type;

      var property1 = A.Fake<TestablePropertyInfo>(o => o.ConfigureFake(info => A.CallTo(() => info.Name).Returns("prop1")));
      var property2 = A.Fake<TestablePropertyInfo>(o => o.ConfigureFake(info => A.CallTo(() => info.Name).Returns("prop2")));
      var property3 = A.Fake<TestablePropertyInfo>(o => o.ConfigureFake(info => A.CallTo(() => info.Name).Returns("prop3")));

      // ACT
      _sut.BeginTypeValidation(type, _reportValidationError);

      _sut.ValidateCollection(property1, new ConfigurationCollectionAttribute { IsDefaultCollection = true }, _reportValidationError);
      _sut.ValidateCollection(property2, new ConfigurationCollectionAttribute { IsDefaultCollection = true }, _reportValidationError);
      _sut.ValidateCollection(property3, new ConfigurationCollectionAttribute { IsDefaultCollection = true }, _reportValidationError);
      _sut.ValidateCollection(_propertyInfo, new ConfigurationCollectionAttribute { IsDefaultCollection = false }, _reportValidationError);

      _sut.EndTypeValidation(type, _reportValidationError);

      // ASSERT
      A.CallTo(
        () => _reportValidationError(
          property1,
          "There can only be one default collection per configuration element and the following properties are already marked as " +
          "default collection: 'prop2', 'prop3'.")).MustHaveHappened();

      A.CallTo(
        () => _reportValidationError(
          property2,
          "There can only be one default collection per configuration element and the following properties are already marked as " +
          "default collection: 'prop1', 'prop3'.")).MustHaveHappened();

      A.CallTo(
        () => _reportValidationError(
          property3,
          "There can only be one default collection per configuration element and the following properties are already marked as " +
          "default collection: 'prop1', 'prop2'.")).MustHaveHappened();
    }

    [Test]
    public void ValidateType_MultipleTypesEachWithOneDefaultCollection_DoesNotReportError ()
    {
      var type1 = Some.Type;
      var type2 = Some.Type;

      // ACT
      _sut.BeginTypeValidation(type1, _reportValidationError);
      _sut.ValidateCollection(_propertyInfo, new ConfigurationCollectionAttribute { IsDefaultCollection = true }, _reportValidationError);
      _sut.EndTypeValidation(type1, _reportValidationError);

      _sut.BeginTypeValidation(type2, _reportValidationError);
      _sut.ValidateCollection(_propertyInfo, new ConfigurationCollectionAttribute { IsDefaultCollection = true }, _reportValidationError);
      _sut.EndTypeValidation(type2, _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }
  }
}