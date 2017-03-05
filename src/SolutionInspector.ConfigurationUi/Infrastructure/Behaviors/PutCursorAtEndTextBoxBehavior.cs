using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace SolutionInspector.ConfigurationUi.Infrastructure.Behaviors
{
  internal class PutCursorAtEndTextBoxBehavior : Behavior<TextBox>
  {
    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.GotFocus += TextBoxGotFocus;
    }

    protected override void OnDetaching()
    {
      AssociatedObject.GotFocus -= TextBoxGotFocus;
      base.OnDetaching();
    }

    private void TextBoxGotFocus(object sender, RoutedEventArgs routedEventArgs)
    {
      AssociatedObject.CaretIndex = AssociatedObject.Text.Length;
    }
  }
}
