using System.Collections.Generic;

namespace SolutionInspector.ConfigurationUi.Features.Dialogs.EditGroupFilter
{
  internal class ProjectTreeViewModel : TreeViewModelBase
  {
    public ProjectTreeViewModel (string name, string extension, bool isChecked, IEnumerable<TreeViewModelBase> children)
      : base(name, extension, TreeViewModelType.Project, isChecked, children)
    {
    }
  }
}