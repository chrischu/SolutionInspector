using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace SolutionInspector.ConfigurationUi.Features.Dialogs.AddRule
{
  internal class AvailableRuleAssemblyViewModel : ViewModelBase
  {

    public AvailableRuleAssemblyViewModel (string name, IEnumerable<AvailableRuleViewModel> availableRules)
    {
      Name = name;
      AvailableRules = new ObservableCollection<AvailableRuleViewModel>(availableRules);
    }

    public string Name { get; }
    public ObservableCollection<AvailableRuleViewModel> AvailableRules { get; }
  }
}