using System.Reflection;
using FakeItEasy;
using NUnit.Framework;
using SolutionInspector.Configuration.Attributes;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.Configuration.Validation.Static.Validators;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Configuration.Tests.Validation.Static.Validators
{
  public class XmlNameUniquenessValidatorTests
  {
    private static XmlNameUniquenessValidator _sut;

    private static PropertyInfo _propertyInfo;
    private static ReportValidationError _reportValidationError;

    [SetUp]
    public void SetUp ()
    {
      _sut = new XmlNameUniquenessValidator();

      _propertyInfo = A.Fake<TestablePropertyInfo>();
      _reportValidationError = A.Fake<ReportValidationError>();
    }

    [Test]
    public void ValidateType_WithOnlyUniqueNames_DoesNotReportError ()
    {
      var type = Some.Type;

      // ACT
      _sut.BeginTypeValidation(type, _reportValidationError);
      _sut.ValidateValue(_propertyInfo, new ConfigurationValueAttribute { AttributeName = "value" }, _reportValidationError);
      _sut.ValidateSubelement(_propertyInfo, new ConfigurationSubelementAttribute { ElementName = "subelement" }, _reportValidationError);
      _sut.ValidateCollection(_propertyInfo, new ConfigurationCollectionAttribute { CollectionName = "collection" }, _reportValidationError);
      _sut.EndTypeValidation(type, _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateType_WithDuplicateAttributeNames_ReportsErrorForEachProperty ()
    {
      var type = Some.Type;
      var property1 = A.Fake<TestablePropertyInfo>(o => o.ConfigureFake(info => A.CallTo(() => info.Name).Returns("prop1")));
      var property2 = A.Fake<TestablePropertyInfo>(o => o.ConfigureFake(info => A.CallTo(() => info.Name).Returns("prop2")));

      // ACT
      _sut.BeginTypeValidation(type, _reportValidationError);
      _sut.ValidateValue(property1, new ConfigurationValueAttribute { AttributeName = "duplicate" }, _reportValidationError);
      _sut.ValidateValue(property2, new ConfigurationValueAttribute { AttributeName = "duplicate" }, _reportValidationError);
      _sut.EndTypeValidation(type, _reportValidationError);

      // ASSERT
      A.CallTo(
        () => _reportValidationError(
          property1,
          "The XML attribute name 'duplicate' is not unique (it is duplicated in 'prop2').")).MustHaveHappened();

      A.CallTo(
        () => _reportValidationError(
          property2,
          "The XML attribute name 'duplicate' is not unique (it is duplicated in 'prop1').")).MustHaveHappened();
    }

    [Test]
    public void ValidateType_WithDuplicateElementNames_ReportsErrorForEachProperty ()
    {
      var type = Some.Type;
      var property1 = A.Fake<TestablePropertyInfo>(o => o.ConfigureFake(info => A.CallTo(() => info.Name).Returns("prop1")));
      var property2 = A.Fake<TestablePropertyInfo>(o => o.ConfigureFake(info => A.CallTo(() => info.Name).Returns("prop2")));

      // ACT
      _sut.BeginTypeValidation(type, _reportValidationError);
      _sut.ValidateSubelement(property1, new ConfigurationSubelementAttribute { ElementName = "duplicate" }, _reportValidationError);
      _sut.ValidateCollection(property2, new ConfigurationCollectionAttribute { CollectionName = "duplicate" }, _reportValidationError);
      _sut.EndTypeValidation(type, _reportValidationError);

      // ASSERT
      A.CallTo(
        () => _reportValidationError(
          property1,
          "The XML element name 'duplicate' is not unique (it is duplicated in 'prop2').")).MustHaveHappened();

      A.CallTo(
        () => _reportValidationError(
          property2,
          "The XML element name 'duplicate' is not unique (it is duplicated in 'prop1').")).MustHaveHappened();
    }

    [Test]
    public void ValidateType_WithSameNameUsedForAttributeAndElement_DoesNotReportError ()
    {
      var type = Some.Type;

      // ACT
      _sut.BeginTypeValidation(type, _reportValidationError);
      _sut.ValidateValue(_propertyInfo, new ConfigurationValueAttribute { AttributeName = "duplicate" }, _reportValidationError);
      _sut.ValidateSubelement(_propertyInfo, new ConfigurationSubelementAttribute { ElementName = "duplicate" }, _reportValidationError);
      _sut.EndTypeValidation(type, _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public void ValidateType_MultipleTypesWithSameXmlNames_DoesNotReportError ()
    {
      var type1 = Some.Type;
      var type2 = Some.Type;

      // ACT
      _sut.BeginTypeValidation(type1, _reportValidationError);
      _sut.ValidateValue(_propertyInfo, new ConfigurationValueAttribute { AttributeName = "duplicate" }, _reportValidationError);
      _sut.ValidateSubelement(_propertyInfo, new ConfigurationSubelementAttribute { ElementName = "duplicate2" }, _reportValidationError);
      _sut.EndTypeValidation(type1, _reportValidationError);

      _sut.BeginTypeValidation(type2, _reportValidationError);
      _sut.ValidateValue(_propertyInfo, new ConfigurationValueAttribute { AttributeName = "duplicate" }, _reportValidationError);
      _sut.ValidateSubelement(_propertyInfo, new ConfigurationSubelementAttribute { ElementName = "duplicate2" }, _reportValidationError);
      _sut.EndTypeValidation(type2, _reportValidationError);

      // ASSERT
      A.CallTo(() => _reportValidationError(A<PropertyInfo>._, A<string>._)).MustNotHaveHappened();
    }
  }
}