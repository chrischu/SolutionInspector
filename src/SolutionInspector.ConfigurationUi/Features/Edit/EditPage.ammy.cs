using JetBrains.Annotations;
using SolutionInspector.ConfigurationUi.Infrastructure;

namespace SolutionInspector.ConfigurationUi.Features.Edit
{
  [UsedImplicitly]
  public partial class EditPage
  {
    public EditPage()
    {
      InitializeComponent();
      DataContext = ViewModelFactory.Instance.EditViewModel;
    }
  }
}
