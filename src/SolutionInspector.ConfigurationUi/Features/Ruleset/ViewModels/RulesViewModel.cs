using GalaSoft.MvvmLight;

namespace SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels
{
  internal class RulesViewModel : ViewModelBase
  {
    private string _globalRulesFilter;

    public RulesViewModel (SolutionRulesViewModel solutionRules, ProjectRulesViewModel projectRules, ProjectItemRulesViewModel projectItemRules)
    {
      SolutionRules = solutionRules;
      ProjectRules = projectRules;
      ProjectItemRules = projectItemRules;
    }

    public SolutionRulesViewModel SolutionRules { get; }
    public ProjectRulesViewModel ProjectRules { get; }
    public ProjectItemRulesViewModel ProjectItemRules { get; }

    public string GlobalRulesFilter
    {
      get { return _globalRulesFilter; }
      set
      {
        if (_globalRulesFilter != value)
        {
          _globalRulesFilter = value;
          SolutionRules.GlobalFilter = value;
          ProjectRules.GlobalFilter = value;
          ProjectItemRules.GlobalFilter = value;
        }
      }
    }
  }
}