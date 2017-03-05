using System.Globalization;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace SolutionInspector.ConfigurationUi.Infrastructure.ValidationRules
{
  internal class NonEmptyValidationRule : ValidationRule
  {
    public override ValidationResult Validate ([CanBeNull] object value, CultureInfo cultureInfo)
    {
      var stringValue = (string) value;
      return string.IsNullOrEmpty(stringValue) ? new ValidationResult(false, "Value is required.") : ValidationResult.ValidResult;
    }
  }
}