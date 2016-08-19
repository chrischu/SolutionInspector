using System;
using System.Reflection;
using System.Xml.Linq;
using FakeItEasy;
using Machine.Specifications;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.Configuration.Validation.Dynamic.Attributes;
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
  [Subject (typeof(ValidationAttributeValidator))]
  class ValidationAttributeValidatorSpec
  {
    static ValidationAttributeValidator SUT;

    static PropertyInfo PropertyInfo;

    static ReportValidationError ReportValidationError;

    Establish ctx = () =>
    {
      SUT = new ValidationAttributeValidator();

      PropertyInfo = A.Fake<TestablePropertyInfo>();
      ReportValidationError = A.Fake<ReportValidationError>();
    };

    class when_validating_value_with_validation_attribute
    {
      Establish ctx = () =>
      {
        ValidationAttribute = A.Fake<ConfigurationValueValidationAttribute>();
        // ReSharper disable once CoVariantArrayConversion
        A.CallTo(() => PropertyInfo.GetCustomAttributes(A<Type>._, A<bool>._)).Returns(new[] { ValidationAttribute });

        Attribute = new ConfigurationValueAttribute();
        XmlAttribute = new XAttribute("dontcare", "dontcare");
      };

      Because of =
          () => SUT.ValidateValue(PropertyInfo, Attribute, XmlAttribute, ReportValidationError);

      It calls_ValidationAttribute_Validate =
          () => A.CallTo(() => ValidationAttribute.Validate(PropertyInfo, Attribute, XmlAttribute, ReportValidationError)).MustHaveHappened();

      It retrieves_custom_attribute = () =>
            A.CallTo(() => PropertyInfo.GetCustomAttributes(typeof(ConfigurationValueValidationAttribute), true)).MustHaveHappened();

      static ConfigurationValueAttribute Attribute;
      static XAttribute XmlAttribute;
      static ConfigurationValueValidationAttribute ValidationAttribute;
    }

    class when_validating_subelement_with_validation_attribute
    {
      Establish ctx = () =>
      {
        ValidationAttribute = A.Fake<ConfigurationSubelementValidationAttribute>();
        // ReSharper disable once CoVariantArrayConversion
        A.CallTo(() => PropertyInfo.GetCustomAttributes(A<Type>._, A<bool>._)).Returns(new[] { ValidationAttribute });

        Attribute = new ConfigurationSubelementAttribute();
        XmlElement = new XElement("dontcare");
      };

      Because of =
          () => SUT.ValidateSubelement(PropertyInfo, Attribute, XmlElement, ReportValidationError);

      It calls_ValidationAttribute_Validate =
          () => A.CallTo(() => ValidationAttribute.Validate(PropertyInfo, Attribute, XmlElement, ReportValidationError)).MustHaveHappened();

      It retrieves_custom_attribute = () =>
            A.CallTo(() => PropertyInfo.GetCustomAttributes(typeof(ConfigurationSubelementValidationAttribute), true)).MustHaveHappened();

      static ConfigurationSubelementAttribute Attribute;
      static XElement XmlElement;
      static ConfigurationSubelementValidationAttribute ValidationAttribute;
    }

    class when_validating_collection_with_validation_attribute
    {
      Establish ctx = () =>
      {
        ValidationAttribute = A.Fake<ConfigurationCollectionValidationAttribute>();
        // ReSharper disable once CoVariantArrayConversion
        A.CallTo(() => PropertyInfo.GetCustomAttributes(A<Type>._, A<bool>._)).Returns(new[] { ValidationAttribute });

        Attribute = new ConfigurationCollectionAttribute();
        CollectionElement = new XElement("dontcare");
        CollectionItems = new[] { new XElement("dontcare") };
      };

      Because of =
          () => SUT.ValidateCollection(PropertyInfo, Attribute, CollectionElement, CollectionItems, ReportValidationError);

      It calls_ValidationAttribute_Validate =
          () =>
            A.CallTo(() => ValidationAttribute.Validate(PropertyInfo, Attribute, CollectionElement, CollectionItems, ReportValidationError))
                .MustHaveHappened();

      It retrieves_custom_attribute = () =>
            A.CallTo(() => PropertyInfo.GetCustomAttributes(typeof(ConfigurationCollectionValidationAttribute), true)).MustHaveHappened();

      static ConfigurationCollectionAttribute Attribute;
      static XElement CollectionElement;
      static XElement[] CollectionItems;
      static ConfigurationCollectionValidationAttribute ValidationAttribute;
    }
  }
}