using System;
using System.Reflection;
using SolutionInspector.Configuration.Attributes;
using SolutionInspector.Configuration.Collections;

namespace SolutionInspector.Configuration.Validation.Static.Validators
{
  internal class ConfigurationCollectionTypeValidator : StaticConfigurationValidatorBase
  {
    public override void ValidateCollection (
      PropertyInfo property,
      ConfigurationCollectionAttribute attribute,
      ReportValidationError reportValidationError)
    {
      if (!IsValidConfigurationCollectionType(property.PropertyType))
        reportValidationError(
          property,
          $"'{property.PropertyType}' is not a valid type for a configuration collection, only " +
          $"'{typeof(IConfigurationElementCollection<>)}' is allowed.");
    }

    private bool IsValidConfigurationCollectionType (Type type)
    {
      return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(IConfigurationElementCollection<>);
    }
  }
}