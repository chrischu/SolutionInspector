using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using JetBrains.Annotations;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using SolutionInspector.Api.Configuration;
using SolutionInspector.ConfigurationUi.Features.Dialogs.AddRule;
using SolutionInspector.ConfigurationUi.Features.Dialogs.EditGroupFilter.EditProjectRuleGroupFilter;
using SolutionInspector.ConfigurationUi.Features.Dialogs.Input;
using SolutionInspector.ConfigurationUi.Features.Dialogs.Loading;
using SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels;
using SolutionInspector.ConfigurationUi.Features.Undo;
using SolutionInspector.ConfigurationUi.Infrastructure;
using SolutionInspector.ConfigurationUi.Infrastructure.ValidationRules;

namespace SolutionInspector.ConfigurationUi.Features.Dialogs
{
  internal interface IDialogManager
  {
    string OpenFile (string initialDirectory = null, params DialogManager.FileFilter[] filters);

    Task<DialogResult<string>> RequestInput (
      string title,
      string initialValue = null,
      string acceptButtonText = "ACCEPT",
      string cancelButtonText = "CANCEL",
      IEnumerable<ValidationRule> validationRules = null);

    Task<T> Load<T> (string message, Task<T> loadingTask);

    Task<DialogResult<RuleViewModel>> AddSolutionRule ();
    Task<DialogResult<RuleViewModel>> AddProjectRule ();
    Task<DialogResult<RuleViewModel>> AddProjectItemRule ();
    Task<DialogResult<NameFilter>> EditProjectRuleGroupFilter (SolutionViewModel solution, NameFilter appliesTo);
  }

  internal class DialogManager : IDialogManager
  {
    private readonly IRuleRepository _ruleRepository;
    private readonly IFilterEvaluator _filterEvaluator;
    private readonly IUndoContext _undoContext;

    public DialogManager (IRuleRepository ruleRepository, IFilterEvaluator filterEvaluator, IUndoContext undoContext)
    {
      _ruleRepository = ruleRepository;
      _filterEvaluator = filterEvaluator;
      _undoContext = undoContext;
    }

    [CanBeNull]
    public string OpenFile (string initialDirectory = null, params FileFilter[] filters)
    {
      var openFileDialog = new OpenFileDialog
                           {
                             Multiselect = false,
                             Filter = string.Join("|", filters.Select(f => f.Filter)),
                             InitialDirectory = initialDirectory ?? Environment.CurrentDirectory
                           };

      return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
    }

    public Task<DialogResult<string>> RequestInput (
      string title,
      string initialValue = null,
      string acceptButtonText = "ACCEPT",
      string cancelButtonText = "CANCEL",
      IEnumerable<ValidationRule> validationRules = null)
    {
      var vm = new InputDialogViewModel(title)
               {
                 Value = initialValue,
                 AcceptButtonText = acceptButtonText.ToUpper(),
                 CancelButtonText = cancelButtonText.ToUpper(),
                 ValidationRules = validationRules ?? new[] { new NonEmptyValidationRule() }
               };

      var dialog = new InputDialog { DataContext = vm };

      return DialogHost.Show(dialog, dialogIdentifier: "Edit")
          .ContinueWith(t => t.Result != null ? DialogResult<string>.Accept((string) t.Result) : DialogResult<string>.Cancel());
    }

    public async Task<T> Load<T> (string message, Task<T> loadingTask)
    {
      var dialog = new LoadingDialog { DataContext = new LoadingDialogViewModel(message) };

      return await DialogHost.Show(
        dialog,
        dialogIdentifier: "Main",
        openedEventHandler: async (sender, args) =>
        {
          var result = await loadingTask;
          args.Session.Close(result);
        }).ContinueWith(t => (T) t.Result);
    }

    public Task<DialogResult<RuleViewModel>> AddSolutionRule ()
    {
      return AddRule("Add Solution Rule", _ruleRepository.GetAvailableSolutionRules());
    }

    public Task<DialogResult<RuleViewModel>> AddProjectRule ()
    {
      return AddRule("Add Project Rule", _ruleRepository.GetAvailableProjectRules());
    }

    public Task<DialogResult<RuleViewModel>> AddProjectItemRule ()
    {
      return AddRule("Add Project Item Rule", _ruleRepository.GetAvailableProjectItemRules());
    }

    private async Task<DialogResult<RuleViewModel>> AddRule (string dialogTitle, IEnumerable<AvailableRuleViewModel> availableRules)
    {
      var dialog = new AddRuleDialog { DataContext = new AddRuleDialogViewModel(_filterEvaluator, dialogTitle, availableRules) };

      var result = await DialogHost.Show(dialog, dialogIdentifier: "Edit");

      if (result == null)
        return DialogResult<RuleViewModel>.Cancel();

      var availableRuleViewModel = (AvailableRuleViewModel) result;

      return DialogResult<RuleViewModel>.Accept(_ruleRepository.GetRule(availableRuleViewModel));
    }

    public async Task<DialogResult<NameFilter>> EditProjectRuleGroupFilter (SolutionViewModel solution, NameFilter appliesTo)
    {
      var projectRuleGroupFilterViewModel = new ProjectRuleGroupFilterViewModel(solution, appliesTo, _undoContext);
      var dialog = new EditProjectRuleGroupFilterDialog
                   {
                     DataContext =
                         new EditProjectRuleGroupFilterDialogViewModel(
                           "Edit Project Rule Group Filter",
                           projectRuleGroupFilterViewModel)
                   };

      var result = Convert.ToBoolean(await DialogHost.Show(dialog, dialogIdentifier: "Edit"));

      return result ? DialogResult<NameFilter>.Accept(projectRuleGroupFilterViewModel.AppliesTo.GetNameFilter()) : DialogResult<NameFilter>.Cancel();
    }

    public class FileFilter
    {
      [PublicAPI]
      public static FileFilter All = new FileFilter("All files", "*");

      public FileFilter (string name, params string[] extensions)
      {
        var extensionList = string.Join(";", extensions.Select(e => $"*.{e}"));
        Filter = $"{name} ({extensionList})|{extensionList}";
      }

      public string Filter { get; }
    }
  }
}