using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using SolutionInspector.ConfigurationUi.Infrastructure.AdvancedObservableCollections;

namespace SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels
{
  internal class RuleConfigurationViewModel : ViewModelBase
  {
    public RuleConfigurationViewModel (IEnumerable<RuleConfigurationPropertyViewModel> configurationProperties)
    {
      Properties =
          new ReadOnlyObservableCollection<RuleConfigurationPropertyViewModel>(
            new AdvancedObservableCollection<RuleConfigurationPropertyViewModel>(configurationProperties.ToArray()));
    }

    public ReadOnlyObservableCollection<RuleConfigurationPropertyViewModel> Properties { get; }
  }
}