using System;
using System.Windows;
using GalaSoft.MvvmLight;

namespace SolutionInspector.ConfigurationUi.Controls
{
  public interface IConfigurationControl
  {
    Type ValueType { get; }

    ResourceDictionary Template { get; }

    ViewModelBase CreateViewModel (object value);
  }
}