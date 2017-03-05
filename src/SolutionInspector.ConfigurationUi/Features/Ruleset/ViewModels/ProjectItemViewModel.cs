using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;

namespace SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels
{
  internal class ProjectItemViewModel : ViewModelBase
  {
    public ProjectItemViewModel (string name, IEnumerable<ProjectItemViewModel> children)
    {
      Name = name;
      Children = children.ToList();
    }

    public string Name { get; }
    public IReadOnlyCollection<ProjectItemViewModel> Children { get; }
  }
}