using System;
using System.Reflection;

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
          $"'{typeof(ConfigurationElementCollection<>)}' is allowed.");
    }

    private bool IsValidConfigurationCollectionType (Type type)
    {
      return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(ConfigurationElementCollection<>);
    }
  }
}