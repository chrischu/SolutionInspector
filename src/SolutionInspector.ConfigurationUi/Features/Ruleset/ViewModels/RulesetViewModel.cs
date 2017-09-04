using GalaSoft.MvvmLight;
using SolutionInspector.Api.Configuration.Ruleset;

namespace SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels
{
  internal class RulesetViewModel : ViewModelBase
  {
    private readonly RulesetConfigurationDocument _ruleset;

    public RulesetViewModel (RulesetConfigurationDocument ruleset, ImportsViewModel imports, RulesViewModel rules, SolutionViewModel solution)
    {
      _ruleset = ruleset;
      Imports = imports;
      Rules = rules;
      Solution = solution;
    }

    public ImportsViewModel Imports { get; }
    public RulesViewModel Rules { get; }
    public SolutionViewModel Solution { get; }

    public void Save (string path)
    {
      _ruleset.Save(path);
    }
  }
}