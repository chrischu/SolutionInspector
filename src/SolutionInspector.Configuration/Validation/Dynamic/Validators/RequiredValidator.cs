using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration.Validation.Dynamic.Validators
{
  internal class RequiredValidator : DynamicConfigurationValidatorBase
  {
    public override void ValidateValue (
      PropertyInfo property,
      ConfigurationValueAttribute attribute,
      [CanBeNull] XAttribute valueAttribute,
      ReportValidationError reportValidationError)
    {
      if (attribute.IsRequired && valueAttribute == null)
        reportValidationError(
          property,
          $"The value is required but no corresponding attribute with the name '{attribute.GetXmlName(property.Name)}' could be found.");
    }

    public override void ValidateSubelement (
      PropertyInfo property,
      ConfigurationSubelementAttribute attribute,
      [CanBeNull] XElement subelement,
      ReportValidationError reportValidationError)
    {
      if (attribute.IsRequired && subelement == null)
        reportValidationError(
          property,
          $"The subelement is required but no corresponding element with the name '{attribute.GetXmlName(property.Name)}' could be found.");
    }

    public override void ValidateCollection (
      PropertyInfo property,
      ConfigurationCollectionAttribute attribute,
      [CanBeNull] XElement collectionElement,
      [CanBeNull] IReadOnlyCollection<XElement> collectionItems,
      ReportValidationError reportValidationError)
    {
      if (attribute.IsRequired && collectionElement == null)
        reportValidationError(
          property,
          $"The collection is required but no corresponding element with the name '{attribute.GetXmlName(property.Name)}' could be found.");
    }
  }
}