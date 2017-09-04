using System;
using System.Reflection;
using SolutionInspector.Commons.Attributes;

namespace SolutionInspector.Configuration.Validation.Static
{
  [ForFutureUse]
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