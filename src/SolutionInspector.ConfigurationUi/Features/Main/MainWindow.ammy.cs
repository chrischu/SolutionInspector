using JetBrains.Annotations;
using SolutionInspector.ConfigurationUi.Infrastructure;

namespace SolutionInspector.ConfigurationUi.Features.Main
{
  [UsedImplicitly]
  public partial class MainWindow
  {
    public MainWindow()
    {
      InitializeComponent();
      DataContext = ViewModelFactory.Instance.MainViewModel;
    }
  }
}
