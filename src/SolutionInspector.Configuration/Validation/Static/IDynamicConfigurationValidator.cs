using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using JetBrains.Annotations;
using SolutionInspector.Commons.Attributes;

namespace SolutionInspector.Configuration.Validation.Static
{
  [ForFutureUse]
  internal interface IDynamicConfigurationValidator
  {
    void BeginTypeValidation (Type configurationElementType, XElement element, ReportValidationError reportValidationError);

    void ValidateValue (
      PropertyInfo property,
      ConfigurationValueAttribute attribute,
      [CanBeNull] XAttribute valueAttribute,
      ReportValidationError reportValidationError);

    void ValidateSubelement (
      PropertyInfo property,
      ConfigurationSubelementAttribute attribute,
      [CanBeNull] XElement subelement,
      ReportValidationError reportValidationError);

    void ValidateCollection (
      PropertyInfo property,
      ConfigurationCollectionAttribute attribute,
      [CanBeNull] XElement collectionElement,
      [CanBeNull] IReadOnlyCollection<XElement> collectionItems,
      ReportValidationError reportValidationError);

    void EndTypeValidation (Type configurationElementType, XElement element, ReportValidationError reportValidationError);
  }
}