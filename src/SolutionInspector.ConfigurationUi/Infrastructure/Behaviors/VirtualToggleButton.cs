using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using JetBrains.Annotations;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.ConfigurationUi.Infrastructure.Behaviors
{
  internal static class VirtualToggleButton
  {
    #region attached properties

    #region IsChecked

    public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.RegisterAttached(
          "IsChecked",
          typeof(bool),
          typeof(VirtualToggleButton),
          new FrameworkPropertyMetadata(
            false,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnIsCheckedChanged));

    public static bool GetIsChecked (DependencyObject d)
    {
      return (bool) d.GetValue(IsCheckedProperty).AssertNotNull();
    }

    public static void SetIsChecked (DependencyObject d, bool value)
    {
      d.SetValue(IsCheckedProperty, value);
    }

    private static void OnIsCheckedChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var pseudoButton = d as UIElement;
      if (pseudoButton != null)
      {
        var newValue = (bool) e.NewValue;
        if (newValue)
          RaiseCheckedEvent(pseudoButton);
        else
          RaiseUncheckedEvent(pseudoButton);
      }
    }

    #endregion

    #region IsVirtualToggleButton

    public static readonly DependencyProperty IsVirtualToggleButtonProperty =
        DependencyProperty.RegisterAttached(
          "IsVirtualToggleButton",
          typeof(bool),
          typeof(VirtualToggleButton),
          new FrameworkPropertyMetadata(
            false,
            OnIsVirtualToggleButtonChanged));

    public static bool GetIsVirtualToggleButton (DependencyObject d)
    {
      return (bool) d.GetValue(IsVirtualToggleButtonProperty).AssertNotNull();
    }

    public static void SetIsVirtualToggleButton (DependencyObject d, bool value)
    {
      d.SetValue(IsVirtualToggleButtonProperty, value);
    }

    private static void OnIsVirtualToggleButtonChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var element = d as IInputElement;
      if (element != null)
      {
        if ((bool) e.NewValue)
        {
          element.MouseLeftButtonDown += OnMouseLeftButtonDown;
          element.KeyDown += OnKeyDown;
        }
        else
        {
          element.MouseLeftButtonDown -= OnMouseLeftButtonDown;
          element.KeyDown -= OnKeyDown;
        }
      }
    }

    #endregion

    #endregion

    #region Routed Events

    #region Checked

    private static void RaiseCheckedEvent ([CanBeNull] DependencyObject target)
    {
      if (target == null)
        return;

      RaiseEvent(target, new RoutedEventArgs { RoutedEvent = ToggleButton.CheckedEvent });
    }

    #endregion

    #region Unchecked

    private static void RaiseUncheckedEvent ([CanBeNull] DependencyObject target)
    {
      if (target == null)
        return;

      RaiseEvent(target, new RoutedEventArgs { RoutedEvent = ToggleButton.UncheckedEvent });
    }

    #endregion

    #endregion

    #region private methods

    private static void OnMouseLeftButtonDown (object sender, MouseButtonEventArgs e)
    {
      e.Handled = true;
      UpdateIsChecked((DependencyObject) sender);
    }

    private static void OnKeyDown (object sender, KeyEventArgs e)
    {
      var dependencyObject = (DependencyObject) sender;

      if (e.OriginalSource == sender)
      {
        if (e.Key == Key.Space)
        {
          // ignore alt+space which invokes the system menu
          if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
            return;

          UpdateIsChecked(dependencyObject);
          e.Handled = true;
        }
        else
        {
          var acceptsReturn = (bool) dependencyObject.GetValue(KeyboardNavigation.AcceptsReturnProperty).AssertNotNull();
          if (e.Key == Key.Enter && acceptsReturn)
          {
            UpdateIsChecked(dependencyObject);
            e.Handled = true;
          }
        }
      }
    }

    private static void UpdateIsChecked (DependencyObject d)
    {
      SetIsChecked(d, !GetIsChecked(d));
    }

    private static void RaiseEvent (DependencyObject target, RoutedEventArgs args)
    {
      (target as UIElement)?.RaiseEvent(args);
      (target as ContentElement)?.RaiseEvent(args);
    }

    #endregion
  }
}