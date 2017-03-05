using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;

namespace SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels
{
  internal class SolutionViewModel : ViewModelBase
  {
    public SolutionViewModel (string name, IEnumerable<ProjectViewModel> projects)
    {
      Name = name;
      Projects = projects.ToList();
    }

    public string Name { get; }
    public IReadOnlyCollection<ProjectViewModel> Projects { get; }
  }
}