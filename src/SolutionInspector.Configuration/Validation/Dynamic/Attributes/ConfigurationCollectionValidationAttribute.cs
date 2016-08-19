using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration.Validation.Dynamic.Attributes
{
  internal abstract class ConfigurationCollectionValidationAttribute : ConfigurationValidationAttribute
  {
    public abstract void Validate (
      PropertyInfo property,
      ConfigurationCollectionAttribute attribute,
      [CanBeNull] XElement collectionElement,
      [CanBeNull] IReadOnlyCollection<XElement> collectionItems,
      ReportValidationError reportValidationError);
  }
}