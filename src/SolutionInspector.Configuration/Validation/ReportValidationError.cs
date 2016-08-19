using System.Reflection;

namespace SolutionInspector.Configuration.Validation
{
  internal delegate void ReportValidationError (PropertyInfo property, string message);
}