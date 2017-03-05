using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace SolutionInspector.ConfigurationUi.Infrastructure.Behaviors
{
  internal class ClearTextBoxBehavior : Behavior<Button>
  {
    public TextBox TextBox
    {
      get { return (TextBox)GetValue(TextBoxProperty); }
      set { SetValue(TextBoxProperty, value); }
    }

    public static readonly DependencyProperty TextBoxProperty =
        DependencyProperty.Register("TextBox", typeof(TextBox), typeof(ClearTextBoxBehavior), new PropertyMetadata(null));

    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.Click += AssociatedObjectOnClick;
    }

    protected override void OnDetaching()
    {
      AssociatedObject.Click -= AssociatedObjectOnClick;
      base.OnDetaching();
    }

    private void AssociatedObjectOnClick(object sender, RoutedEventArgs routedEventArgs)
    {
      TextBox.Clear();
      TextBox.Focus();
    }
  }
}