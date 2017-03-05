using JetBrains.Annotations;
using SolutionInspector.ConfigurationUi.Infrastructure;

namespace SolutionInspector.ConfigurationUi.Features.Start
{
  [UsedImplicitly]
  public partial class StartPage
  {
    public StartPage ()
    {
      InitializeComponent();
      DataContext = ViewModelFactory.Instance.StartViewModel;
    }
  }
}