using GalaSoft.MvvmLight;
using JetBrains.Annotations;
using SolutionInspector.ConfigurationUi.Infrastructure.Messages;

namespace SolutionInspector.ConfigurationUi.Features.Main
{
  [UsedImplicitly /* by ViewModelFactory */]
  internal class MainViewModel : ViewModelBase
  {
    public MainViewModel ()
    {
      MessengerInstance.Register<SwitchPageMessage>(this, msg => ActivePage = msg.Page);
    }

    public Page ActivePage { get; set; } = Page.Start;
    
    internal enum Page
    {
      Start,
      Edit
    }
  }
}