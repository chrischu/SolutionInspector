using System.Reflection;
using System.Xml.Linq;
using FakeItEasy;
using NUnit.Framework;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.Configuration.Validation.Dynamic.Validators;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Configuration.Tests.Validation.Dynamic.Validators
{
  public class RequiredValidatorTests
  {
    private PropertyInfo _propertyInfo;
    private ReportValidationError _reportValidationError;
    private RequiredValidator _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new RequiredValidator();

      _propertyInfo = A.Fake<TestablePropertyInfo>();
      A.CallTo(() => _propertyInfo.Name).Returns("Property");

      _reportValidationError = A.Fake<ReportValidationError>();
    }

    [Test]
    public void ValidateValue_OptionalWithMissingAttribute_ReportsNoError ()
    {
      var attribute = new ConfigurationValueAttribute { IsOptional = true };

      // ACT
      _sut.ValidateValue(_propertyInfo, attribute, null, _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateValue_RequiredWithExistingAttribute_ReportsNoError ()
    {
      var attribute = new ConfigurationValueAttribute { IsOptional = false };

      // ACT
      _sut.ValidateValue(_propertyInfo, attribute, new XAttribute("DoNotCare", "DoNotCare"), _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateValue_RequiredWithMissingAttribute_ReportsError ()
    {
      var attribute = new ConfigurationValueAttribute { IsOptional = false };

      // ACT
      _sut.ValidateValue(_propertyInfo, attribute, null, _reportValidationError);

      // ASSERT
      A.CallTo(
        () => _reportValidationError(
          _propertyInfo,
          "The value is required but no corresponding attribute with the name 'property' could be found.")).MustHaveHappened();
    }

    [Test]
    public void ValidateSubelement_OptionalWithMissingElement_ReportsNoError ()
    {
      var attribute = new ConfigurationSubelementAttribute { IsOptional = true };

      // ACT
      _sut.ValidateSubelement(_propertyInfo, attribute, null, _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateSubelement_RequiredWithExistingElement_ReportsNoError ()
    {
      var attribute = new ConfigurationSubelementAttribute { IsOptional = false };

      // ACT
      _sut.ValidateSubelement(_propertyInfo, attribute, new XElement("DoNotCare"), _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateSubelement_RequiredWithMissingElement_ReportsError ()
    {
      var attribute = new ConfigurationSubelementAttribute { IsOptional = false };

      // ACT
      _sut.ValidateSubelement(_propertyInfo, attribute, null, _reportValidationError);

      // ASSERT
      A.CallTo(
        () => _reportValidationError(
          _propertyInfo,
          "The subelement is required but no corresponding element with the name 'property' could be found.")).MustHaveHappened();
    }

    [Test]
    public void ValidateCollection_OptionalWithMissingElement_ReportsNoError ()
    {
      var attribute = new ConfigurationCollectionAttribute { IsOptional = true };

      // ACT
      _sut.ValidateCollection(_propertyInfo, attribute, null, null, _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateCollection_RequiredWithExistingElement_ReportsNoError ()
    {
      var attribute = new ConfigurationCollectionAttribute { IsOptional = false };

      // ACT
      _sut.ValidateCollection(_propertyInfo, attribute, new XElement("DoNotCare"), null, _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateCollection_RequiredWithMissingElement_ReportsError ()
    {
      var attribute = new ConfigurationCollectionAttribute { IsOptional = false };

      // ACT
      _sut.ValidateCollection(_propertyInfo, attribute, null, null, _reportValidationError);

      // ASSERT
      A.CallTo(
        () => _reportValidationError(
          _propertyInfo,
          "The collection is required but no corresponding element with the name 'property' could be found.")).MustHaveHappened();
    }
  }
}