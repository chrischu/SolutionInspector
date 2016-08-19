using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using JetBrains.Annotations;
using SolutionInspector.Configuration.Validation.Dynamic.Attributes;

namespace SolutionInspector.Configuration.Validation.Dynamic.Validators
{
  internal class ValidationAttributeValidator : DynamicConfigurationValidatorBase
  {
    public override void ValidateValue (
      PropertyInfo property,
      ConfigurationValueAttribute attribute,
      [CanBeNull] XAttribute valueAttribute,
      ReportValidationError reportValidationError)
    {
      var validationAttribute = property.GetCustomAttribute<ConfigurationValueValidationAttribute>();
      validationAttribute?.Validate(property, attribute, valueAttribute, reportValidationError);
    }

    public override void ValidateSubelement (
      PropertyInfo property,
      ConfigurationSubelementAttribute attribute,
      [CanBeNull] XElement subelement,
      ReportValidationError reportValidationError)
    {
      var validationAttribute = property.GetCustomAttribute<ConfigurationSubelementValidationAttribute>();
      validationAttribute?.Validate(property, attribute, subelement, reportValidationError);
    }

    public override void ValidateCollection (
      PropertyInfo property,
      ConfigurationCollectionAttribute attribute,
      [CanBeNull] XElement collectionElement,
      [CanBeNull] IReadOnlyCollection<XElement> collectionItems,
      ReportValidationError reportValidationError)
    {
      var validationAttribute = property.GetCustomAttribute<ConfigurationCollectionValidationAttribute>();
      validationAttribute?.Validate(property, attribute, collectionElement, collectionItems, reportValidationError);
    }
  }
}