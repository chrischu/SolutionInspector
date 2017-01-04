using System.Reflection;
using FakeItEasy;
using NUnit.Framework;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.Configuration.Validation.Static.Validators;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Configuration.Tests.Validation.Static.Validators
{
  public class XmlNameValidatorTests
  {
    static XmlNameValidator _sut;

    static PropertyInfo _propertyInfo;
    static ReportValidationError _reportValidationError;

    [SetUp]
    public void SetUp ()
    {
      _sut = new XmlNameValidator();

      _propertyInfo = A.Fake<TestablePropertyInfo>();
      _reportValidationError = A.Fake<ReportValidationError>();
    }

    [Test]
    public void ValidateValue_WithValidName_DoesNotReportError ()
    {
      // ACT
      _sut.ValidateValue(_propertyInfo, new ConfigurationValueAttribute { AttributeName = "valid" }, _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateValue_WithInvalidName_ReportsError ()
    {
      // ACT
      _sut.ValidateValue(_propertyInfo, new ConfigurationValueAttribute { AttributeName = "in@valid" }, _reportValidationError);

      // ASSERT
      A.CallTo(
        () => _reportValidationError(
          _propertyInfo,
          "'in@valid' is not a valid XML name.")).MustHaveHappened();
    }

    [Test]
    public void ValidateSubelement_WithValidName_DoesNotReportError ()
    {
      // ACT
      _sut.ValidateSubelement(_propertyInfo, new ConfigurationSubelementAttribute { ElementName = "valid" }, _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateSubelement_WithInvalidName_ReportsError ()
    {
      // ACT
      _sut.ValidateSubelement(_propertyInfo, new ConfigurationSubelementAttribute { ElementName = "in@valid" }, _reportValidationError);

      // ASSERT
      A.CallTo(
        () => _reportValidationError(
          _propertyInfo,
          "'in@valid' is not a valid XML name.")).MustHaveHappened();
    }

    [Test]
    public void ValidateCollection_WithValidNames_DoesNotReportError ()
    {
      // ACT
      _sut.ValidateCollection(
        _propertyInfo,
        new ConfigurationCollectionAttribute { CollectionName = "valid", ElementName = "valid" },
        _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateCollection_WithInvalidNames_ReportsError ()
    {
      // ACT
      _sut.ValidateCollection(
        _propertyInfo,
        new ConfigurationCollectionAttribute { CollectionName = "in@validCollection", ElementName = "in@validElement" },
        _reportValidationError);

      // ASSERT
      A.CallTo(
        () => _reportValidationError(
          _propertyInfo,
          "'in@validCollection' is not a valid XML name.")).MustHaveHappened();

      A.CallTo(
        () => _reportValidationError(
          _propertyInfo,
          "'in@validElement' is not a valid XML name.")).MustHaveHappened();
    }
  }
}