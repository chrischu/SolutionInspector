using System.Reflection;
using System.Xml.Linq;
using JetBrains.Annotations;
using SolutionInspector.Configuration.Attributes;

namespace SolutionInspector.Configuration.Validation.Dynamic.Attributes
{
  internal abstract class ConfigurationSubelementValidationAttribute : ConfigurationValidationAttribute
  {
    public abstract void Validate (
      PropertyInfo property,
      ConfigurationSubelementAttribute attribute,
      [CanBeNull] XElement subelement,
      ReportValidationError reportValidationError);
  }
}