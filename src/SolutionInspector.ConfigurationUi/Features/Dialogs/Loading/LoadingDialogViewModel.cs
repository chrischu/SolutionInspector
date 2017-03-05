using GalaSoft.MvvmLight;

namespace SolutionInspector.ConfigurationUi.Features.Dialogs.Loading
{
  internal class LoadingDialogViewModel : ViewModelBase
  {
    public LoadingDialogViewModel(string message)
    {
      Message = message;
    }

    public string Message { get; }
  }
}