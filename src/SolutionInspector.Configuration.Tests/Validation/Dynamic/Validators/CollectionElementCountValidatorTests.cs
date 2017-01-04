using System.Reflection;
using System.Xml.Linq;
using FakeItEasy;
using NUnit.Framework;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.Configuration.Validation.Dynamic.Validators;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Configuration.Tests.Validation.Dynamic.Validators
{
  public class CollectionElementCountValidatorTests
  {
    private CollectionElementCountValidator _sut;

    private PropertyInfo _propertyInfo;
    private ReportValidationError _reportValidationError;

    [SetUp]
    public void SetUp ()
    {
      _sut = new CollectionElementCountValidator();

      _propertyInfo = A.Fake<TestablePropertyInfo>();
      _reportValidationError = A.Fake<ReportValidationError>();
    }

    [Test]
    public void ValidateCollection_WithTooFewElements_ReportsError ()
    {
      var attribute = new ConfigurationCollectionAttribute { MinimumElementCount = 1 };

      // ACT
      _sut.ValidateCollection(_propertyInfo, attribute, new XElement("collection"), new XElement[0], _reportValidationError);

      // ASSERT
      A.CallTo(
        () => _reportValidationError(
          _propertyInfo,
          "The collection needs to contain at least 1 element, but contains 0.")).MustHaveHappened();
    }

    [Test]
    public void ValidateCollection_WithTooManyElements_ReportsError ()
    {
      var attribute = new ConfigurationCollectionAttribute { MaximumElementCount = 1 };

      // ACT
      _sut.ValidateCollection(_propertyInfo, attribute, new XElement("collection"), new XElement[2], _reportValidationError);

      // ASSERT
      A.CallTo(
        () => _reportValidationError(
          _propertyInfo,
          "The collection needs to contain at most 1 element, but contains 2.")).MustHaveHappened();
    }


    [Test]
    public void ValidateCollection_ThatDoesNotExistButHasMinimumAndMaximum_DoesNotReportError ()
    {
      var attribute = new ConfigurationCollectionAttribute { MinimumElementCount = 1, MaximumElementCount = 1 };

      // ACT
      _sut.ValidateCollection(_propertyInfo, attribute, null, null, _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateCollection_WithNullElements_TreatsElementCountsAsZero ()
    {
      var attribute = new ConfigurationCollectionAttribute { MinimumElementCount = 0, MaximumElementCount = 0 };

      // ACT
      _sut.ValidateCollection(_propertyInfo, attribute, new XElement("collection"), null, _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }
  }
}