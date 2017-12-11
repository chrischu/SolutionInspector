using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.Configuration.Validation.Static;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Configuration.Tests.Validation
{
  public class ValidatingConfigurationVisitorTests
  {
    private IDynamicConfigurationValidator _dynamicConfigurationValidator;
    private IConfigurationValidationErrorCollector _errorCollector;
    private IStaticConfigurationValidator _staticConfigurationValidator;

    private ValidatingConfigurationVisitor _sut;

    [SetUp]
    public void SetUp ()
    {
      _errorCollector = A.Fake<IConfigurationValidationErrorCollector>();
      _staticConfigurationValidator = A.Fake<IStaticConfigurationValidator>();
      _dynamicConfigurationValidator = A.Fake<IDynamicConfigurationValidator>();

      _sut = new ValidatingConfigurationVisitor(_errorCollector, new[] { _staticConfigurationValidator }, new[] { _dynamicConfigurationValidator });
    }

    [Test]
    public void StaticBeginTypeVisit ()
    {
      var propertyPath = "Type";
      var configurationElementType = Some.Type;

      var propertyInfo = A.Fake<TestablePropertyInfo>();
      A.CallTo(() => propertyInfo.Name).Returns("Property");

      A.CallTo(() => _staticConfigurationValidator.BeginTypeValidation(A<Type>._, A<ReportValidationError>._))
          .Invokes((Type t, ReportValidationError r) => r(propertyInfo, "Message"));

      // ACT
      _sut.BeginTypeVisit(propertyPath, configurationElementType);

      // ASSERT
      A.CallTo(() => _errorCollector.AddError("Type.Property", "Message")).MustHaveHappened();

      A.CallTo(() => _staticConfigurationValidator.BeginTypeValidation(configurationElementType, A<ReportValidationError>._)).MustHaveHappened();
    }

    [Test]
    public void StaticBeginTypeVisit_WhenErrorIsCachedAlready ()
    {
      var propertyPath = "Type";
      var configurationElementType = Some.Type;

      var propertyInfo = A.Fake<TestablePropertyInfo>();
      A.CallTo(() => propertyInfo.Name).Returns("Property");
      A.CallTo(() => propertyInfo.DeclaringType).Returns(configurationElementType);

      A.CallTo(() => _staticConfigurationValidator.BeginTypeValidation(A<Type>._, A<ReportValidationError>._))
          .Invokes((Type t, ReportValidationError r) => r(propertyInfo, "Message"));

      _sut.BeginTypeVisit(propertyPath, configurationElementType);

      // ACT
      _sut.BeginTypeVisit(propertyPath, configurationElementType);

      // ASSERT
      A.CallTo(() => _errorCollector.AddError("Type.Property", "Message")).MustHaveHappened();

      A.CallTo(() => _staticConfigurationValidator.BeginTypeValidation(configurationElementType, A<ReportValidationError>._))
          .MustHaveHappened(Repeated.Exactly.Once);
    }

    [Test]
    public void StaticVisitValue ()
    {
      var propertyPath = "Type.Value";
      var propertyInfo = A.Fake<TestablePropertyInfo>();
      var configurationValueAttribute = new ConfigurationValueAttribute();

      A.CallTo(() => _staticConfigurationValidator.ValidateValue(A<PropertyInfo>._, A<ConfigurationValueAttribute>._, A<ReportValidationError>._))
          .Invokes((PropertyInfo p, ConfigurationValueAttribute _, ReportValidationError r) => r(p, "Message"));

      // ACT
      _sut.VisitValue(propertyPath, propertyInfo, configurationValueAttribute);

      // ASSERT
      A.CallTo(() => _errorCollector.AddError("Type.Value", "Message")).MustHaveHappened();

      A.CallTo(() => _staticConfigurationValidator.ValidateValue(propertyInfo, configurationValueAttribute, A<ReportValidationError>._))
          .MustHaveHappened();
    }

    [Test]
    public void StaticVisitValue_WhenValidationIsDisabled_SkipsValidation ()
    {
      var type = Some.Type;

      DisableValidationForType(type);

      var propertyPath = "Type.Value";
      var propertyInfo = A.Fake<TestablePropertyInfo>();
      var configurationValueAttribute = new ConfigurationValueAttribute();

      // ACT
      _sut.VisitValue(propertyPath, propertyInfo, configurationValueAttribute);

      // ASSERT
      A.CallTo(() => _staticConfigurationValidator.ValidateValue(propertyInfo, configurationValueAttribute, A<ReportValidationError>._))
          .MustNotHaveHappened();
    }

    [Test]
    public void StaticVisitSubelement ()
    {
      var propertyPath = "Type.Value";
      var propertyInfo = A.Fake<TestablePropertyInfo>();
      var configurationSubelementAttribute = new ConfigurationSubelementAttribute();

      A.CallTo(
            () =>
              _staticConfigurationValidator.ValidateSubelement(
                A<PropertyInfo>._,
                A<ConfigurationSubelementAttribute>._,
                A<ReportValidationError>._))
          .Invokes((PropertyInfo p, ConfigurationSubelementAttribute _, ReportValidationError r) => r(p, "Message"));

      // ACT
      _sut.VisitSubelement(propertyPath, propertyInfo, configurationSubelementAttribute);

      // ASSERT
      A.CallTo(() => _errorCollector.AddError("Type.Value", "Message")).MustHaveHappened();

      A.CallTo(() => _staticConfigurationValidator.ValidateSubelement(propertyInfo, configurationSubelementAttribute, A<ReportValidationError>._))
          .MustHaveHappened();
    }

    [Test]
    public void StaticVisitSubelement_WhenValidationIsDisabled_SkipsValidation ()
    {
      var type = Some.Type;

      DisableValidationForType(type);

      var propertyPath = "Type.Value";
      var propertyInfo = A.Fake<TestablePropertyInfo>();
      var configurationSubelementAttribute = new ConfigurationSubelementAttribute();

      // ACT
      _sut.VisitSubelement(propertyPath, propertyInfo, configurationSubelementAttribute);

      // ASSERT
      A.CallTo(() => _staticConfigurationValidator.ValidateSubelement(propertyInfo, configurationSubelementAttribute, A<ReportValidationError>._))
          .MustNotHaveHappened();
    }

    [Test]
    public void StaticVisitCollection ()
    {
      var propertyPath = "Type.Value";
      var propertyInfo = A.Fake<TestablePropertyInfo>();
      var configurationCollectionAttribute = new ConfigurationCollectionAttribute();

      A.CallTo(
            () =>
              _staticConfigurationValidator.ValidateCollection(
                A<PropertyInfo>._,
                A<ConfigurationCollectionAttribute>._,
                A<ReportValidationError>._))
          .Invokes((PropertyInfo p, ConfigurationCollectionAttribute _, ReportValidationError r) => r(p, "Message"));

      // ACT
      _sut.VisitCollection(propertyPath, propertyInfo, configurationCollectionAttribute);

      // ASSERT
      A.CallTo(() => _errorCollector.AddError("Type.Value", "Message")).MustHaveHappened();

      A.CallTo(() => _staticConfigurationValidator.ValidateCollection(propertyInfo, configurationCollectionAttribute, A<ReportValidationError>._))
          .MustHaveHappened();
    }

    [Test]
    public void StaticVisitCollection_WhenValidationIsDisabled_SkipsValidation ()
    {
      var type = Some.Type;

      DisableValidationForType(type);

      var propertyPath = "Type.Value";
      var propertyInfo = A.Fake<TestablePropertyInfo>();
      var configurationCollectionAttribute = new ConfigurationCollectionAttribute();

      // ACT
      _sut.VisitCollection(propertyPath, propertyInfo, configurationCollectionAttribute);

      // ASSERT
      A.CallTo(() => _staticConfigurationValidator.ValidateCollection(propertyInfo, configurationCollectionAttribute, A<ReportValidationError>._))
          .MustNotHaveHappened();
    }

    [Test]
    public void StaticEndTypeVisit ()
    {
      var propertyPath = "Type";
      var configurationElementType = Some.Type;

      var propertyInfo = A.Fake<TestablePropertyInfo>();
      A.CallTo(() => propertyInfo.Name).Returns("Property");

      A.CallTo(() => _staticConfigurationValidator.EndTypeValidation(A<Type>._, A<ReportValidationError>._))
          .Invokes((Type t, ReportValidationError r) => r(propertyInfo, "Message"));

      // ACT
      _sut.EndTypeVisit(propertyPath, configurationElementType);

      // ASSERT
      A.CallTo(() => _errorCollector.AddError("Type.Property", "Message")).MustHaveHappened();

      A.CallTo(() => _staticConfigurationValidator.EndTypeValidation(configurationElementType, A<ReportValidationError>._)).MustHaveHappened();
    }

    [Test]
    public void StaticEndTypeVisit_WhenValidationIsDisabled_SkipsValidationAndReenablesIt ()
    {
      var configurationElementType = Some.Type;

      DisableValidationForType(configurationElementType);

      var propertyPath = "Type";

      // ASSERT
      A.CallTo(() => _errorCollector.AddError(A<string>._, "Message")).MustNotHaveHappened();

      // ACT
      _sut.EndTypeVisit(propertyPath, configurationElementType);

      // ASSERT
      A.CallTo(() => _staticConfigurationValidator.EndTypeValidation(configurationElementType, A<ReportValidationError>._)).MustNotHaveHappened();

      IsValidationDisabled().Should().BeFalse();
    }

    private void DisableValidationForType (Type type)
    {
      var propertyInfo = A.Dummy<TestablePropertyInfo>();
      A.CallTo(() => propertyInfo.DeclaringType).Returns(type);

      A.CallTo(() => _staticConfigurationValidator.BeginTypeValidation(A<Type>._, A<ReportValidationError>._))
          .Invokes((Type t, ReportValidationError r) => r(propertyInfo, Some.String));

      _sut.BeginTypeVisit(Some.String, type);
      _sut.BeginTypeVisit(Some.String, type);
    }

    private bool IsValidationDisabled ()
    {
      var result = true;
      var type = typeof(object);

      A.CallTo(() => _errorCollector.AddError(A<string>._, A<string>._)).Invokes(() => result = false);
      A.CallTo(() => _staticConfigurationValidator.EndTypeValidation(type, A<ReportValidationError>._))
          .Invokes((Type t, ReportValidationError r) => r(A.Dummy<TestablePropertyInfo>(), "Message"));

      _sut.EndTypeVisit(Some.String, type);

      return result;
    }

    [Test]
    public void DynamicBeginTypeVisit ()
    {
      var propertyPath = "Type";
      var configurationElementType = Some.Type;
      var element = Some.XElement;

      var propertyInfo = A.Fake<TestablePropertyInfo>();
      A.CallTo(() => propertyInfo.Name).Returns("Property");

      A.CallTo(() => _dynamicConfigurationValidator.BeginTypeValidation(A<Type>._, A<XElement>._, A<ReportValidationError>._))
          .Invokes((Type t, XElement _, ReportValidationError r) => r(propertyInfo, "Message"));

      // ACT
      _sut.BeginTypeVisit(propertyPath, configurationElementType, element);

      // ASSERT
      A.CallTo(() => _errorCollector.AddError("Type.Property", "Message")).MustHaveHappened();

      A.CallTo(() => _dynamicConfigurationValidator.BeginTypeValidation(configurationElementType, element, A<ReportValidationError>._))
          .MustHaveHappened();
    }

    [Test]
    public void DynamicVisitValue ()
    {
      var propertyPath = "Type.Property";
      var propertyInfo = A.Fake<TestablePropertyInfo>();
      var attribute = new ConfigurationValueAttribute();

      var xAttribute = Some.XAttribute;

      A.CallTo(
            () =>
              _dynamicConfigurationValidator.ValidateValue(
                A<PropertyInfo>._,
                A<ConfigurationValueAttribute>._,
                A<XAttribute>._,
                A<ReportValidationError>._))
          .Invokes((PropertyInfo p, ConfigurationValueAttribute _, XAttribute __, ReportValidationError r) => r(p, "Message"));

      // ACT
      _sut.VisitValue(propertyPath, propertyInfo, attribute, xAttribute);

      // ASSERT
      A.CallTo(() => _errorCollector.AddError("Type.Property", "Message")).MustHaveHappened();

      A.CallTo(() => _dynamicConfigurationValidator.ValidateValue(propertyInfo, attribute, xAttribute, A<ReportValidationError>._))
          .MustHaveHappened();
    }

    [Test]
    public void DynamicVisitSubelement ()
    {
      var propertyPath = "Type.Property";
      var propertyInfo = A.Fake<TestablePropertyInfo>();
      var attribute = new ConfigurationSubelementAttribute();

      var xElement = Some.XElement;

      A.CallTo(
            () =>
              _dynamicConfigurationValidator.ValidateSubelement(
                A<PropertyInfo>._,
                A<ConfigurationSubelementAttribute>._,
                A<XElement>._,
                A<ReportValidationError>._))
          .Invokes((PropertyInfo p, ConfigurationSubelementAttribute _, XElement __, ReportValidationError r) => r(p, "Message"));

      // ACT
      _sut.VisitSubelement(propertyPath, propertyInfo, attribute, xElement);

      // ASSERT
      A.CallTo(() => _errorCollector.AddError("Type.Property", "Message")).MustHaveHappened();

      A.CallTo(() => _dynamicConfigurationValidator.ValidateSubelement(propertyInfo, attribute, xElement, A<ReportValidationError>._))
          .MustHaveHappened();
    }

    [Test]
    public void DynamicVisitCollection ()
    {
      var propertyPath = "Type.Property";
      var propertyInfo = A.Fake<TestablePropertyInfo>();
      var attribute = new ConfigurationCollectionAttribute();

      var collectionElement = Some.XElement;
      var collectionElements = new[] { Some.XElement };

      A.CallTo(
            () =>
              _dynamicConfigurationValidator.ValidateCollection(
                A<PropertyInfo>._,
                A<ConfigurationCollectionAttribute>._,
                A<XElement>._,
                A<IReadOnlyCollection<XElement>>._,
                A<ReportValidationError>._))
          .Invokes(c => ((ReportValidationError) c.Arguments[4])((PropertyInfo) c.Arguments[0], "Message"));

      // ACT
      _sut.VisitCollection(propertyPath, propertyInfo, attribute, collectionElement, collectionElements);

      // ASSERT
      A.CallTo(() => _errorCollector.AddError("Type.Property", "Message")).MustHaveHappened();

      A.CallTo(
            () =>
              _dynamicConfigurationValidator.ValidateCollection(
                propertyInfo,
                attribute,
                collectionElement,
                collectionElements,
                A<ReportValidationError>._))
          .MustHaveHappened();
    }

    [Test]
    public void DynamicEndTypeVisit ()
    {
      var propertyPath = "Type";
      var configurationElementType = Some.Type;
      var element = Some.XElement;

      var propertyInfo = A.Fake<TestablePropertyInfo>();
      A.CallTo(() => propertyInfo.Name).Returns("Property");

      A.CallTo(() => _dynamicConfigurationValidator.EndTypeValidation(A<Type>._, A<XElement>._, A<ReportValidationError>._))
          .Invokes((Type t, XElement _, ReportValidationError r) => r(propertyInfo, "Message"));

      // ACT
      _sut.EndTypeVisit(propertyPath, configurationElementType, element);

      // ASSERT
      A.CallTo(() => _errorCollector.AddError("Type.Property", "Message")).MustHaveHappened();

      A.CallTo(() => _dynamicConfigurationValidator.EndTypeValidation(configurationElementType, element, A<ReportValidationError>._))
          .MustHaveHappened();
    }

    [Test]
    public void BuildPropertyPath ()
    {
      var configurationElementType = Some.Type;

      var propertyInfo = A.Fake<TestablePropertyInfo>();
      A.CallTo(() => propertyInfo.Name).Returns("Property");

      A.CallTo(() => _staticConfigurationValidator.BeginTypeValidation(A<Type>._, A<ReportValidationError>._))
          .Invokes((Type t, ReportValidationError r) => r(propertyInfo, "Message"));

      // ACT
      _sut.BeginTypeVisit("Type", configurationElementType);

      // ASSERT
      A.CallTo(() => _errorCollector.AddError("Type.Property", "Message")).MustHaveHappened();

      // ACT
      _sut.BeginTypeVisit("", configurationElementType);

      // ASSERT
      A.CallTo(() => _errorCollector.AddError("Property", "Message")).MustHaveHappened();
    }
  }
}