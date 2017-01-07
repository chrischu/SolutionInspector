using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using JetBrains.Annotations;
using SolutionInspector.ConfigurationUi.Messages;
using SolutionInspector.ConfigurationUi.Services;

namespace SolutionInspector.ConfigurationUi.ViewModel
{
  [UsedImplicitly /* by ViewModelLocator */]
  internal class StartViewModel : ViewModelBase
  {
    private readonly IDialogManager _dialogManager;
    private readonly IWindowManager _windowManager;

    public StartViewModel (IDialogManager dialogManager, IWindowManager windowManager)
    {
      _dialogManager = dialogManager;
      _windowManager = windowManager;
    }

    public ICommand CreateNewConfigurationCommand => new RelayCommand(CreateNewConfiguration);
    public ICommand LoadExistingConfigurationCommand => new RelayCommand(LoadExistingConfiguration);

    private void CreateNewConfiguration ()
    {
    }

    private void LoadExistingConfiguration ()
    {
      var configurationFilePath =
          _dialogManager.OpenFile(new DialogManager.FileFilter(name: "SolutionInspector ruleset files", extensions: "SolutionInspectorRuleset"));
      if (configurationFilePath == null)
        return;

      _windowManager.SwitchTo<MainWindow>();
      MessengerInstance.Send(new LoadConfigurationFileMessage(configurationFilePath));
    }
  }
}