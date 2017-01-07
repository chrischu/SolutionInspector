using System;
using System.Reflection;

namespace SolutionInspector.Configuration.Validation.Static.Validators
{
  internal class ConfigurationValueTypeValidator : StaticConfigurationValidatorBase
  {
    public override void ValidateValue (
      PropertyInfo property,
      ConfigurationValueAttribute attribute,
      ReportValidationError reportValidationError)
    {
      if (attribute.ConfigurationConverter == null &&
          property.PropertyType.GetCustomAttribute<ConfigurationConverterAttribute>() == null &&
          !IsValidConfigurationValueType(property.PropertyType))
        reportValidationError(
          property,
          $"'{property.PropertyType}' is not a valid type for a configuration value, " +
          $"only primitives (e.g. 'int', 'double'), 'string' and types deriving from '{typeof(IConfigurationValue)}' are allowed.");
    }

    private bool IsValidConfigurationValueType (Type type)
    {
      return type.IsPrimitive || type == typeof(string) || typeof(IConfigurationValue).IsAssignableFrom(type);
    }
  }
}