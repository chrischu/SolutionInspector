using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace SolutionInspector.ConfigurationUi.Infrastructure.ValidationRules
{
  internal class NoDuplicatesValidationRule<T> : ValidationRule
  {
    private readonly ISet<T> _alreadyUsedValues;

    public NoDuplicatesValidationRule (IEnumerable<T> alreadyUsedValues, IEqualityComparer<T> equalityComparer = null)
    {
      _alreadyUsedValues = new HashSet<T>(alreadyUsedValues, equalityComparer ?? EqualityComparer<T>.Default);
    }

    public override ValidationResult Validate ([CanBeNull] object value, [NotNull] CultureInfo cultureInfo)
    {
      return _alreadyUsedValues.Contains((T) value)
        ? new ValidationResult(false, $"The value '{value}' is used already.")
        : ValidationResult.ValidResult;
    }
  }
}