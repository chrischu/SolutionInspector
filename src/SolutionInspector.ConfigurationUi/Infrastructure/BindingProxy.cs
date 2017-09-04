using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using JetBrains.Annotations;

namespace SolutionInspector.ConfigurationUi.Infrastructure
{
  [UsedImplicitly /* by InputDialog.ammy */]
  internal class BindingProxy : FrameworkElement
  {
    public static readonly DependencyProperty InProperty;
    public static readonly DependencyProperty OutProperty;

    public BindingProxy()
    {
      Visibility = Visibility.Collapsed;
    }

    static BindingProxy()
    {
      var inMetadata = new FrameworkPropertyMetadata(InPropertyChanged)
                       {
                         BindsTwoWayByDefault = false,
                         DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                       };
      InProperty = DependencyProperty.Register("In", typeof(object), typeof(BindingProxy), inMetadata);

      var outMetadata = new FrameworkPropertyMetadata(OutPropertyChanged){
                          BindsTwoWayByDefault = true,
                          DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        };
      OutProperty = DependencyProperty.Register("Out", typeof(object), typeof(BindingProxy), outMetadata);
    }

    private static void InPropertyChanged (DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      if (BindingOperations.GetBinding(sender, OutProperty) != null)
        ((BindingProxy) sender).Out = args.NewValue;
    }

    private static void OutPropertyChanged (DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var source = DependencyPropertyHelper.GetValueSource(sender, args.Property);

      if (source.BaseValueSource != BaseValueSource.Local)
      {
        var proxy = (BindingProxy)sender;
        var expected = proxy.In;
        if (!ReferenceEquals(args.NewValue, expected))
          Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() => proxy.Out = proxy.In));
      }
    }

    public object In
    {
      get { return GetValue(InProperty); }
      set { SetValue(InProperty, value); }
    }

    public object Out
    {
      get { return GetValue(OutProperty); }
      set { SetValue(OutProperty, value); }
    }
  }
}