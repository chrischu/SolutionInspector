using System.Reflection;

namespace SolutionInspector.Configuration.Validation.Static.Validators
{
  internal class ConfigurationSubelementTypeValidator : StaticConfigurationValidatorBase
  {
    public override void ValidateSubelement (
      PropertyInfo property,
      ConfigurationSubelementAttribute attribute,
      ReportValidationError reportValidationError)
    {
      if (!typeof(ConfigurationElement).IsAssignableFrom(property.PropertyType))
      {
        reportValidationError(
          property,
          $"'{property.PropertyType}' is not a valid type for a configuration sub element, only " +
          $"types derived from '{typeof(ConfigurationElement)}' are allowed.");
      }
    }
  }
}