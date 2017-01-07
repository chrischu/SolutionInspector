using System;
using System.Linq;
using System.Reflection;

namespace SolutionInspector.Configuration.Validation.Static.Validators
{
  internal class ConfigurationConverterValidator : StaticConfigurationValidatorBase
  {
    public override void ValidateValue (PropertyInfo property, ConfigurationValueAttribute attribute, ReportValidationError reportValidationError)
    {
      if (attribute.ConfigurationConverter != null && !IsValidConfigurationConverterType(attribute.ConfigurationConverter, property.PropertyType))
        reportValidationError(
          property,
          $"The type '{attribute.ConfigurationConverter}' is not a valid configuration converter " +
          $"type for a property of type '{property.PropertyType}'.");
    }

    private bool IsValidConfigurationConverterType (Type configurationConverterType, Type propertyType)
    {
      return configurationConverterType.GetInterfaces().Any(
        t => t.IsConstructedGenericType
             && typeof(IConfigurationConverter<>).IsAssignableFrom(t.GetGenericTypeDefinition())
             && propertyType == t.GetGenericArguments().Single());
    }
  }
}