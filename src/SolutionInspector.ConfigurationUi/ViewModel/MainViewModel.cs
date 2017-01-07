using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Configuration;
using SolutionInspector.ConfigurationUi.Configuration;
using SolutionInspector.ConfigurationUi.Messages;
using Wrapperator.Wrappers;
using System.Threading.Tasks;

namespace SolutionInspector.ConfigurationUi.ViewModel
{
  [UsedImplicitly /* by ViewModelLocator */]
  internal class MainViewModel : ViewModelBase
  {
    private string _configurationFilePath;
    private IRulesetLoader _rulesetLoader;

    public MainViewModel (IRulesetLoader rulesetLoader)
    {
      _rulesetLoader = rulesetLoader;
      ////if (IsInDesignMode)
      ////{
      ////    // Code runs in Blend --> create design time data.
      ////}
      ////else
      ////{
      ////    // Code runs "for real"
      ////}

      Messenger.Default.Register<LoadConfigurationFileMessage>(this, LoadConfigurationFile);
    }

    public RulesetViewModel Ruleset { get; private set; }

    public ICommand SaveCommand => new RelayCommand(ExecuteSave);

    private void ExecuteSave ()
    {
      Ruleset.Save(_configurationFilePath);
    }

    private async void LoadConfigurationFile (LoadConfigurationFileMessage loadConfigurationFileMessage)
    {
      var controller = await ((MetroWindow) App.Current.MainWindow).ShowProgressAsync("Loading Ruleset…", null);
      controller.SetIndeterminate();

      _configurationFilePath = loadConfigurationFileMessage.ConfigurationFilePath;

      Ruleset = await Task.Run(() => _rulesetLoader.Load(loadConfigurationFileMessage.ConfigurationFilePath));

      await controller.CloseAsync();
    }
  }
}