using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace SolutionInspector.ConfigurationUi.Infrastructure.Behaviors
{
  internal class SearchButtonBehavior : Behavior<Button>
  {
    public TextBox SearchBox
    {
      get { return (TextBox) GetValue(SearchBoxProperty); }
      set { SetValue(SearchBoxProperty, value); }
    }

    public static readonly DependencyProperty SearchBoxProperty =
        DependencyProperty.Register("SearchBox", typeof(TextBox), typeof(SearchButtonBehavior), new PropertyMetadata(null));

    protected override void OnAttached ()
    {
      base.OnAttached();
      AssociatedObject.Click += AssociatedObjectOnClick;
    }

    protected override void OnDetaching ()
    {
      AssociatedObject.Click -= AssociatedObjectOnClick;
      base.OnDetaching();
    }

    private void AssociatedObjectOnClick (object sender, RoutedEventArgs routedEventArgs)
    {
      SearchBox.Focus();
      SearchBox.SelectAll();
    }
  }
}