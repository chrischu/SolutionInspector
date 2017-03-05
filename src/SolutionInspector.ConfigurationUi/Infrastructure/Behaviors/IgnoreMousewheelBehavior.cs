using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SolutionInspector.ConfigurationUi.Infrastructure.Behaviors
{
  internal class IgnoreMousewheelBehavior : Behavior<UIElement>
  {
    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
    }

    protected override void OnDetaching()
    {
      AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
      base.OnDetaching();
    }

    void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      e.Handled = true;

      var forwardedEvent = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) { RoutedEvent = UIElement.MouseWheelEvent };
      AssociatedObject.RaiseEvent(forwardedEvent);
    }
  }
}
