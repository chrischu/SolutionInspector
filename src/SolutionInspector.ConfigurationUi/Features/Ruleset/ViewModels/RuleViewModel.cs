using System;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.ConfigurationUi.Infrastructure;

namespace SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels
{
  internal class RuleViewModel : ViewModelBase
  {
    private readonly IFilterEvaluator _filterEvaluator;
    private string _globalFilter;
    private string _groupFilter;

    public RuleViewModel (
      IFilterEvaluator filterEvaluator,
      RuleConfigurationElement rule,
      Type ruleType,
      string documentation,
      [CanBeNull] RuleConfigurationViewModel configuration)
    {
      _filterEvaluator = filterEvaluator;
      Rule = rule;
      RuleType = ruleType;
      Documentation = documentation;
      Configuration = configuration;
    }

    public string Name => RuleType.Name;

    public RuleConfigurationElement Rule { get; }
    public Type RuleType { get; }
    public string Documentation { get; }
    public RuleConfigurationViewModel Configuration { get; }

    public string GlobalFilter
    {
      get { return _globalFilter; }
      set
      {
        if (_globalFilter != value)
        {
          _globalFilter = value;
          RaisePropertyChanged(() => IsFiltered);
        }
      }
    }

    public string GroupFilter
    {
      get { return _groupFilter; }
      set
      {
        if (_groupFilter != value)
        {
          _groupFilter = value;
          RaisePropertyChanged(() => IsFiltered);
        }
      }
    }

    public bool IsFiltered => !IsIncludedInFilter;

    private bool IsIncludedInFilter =>
        (string.IsNullOrEmpty(GlobalFilter) || MatchesFilter(GlobalFilter)) &&
        (string.IsNullOrEmpty(GroupFilter) || MatchesFilter(GroupFilter));

    private bool MatchesFilter(string filter) => _filterEvaluator.MatchesFilter(filter, Name) || _filterEvaluator.MatchesFilter(filter, Documentation);
  }
}