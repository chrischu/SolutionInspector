using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using JetBrains.Annotations;
using SolutionInspector.ConfigurationUi.Features.Dialogs;
using SolutionInspector.ConfigurationUi.Features.Main;
using SolutionInspector.ConfigurationUi.Features.Ruleset;
using SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels;
using SolutionInspector.ConfigurationUi.Features.Undo;
using SolutionInspector.ConfigurationUi.Infrastructure.Messages;

namespace SolutionInspector.ConfigurationUi.Features.Edit
{
  [UsedImplicitly /* by ViewModelFactory */]
  internal class EditViewModel : ViewModelBase
  {
    private readonly IDialogManager _dialogManager;
    private readonly IUndoManager _undoManager;
    private readonly IRulesetLoader _rulesetLoader;
    private string _configurationFilePath;

    public EditViewModel (IDialogManager dialogManager, IUndoManager undoManager, IRulesetLoader rulesetLoader)
    {
      _dialogManager = dialogManager;
      _undoManager = undoManager;
      _rulesetLoader = rulesetLoader;

      MessengerInstance.Register<CreateNewConfigurationMessage>(this, msg => CreateNewConfiguration());
      MessengerInstance.Register<LoadExistingConfigurationMessage>(this, msg => LoadExistingConfiguration());
    }

    public RulesetViewModel Ruleset { get; private set; }

    [UsedImplicitly /* by Binding */]
    public ICommand NewCommand => new RelayCommand(CreateNewConfiguration);

    [UsedImplicitly /* by Binding */]
    public ICommand OpenCommand => new RelayCommand(LoadExistingConfiguration);

    [UsedImplicitly /* by Binding */]
    public ICommand CloseCommand => new RelayCommand(() => MessengerInstance.Send(new SwitchPageMessage(MainViewModel.Page.Start)));

    [UsedImplicitly /* by Binding */]
    public ICommand SaveCommand => new RelayCommand(() => Ruleset.Save(_configurationFilePath));

    [UsedImplicitly /* by Binding */]
    public ICommand UndoCommand => new RelayCommand(() => _undoManager.Undo(), () => _undoManager.CanUndo);

    [UsedImplicitly /* by Binding */]
    public ICommand RedoCommand => new RelayCommand(() => _undoManager.Redo(), () => _undoManager.CanRedo);

    private void CreateNewConfiguration ()
    {
      // TODO
    }

    private async void LoadExistingConfiguration ()
    {
      var configurationFilePath =
          // TODO: Change to real path
          _dialogManager.OpenFile(
            @"D:\Development\SolutionInspector\src",
            new DialogManager.FileFilter(name: "SolutionInspector ruleset files", extensions: "SolutionInspectorRuleset"));

      if (configurationFilePath == null)
        return;

      _configurationFilePath = configurationFilePath;

      Ruleset = await _dialogManager.Load("Loading configuration file...", Task.Run(() => _rulesetLoader.Load(configurationFilePath)));
      _undoManager.Reset();
      MessengerInstance.Send(new SwitchPageMessage(MainViewModel.Page.Edit));
    }
  }
}