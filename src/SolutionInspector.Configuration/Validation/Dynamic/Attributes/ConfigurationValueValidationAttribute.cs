using System.Reflection;
using System.Xml.Linq;
using JetBrains.Annotations;
using SolutionInspector.Configuration.Attributes;

namespace SolutionInspector.Configuration.Validation.Dynamic.Attributes
{
  internal abstract class ConfigurationValueValidationAttribute : ConfigurationValidationAttribute
  {
    public abstract void Validate (
      PropertyInfo property,
      ConfigurationValueAttribute attribute,
      [CanBeNull] XAttribute valueAttribute,
      ReportValidationError reportValidationError);
  }
}