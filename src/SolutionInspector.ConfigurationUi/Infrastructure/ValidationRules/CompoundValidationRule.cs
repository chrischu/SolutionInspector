using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace SolutionInspector.ConfigurationUi.Infrastructure.ValidationRules
{
  internal class CompoundValidationRule : ValidationRule
  {
    [CanBeNull]
    public IEnumerable<ValidationRule> ValidationRules { get; set; }

    public override ValidationResult Validate ([CanBeNull] object value, [NotNull] CultureInfo cultureInfo)
    {
      if (ValidationRules != null)
      {
        foreach (var validationRule in ValidationRules)
        {
          var result = validationRule.Validate(value, cultureInfo);
          if (!result.IsValid)
            return result;
        }
      }

      return ValidationResult.ValidResult;
    }
  }
}