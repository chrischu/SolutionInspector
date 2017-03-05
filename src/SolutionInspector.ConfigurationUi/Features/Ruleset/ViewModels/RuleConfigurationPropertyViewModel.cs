using System;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;

namespace SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels
{
  internal class RuleConfigurationPropertyViewModel : ViewModelBase
  {
    public RuleConfigurationPropertyViewModel (string name, Type type, [CanBeNull] string documentation, ViewModelBase valueViewModel)
    {
      Name = name;
      Type = type;
      Documentation = documentation;
      Value = valueViewModel;
    }

    public string Name { get; }
    public Type Type { get; }
    public string Documentation { get; }
    public ViewModelBase Value { get; }
  }
}