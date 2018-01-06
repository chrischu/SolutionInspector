using System;
using System.Reflection;
using SolutionInspector.Configuration.Attributes;

namespace SolutionInspector.Configuration.Validation.Static.Validators
{
  internal class RequiredConfigurationValueMustNotHaveADefaultValueValidator : StaticConfigurationValidatorBase
  {
    public override void ValidateValue (PropertyInfo property, ConfigurationValueAttribute attribute, ReportValidationError reportValidationError)
    {
      if(attribute.IsRequired && attribute.DefaultValue != null)
        reportValidationError(property, "A required property must not have a default value.");
    }
  }
}