using System;
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
    private readonly IWindowManager _windowManager;
    private readonly IDialogManager _dialogManager;

    public StartViewModel (IWindowManager windowManager, IDialogManager dialogManager)
    {
      _windowManager = windowManager;
      _dialogManager = dialogManager;
    }

    public ICommand CreateNewConfigurationCommand => new RelayCommand(CreateNewConfiguration);
    public ICommand LoadExistingConfigurationCommand => new RelayCommand(LoadExistingConfiguration);

    private void CreateNewConfiguration()
    {

    }

    private void LoadExistingConfiguration ()
    {
      var file = _dialogManager.OpenFile(new DialogManager.FileFilter(name: "SolutionInspector configuration files", extensions: "SolutionInspectorConfig"));
      if (file == null)
        return;

      ProcessConfigurationFileAndSwitchToMain(file);
    }

    private void ProcessConfigurationFileAndSwitchToMain(string configurationFilePath)
    {
      MessengerInstance.Send<LoadConfigurationFileMessage, MainViewModel>(new LoadConfigurationFileMessage(configurationFilePath));
      _windowManager.SwitchTo<MainWindow>();
    }
  }
}