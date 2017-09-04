using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using SolutionInspector.ConfigurationUi.Infrastructure;

namespace SolutionInspector.ConfigurationUi.Features.Dialogs.AddRule
{
  internal class AddRuleDialogViewModel : DialogViewModelBase
  {
    private readonly IFilterEvaluator _filterEvaluator;
    private string _filter;

    public AddRuleDialogViewModel (IFilterEvaluator filterEvaluator, string title, IEnumerable<AvailableRuleViewModel> availableRules) : base(title)
    {
      _filterEvaluator = filterEvaluator;

      var collectionView = CollectionViewSource.GetDefaultView(availableRules);
      collectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(AvailableRuleViewModel.AssemblyName)));
      collectionView.Filter = FilterItem;
      AvailableRules = collectionView;
    }

    private bool FilterItem (object o)
    {
      var availableRule = (AvailableRuleViewModel) o;

      return
          string.IsNullOrEmpty(Filter)
          || _filterEvaluator.MatchesFilter(Filter, availableRule.Name)
          || _filterEvaluator.MatchesFilter(Filter, availableRule.Documentation);
    }

    public ICollectionView AvailableRules { get; }

    public string Filter
    {
      get { return _filter; }
      set
      {
        if (_filter != value)
        {
          _filter = value;
          AvailableRules.Refresh();
        }
      }
    }

    public AvailableRuleViewModel SelectedRule { get; set; }
  }
}