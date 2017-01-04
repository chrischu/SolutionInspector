using System;
using System.Reflection;
using System.Xml.Linq;
using FakeItEasy;
using NUnit.Framework;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.Configuration.Validation.Dynamic.Attributes;
using SolutionInspector.Configuration.Validation.Dynamic.Validators;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Configuration.Tests.Validation.Dynamic.Validators
{
  public class ValidationAttributeValidatorTests
  {
    private ValidationAttributeValidator _sut;

    private PropertyInfo _propertyInfo;
    private ReportValidationError _reportValidationError;

    [SetUp]
    public void SetUp ()
    {
      _sut = new ValidationAttributeValidator();

      _propertyInfo = A.Fake<TestablePropertyInfo>();
      A.CallTo(() => _propertyInfo.Name).Returns("Property");

      _reportValidationError = A.Fake<ReportValidationError>();
    }

    [Test]
    public void ValidateValue_WithValidationAttribute_CallsValidationAttribute ()
    {
      var validationAttribute = A.Fake<ConfigurationValueValidationAttribute>();
      // ReSharper disable once CoVariantArrayConversion
      A.CallTo(() => _propertyInfo.GetCustomAttributes(A<Type>._, A<bool>._)).Returns(new[] { validationAttribute });

      var attribute = new ConfigurationValueAttribute();
      var xmlAttribute = new XAttribute("DontCare", "DontCare");

      // ACT
      _sut.ValidateValue(_propertyInfo, attribute, xmlAttribute, _reportValidationError);

      // ASSERT
      A.CallTo(() => validationAttribute.Validate(_propertyInfo, attribute, xmlAttribute, _reportValidationError)).MustHaveHappened();
      A.CallTo(() => _propertyInfo.GetCustomAttributes(typeof(ConfigurationValueValidationAttribute), true)).MustHaveHappened();
    }

    [Test]
    public void ValidateSubelement_WithValidationAttribute_CallsValidationAttribute ()
    {
      var validationAttribute = A.Fake<ConfigurationSubelementValidationAttribute>();
      // ReSharper disable once CoVariantArrayConversion
      A.CallTo(() => _propertyInfo.GetCustomAttributes(A<Type>._, A<bool>._)).Returns(new[] { validationAttribute });

      var attribute = new ConfigurationSubelementAttribute();
      var xmlElement = new XElement("DontCare");

      // ACT
      _sut.ValidateSubelement(_propertyInfo, attribute, xmlElement, _reportValidationError);

      // ASSERT
      A.CallTo(() => validationAttribute.Validate(_propertyInfo, attribute, xmlElement, _reportValidationError)).MustHaveHappened();
      A.CallTo(() => _propertyInfo.GetCustomAttributes(typeof(ConfigurationSubelementValidationAttribute), true)).MustHaveHappened();
    }

    [Test]
    public void ValidateCollection_WithValidationAttribute_CallsValidationAttribute ()
    {
      var validationAttribute = A.Fake<ConfigurationCollectionValidationAttribute>();
      // ReSharper disable once CoVariantArrayConversion
      A.CallTo(() => _propertyInfo.GetCustomAttributes(A<Type>._, A<bool>._)).Returns(new[] { validationAttribute });

      var attribute = new ConfigurationCollectionAttribute();
      var collectionElement = new XElement("DontCare");
      var collectionItems = new[] { new XElement("DontCare") };

      // ACT
      _sut.ValidateCollection(_propertyInfo, attribute, collectionElement, collectionItems, _reportValidationError);

      // ASSERT
      A.CallTo(() => validationAttribute.Validate(_propertyInfo, attribute, collectionElement, collectionItems, _reportValidationError))
          .MustHaveHappened();
      A.CallTo(() => _propertyInfo.GetCustomAttributes(typeof(ConfigurationCollectionValidationAttribute), true)).MustHaveHappened();
    }
  }
}