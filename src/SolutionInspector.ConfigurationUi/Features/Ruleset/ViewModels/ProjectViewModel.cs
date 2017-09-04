using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;

namespace SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels
{
  internal class ProjectViewModel : ViewModelBase
  {
    public ProjectViewModel(string name, string projectFileExtension, IEnumerable<ProjectItemViewModel> projectItems)
    {
      Name = name;
      ProjectFileExtension = projectFileExtension;
      ProjectItems = projectItems.ToList();
    }

    public string Name { get; }
    public string ProjectFileExtension { get; }
    public IReadOnlyCollection<ProjectItemViewModel> ProjectItems { get; }
  }
}