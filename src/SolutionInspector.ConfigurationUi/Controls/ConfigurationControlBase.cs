using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using GalaSoft.MvvmLight;

namespace SolutionInspector.ConfigurationUi.Controls
{
  public abstract class ConfigurationControlBase : IConfigurationControl
  {
    private readonly Lazy<ResourceDictionary> _template;

    protected ConfigurationControlBase ()
    {
      _template = new Lazy<ResourceDictionary>(
        () =>
        {
          var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
          var controlName = GetType().Namespace.Split('.').Last();
          return new ResourceDictionary
                 {
                   Source =
                       new Uri(
                         $"pack://application:,,,/{assemblyName};component/Controls/{controlName}/{controlName}Template.xaml")
                 };
        });
    }

    public ResourceDictionary Template => _template.Value;

    public abstract Type ValueType { get; }
    public abstract ViewModelBase CreateViewModel (object value);
  }
}