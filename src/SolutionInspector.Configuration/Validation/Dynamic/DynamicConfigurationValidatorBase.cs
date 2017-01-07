using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Xml.Linq;
using JetBrains.Annotations;
using SolutionInspector.Configuration.Validation.Static;

namespace SolutionInspector.Configuration.Validation.Dynamic
{
  [ExcludeFromCodeCoverage]
  internal abstract class DynamicConfigurationValidatorBase : IDynamicConfigurationValidator
  {
    public virtual void BeginTypeValidation (Type configurationElementType, XElement element, ReportValidationError reportValidationError)
    {
    }

    public virtual void ValidateValue (
      PropertyInfo property,
      ConfigurationValueAttribute attribute,
      [CanBeNull] XAttribute valueAttribute,
      ReportValidationError reportValidationError)
    {
    }

    public virtual void ValidateSubelement (
      PropertyInfo property,
      ConfigurationSubelementAttribute attribute,
      [CanBeNull] XElement subelement,
      ReportValidationError reportValidationError)
    {
    }

    public virtual void ValidateCollection (
      PropertyInfo property,
      ConfigurationCollectionAttribute attribute,
      [CanBeNull] XElement collectionElement,
      [CanBeNull] IReadOnlyCollection<XElement> collectionItems,
      ReportValidationError reportValidationError)
    {
    }

    public virtual void EndTypeValidation (Type configurationElementType, XElement element, ReportValidationError reportValidationError)
    {
    }
  }
}