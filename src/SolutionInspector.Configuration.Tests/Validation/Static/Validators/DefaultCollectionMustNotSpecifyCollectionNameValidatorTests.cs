using System.Reflection;
using FakeItEasy;
using NUnit.Framework;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.Configuration.Validation.Static.Validators;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Configuration.Tests.Validation.Static.Validators
{
  public class DefaultCollectionMustNotSpecifyCollectionNameValidatorTests
  {
    private DefaultCollectionMustNotSpecifyCollectionNameValidator _sut;

    private PropertyInfo _propertyInfo;
    private ReportValidationError _reportValidationError;

    [SetUp]
    public void SetUp ()
    {
      _sut = new DefaultCollectionMustNotSpecifyCollectionNameValidator();

      _propertyInfo = A.Fake<TestablePropertyInfo>();
      _reportValidationError = A.Fake<ReportValidationError>();
    }

    [Test]
    public void ValidateCollection_NonDefaultCollectionWithCollectionName_DoesNotReportError ()
    {
      var attribute = new ConfigurationCollectionAttribute { CollectionName = Some.String(), IsDefaultCollection = false };

      // ACT
      _sut.ValidateCollection(_propertyInfo, attribute, _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateCollection_DefaultCollectionWithoutCollectionName_DoesNotReportError ()
    {
      var attribute = new ConfigurationCollectionAttribute { CollectionName = null, IsDefaultCollection = true };

      // ACT
      _sut.ValidateCollection(_propertyInfo, attribute, _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateCollection_DefaultCollectionWithCollectionName_ReportsError ()
    {
      var attribute = new ConfigurationCollectionAttribute { CollectionName = Some.String(), IsDefaultCollection = true };

      // ACT
      _sut.ValidateCollection(_propertyInfo, attribute, _reportValidationError);

      // ASSERT
      A.CallTo(
            () => _reportValidationError(
              _propertyInfo,
              "A configuration collection marked as the default collection must not specify a collection name."))
          .MustHaveHappened();
    }
  }
}