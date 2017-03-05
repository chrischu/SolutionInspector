using SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels;

namespace SolutionInspector.ConfigurationUi.Features.Ruleset
{
  internal interface IRulesetLoader
  {
    RulesetViewModel Load (string configurationFilePath);
  }
}