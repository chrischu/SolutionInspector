using System;
using System.Reflection;
using SolutionInspector.Configuration.Attributes;

namespace SolutionInspector.Configuration.Validation.Static
{
  internal abstract class StaticConfigurationValidatorBase : IStaticConfigurationValidator
  {
    public virtual void BeginTypeValidation (Type configurationElementType, ReportValidationError reportValidationError)
    {
    }

    public virtual void EndTypeValidation (Type configurationElementType, ReportValidationError reportValidationError)
    {
    }

    public virtual void ValidateCollection (
      PropertyInfo property,
      ConfigurationCollectionAttribute attribute,
      ReportValidationError reportValidationError)
    {
    }

    public virtual void ValidateSubelement (
      PropertyInfo property,
      ConfigurationSubelementAttribute attribute,
      ReportValidationError reportValidationError)
    {
    }

    public virtual void ValidateValue (
      PropertyInfo property,
      ConfigurationValueAttribute attribute,
      ReportValidationError reportValidationError)
    {
    }
  }
}