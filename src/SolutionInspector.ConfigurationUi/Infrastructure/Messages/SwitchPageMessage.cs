using System.Diagnostics.CodeAnalysis;
using SolutionInspector.ConfigurationUi.Features.Main;

namespace SolutionInspector.ConfigurationUi.Infrastructure.Messages
{
  [ExcludeFromCodeCoverage]
  internal class SwitchPageMessage
  {
    public SwitchPageMessage (MainViewModel.Page page)
    {
      Page = page;
    }

    public MainViewModel.Page Page { get; }
  }
}