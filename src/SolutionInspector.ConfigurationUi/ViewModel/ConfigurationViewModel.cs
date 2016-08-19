using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace SolutionInspector.ConfigurationUi.ViewModel
{
  internal class ConfigurationViewModel : ViewModelBase
  {
    public async Task LoadConfigurationFile(string configurationFilePath)
    {
      var controller = await ((MetroWindow)App.Current.MainWindow).ShowProgressAsync("A", "B");
      controller.SetIndeterminate();

      await Task.Delay(3000);

      //RulesetViewModel = await Task.Run(() => _rulesetLoader.Load(loadConfigurationFileMessage.ConfigurationFilePath));

      await controller.CloseAsync();

      //RulesetViewModel =
      //    //new NotifyTaskCompletion<RulesetViewModel>(_rulesetLoader.Load(loadConfigurationFileMessage.ConfigurationFilePath));
      //    new NotifyTaskCompletion<RulesetViewModel>(
      //        _loadManager.Load("Loading…", "Loading ruleset…", () => _rulesetLoader.Load(loadConfigurationFileMessage.ConfigurationFilePath)));
    }
  }
}