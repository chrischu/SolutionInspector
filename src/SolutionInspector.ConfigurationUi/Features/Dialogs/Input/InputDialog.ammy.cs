using System.Windows.Controls;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.ConfigurationUi.Features.Dialogs.Input
{
  public partial class InputDialog
  {
    public InputDialog()
    {
      InitializeComponent();
      IsVisibleChanged += Dialog_IsVisibleChanged;
    }

    private void Dialog_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
      var isVisible = (bool)e.NewValue;
      if (isVisible)
        ValueInput.GetBindingExpression(TextBox.TextProperty).AssertNotNull().UpdateSource();
    }
  }
}
