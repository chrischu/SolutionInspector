using System;
using System.Windows;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;

namespace SolutionInspector.ConfigurationUi.Features.Controls.Configuration
{
  /// <summary>
  ///   Represents a configuration control.
  /// </summary>
  public interface IConfigurationControl
  {
    Type ValueType { get; }

    ResourceDictionary Template { get; }

    ViewModelBase CreateViewModel (object value, IServiceLocator serviceLocator);
  }
}