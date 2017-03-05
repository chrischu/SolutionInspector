using System.Collections.Generic;
using System.Windows.Controls;
using SolutionInspector.ConfigurationUi.Infrastructure.ValidationRules;

namespace SolutionInspector.ConfigurationUi.Features.Dialogs.Input
{
  internal class InputDialogViewModel : DialogViewModelBase
  {
    public InputDialogViewModel (string title) : base(title)
    {
      ValidationRules = new[] { new NonEmptyValidationRule() };
    }

    public string Value { get; set; } = "";

    public string AcceptButtonText { get; set; } = "ACCEPT";
    public string CancelButtonText { get; set; } = "CANCEL";

    public IEnumerable<ValidationRule> ValidationRules { get; set; }
  }
}