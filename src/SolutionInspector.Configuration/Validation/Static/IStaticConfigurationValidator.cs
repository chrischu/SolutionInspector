using System;
using System.Reflection;

namespace SolutionInspector.Configuration.Validation.Static
{
  internal interface IStaticConfigurationValidator
  {
    void BeginTypeValidation (Type configurationElementType, ReportValidationError reportValidationError);

    void ValidateValue (
      PropertyInfo property,
      ConfigurationValueAttribute attribute,
      ReportValidationError reportValidationError);

    void ValidateSubelement (
      PropertyInfo property,
      ConfigurationSubelementAttribute attribute,
      ReportValidationError reportValidationError);

    void ValidateCollection (
      PropertyInfo property,
      ConfigurationCollectionAttribute attribute,
      ReportValidationError reportValidationError);

    void EndTypeValidation (Type configurationElementType, ReportValidationError reportValidationError);
  }
}