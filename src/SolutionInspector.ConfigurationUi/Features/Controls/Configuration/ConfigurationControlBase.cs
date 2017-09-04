using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.ConfigurationUi.Features.Controls.Configuration
{
  /// <summary>
  ///   Base class for configuration controls.
  /// </summary>
  public abstract class ConfigurationControlBase : IConfigurationControl
  {
    private readonly Lazy<ResourceDictionary> _template;

    protected ConfigurationControlBase ()
    {
      _template = new Lazy<ResourceDictionary>(
        () =>
        {
          var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
          var controlName = GetType().Namespace.AssertNotNull().Split('.').Last();
          return new ResourceDictionary
                 {
                   Source =
                       new Uri(
                         $"pack://application:,,,/{assemblyName};component/Features/Controls/Configuration/{controlName}/{controlName}Template.xaml")
                 };
        });
    }

    public ResourceDictionary Template => _template.Value;

    public abstract Type ValueType { get; }
    public abstract ViewModelBase CreateViewModel (object value, IServiceLocator serviceLocator);
  }
}