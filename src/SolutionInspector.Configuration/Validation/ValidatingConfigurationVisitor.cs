using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using JetBrains.Annotations;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration.Validation.Dynamic;
using SolutionInspector.Configuration.Validation.Static;

namespace SolutionInspector.Configuration.Validation
{
  internal class ValidatingConfigurationVisitor : IStaticConfigurationVisitor, IDynamicConfigurationVisitor
  {
    private readonly Dictionary<Type, Dictionary<PropertyInfo, List<string>>> _cachedStaticValidationErrorsByType =
        new Dictionary<Type, Dictionary<PropertyInfo, List<string>>>();

    private readonly IReadOnlyCollection<IDynamicConfigurationValidator> _dynamicValidators;
    private readonly IConfigurationValidationErrorCollector _errorCollector;
    private readonly IReadOnlyCollection<IStaticConfigurationValidator> _staticValidators;

    private bool _disableValidation;

    public ValidatingConfigurationVisitor (
      IConfigurationValidationErrorCollector errorCollector,
      IReadOnlyCollection<IStaticConfigurationValidator> staticValidators,
      IReadOnlyCollection<IDynamicConfigurationValidator> dynamicValidators
    )
    {
      _errorCollector = errorCollector;
      _staticValidators = staticValidators;
      _dynamicValidators = dynamicValidators;
    }

    private string BuildPropertyPath (string previousPropertyPath, PropertyInfo property)
    {
      if (string.IsNullOrEmpty(previousPropertyPath))
        return property.Name;
      return previousPropertyPath + "." + property.Name;
    }

    #region Static

    public void BeginTypeVisit (string propertyPath, Type configurationElementType)
    {
      Dictionary<PropertyInfo, List<string>> typeValidationErrors;

      if (_cachedStaticValidationErrorsByType.TryGetValue(configurationElementType, out typeValidationErrors))
      {
        _disableValidation = true;

        foreach (var propertyValidationErrors in typeValidationErrors)
        {
          var newPropertyPath = BuildPropertyPath(propertyPath, propertyValidationErrors.Key);

          foreach (var messages in propertyValidationErrors.Value)
            _errorCollector.AddError(newPropertyPath, messages);
        }
      }
      else
      {
        foreach (var configurationValidator in _staticValidators)
          configurationValidator.BeginTypeValidation(
            configurationElementType,
            (prop, msg) => ReportStaticValidationError(BuildPropertyPath(propertyPath, prop), prop, msg));
      }
    }

    public void VisitValue (string propertyPath, PropertyInfo property, ConfigurationValueAttribute attribute)
    {
      if (!_disableValidation)
        foreach (var configurationValidator in _staticValidators)
          configurationValidator.ValidateValue(property, attribute, (prop, msg) => ReportStaticValidationError(propertyPath, prop, msg));
    }

    public void VisitSubelement (string propertyPath, PropertyInfo property, ConfigurationSubelementAttribute attribute)
    {
      if (!_disableValidation)
        foreach (var configurationValidator in _staticValidators)
          configurationValidator.ValidateSubelement(property, attribute, (prop, msg) => ReportStaticValidationError(propertyPath, prop, msg));
    }

    public void VisitCollection (string propertyPath, PropertyInfo property, ConfigurationCollectionAttribute attribute)
    {
      if (!_disableValidation)
        foreach (var configurationValidator in _staticValidators)
          configurationValidator.ValidateCollection(property, attribute, (prop, msg) => ReportStaticValidationError(propertyPath, prop, msg));
    }

    public void EndTypeVisit (string propertyPath, Type configurationElementType)
    {
      if (!_disableValidation)
        foreach (var configurationValidator in _staticValidators)
          configurationValidator.EndTypeValidation(
            configurationElementType,
            (prop, msg) => ReportStaticValidationError(BuildPropertyPath(propertyPath, prop), prop, msg));

      _disableValidation = false;
    }

    private void ReportStaticValidationError (string propertyPath, PropertyInfo property, string message)
    {
      var validationErrorsForType = _cachedStaticValidationErrorsByType.GetOrAdd(
        property.DeclaringType.AssertNotNull(),
        key => new Dictionary<PropertyInfo, List<string>>());
      var validationErrorsForProperty = validationErrorsForType.GetOrAdd(property, key => new List<string>());
      validationErrorsForProperty.Add(message);

      _errorCollector.AddError(propertyPath, message);
    }

    #endregion

    #region Dynamic

    public void BeginTypeVisit (string propertyPath, Type configurationElementType, XElement element)
    {
      foreach (var configurationValidator in _dynamicValidators)
        configurationValidator.BeginTypeValidation(
          configurationElementType,
          element,
          (prop, msg) => ReportDynamicValidationError(BuildPropertyPath(propertyPath, prop), msg));
    }

    public void VisitValue (string propertyPath, PropertyInfo property, ConfigurationValueAttribute attribute, [CanBeNull] XAttribute xAttribute)
    {
      foreach (var configurationValidator in _dynamicValidators)
        configurationValidator.ValidateValue(property, attribute, xAttribute, (prop, msg) => ReportDynamicValidationError(propertyPath, msg));
    }

    public void VisitSubelement (
      string propertyPath,
      PropertyInfo property,
      ConfigurationSubelementAttribute attribute,
      [CanBeNull] XElement subelement)
    {
      foreach (var configurationValidator in _dynamicValidators)
        configurationValidator.ValidateSubelement(property, attribute, subelement, (prop, msg) => ReportDynamicValidationError(propertyPath, msg));
    }

    public void VisitCollection (
      string propertyPath,
      PropertyInfo property,
      ConfigurationCollectionAttribute attribute,
      [CanBeNull] XElement collectionElement,
      [CanBeNull] IReadOnlyCollection<XElement> collectionItems)
    {
      foreach (var configurationValidator in _dynamicValidators)
        configurationValidator.ValidateCollection(
          property,
          attribute,
          collectionElement,
          collectionItems,
          (prop, msg) => ReportDynamicValidationError(propertyPath, msg));
    }

    public void EndTypeVisit (string propertyPath, Type configurationElementType, XElement element)
    {
      foreach (var configurationValidator in _dynamicValidators)
        configurationValidator.EndTypeValidation(
          configurationElementType,
          element,
          (prop, msg) => ReportDynamicValidationError(BuildPropertyPath(propertyPath, prop), msg));
    }

    private void ReportDynamicValidationError (string propertyPath, string message)
    {
      _errorCollector.AddError(propertyPath, message);
    }

    #endregion
  }
}