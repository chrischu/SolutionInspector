using GalaSoft.MvvmLight;

namespace SolutionInspector.ConfigurationUi.Features.Dialogs
{
  internal abstract class DialogViewModelBase : ViewModelBase
  {
    protected DialogViewModelBase(string title)
    {
      Title = title;
    }
    
    public string Title { get; }
  }
}