using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Configuration;
using SolutionInspector.ConfigurationUi.Features.Controls.Configuration;
using SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels;
using SolutionInspector.ConfigurationUi.Infrastructure;
using SolutionInspector.Internals;

namespace SolutionInspector.ConfigurationUi.Features.Ruleset
{
  internal class RulesetLoader : IRulesetLoader
  {
    private readonly IServiceLocator _serviceLocator;
    private readonly IConfigurationManager _configurationManager;
    private readonly IConfigurationControlRetriever _configurationControlRetriever;
    private readonly IRuleTypeResolver _ruleTypeResolver;
    private readonly IRuleConfigurationInstantiator _ruleConfigurationInstantiator;
    private readonly IDocumentationRetriever _documentationRetriever;
    private readonly ISolutionLoader _solutionLoader;
    private readonly IMsBuildParsingConfiguration _msBuildParsingConfiguration;

    public RulesetLoader (
      IServiceLocator serviceLocator,
      IConfigurationManager configurationManager,
      IConfigurationControlRetriever configurationControlRetriever,
      IRuleTypeResolver ruleTypeResolver,
      IRuleConfigurationInstantiator ruleConfigurationInstantiator,
      IDocumentationRetriever documentationRetriever,
      ISolutionLoader solutionLoader,
      IMsBuildParsingConfiguration msBuildParsingConfiguration)
    {
      _serviceLocator = serviceLocator;
      _configurationManager = configurationManager;
      _configurationControlRetriever = configurationControlRetriever;
      _ruleTypeResolver = ruleTypeResolver;
      _ruleConfigurationInstantiator = ruleConfigurationInstantiator;
      _documentationRetriever = documentationRetriever;
      _solutionLoader = solutionLoader;
      _msBuildParsingConfiguration = msBuildParsingConfiguration;
    }

    public RulesetViewModel Load (string configurationFilePath)
    {
      var ruleset = LoadRuleset(configurationFilePath);

      var solutionFilePath = Path.ChangeExtension(configurationFilePath, "sln");
      var solution = LoadSolution(solutionFilePath);

      return ViewModelFactory.CreateRulesetViewModel(ruleset, LoadImports(ruleset.RuleAssemblyImports), LoadRules(ruleset.Rules), solution);
    }

    private RulesetConfigurationDocument LoadRuleset(string configurationFilePath)
    {
      return _configurationManager.LoadDocument<RulesetConfigurationDocument>(configurationFilePath);
    }

    private SolutionViewModel LoadSolution(string solutionFilePath)
    {
      var solution = _solutionLoader.Load(solutionFilePath, _msBuildParsingConfiguration);

      var projects = solution.Projects.Select(LoadProject);
      return new SolutionViewModel(solution.Name, projects);
    }

    private ProjectViewModel LoadProject (IProject project)
    {
      var projectItems = project.ProjectItems.Select(LoadProjectItem);
      return new ProjectViewModel(project.Name, project.ProjectFile.Extension, projectItems);
    }

    private ProjectItemViewModel LoadProjectItem (IProjectItem projectItem)
    {
      var children = projectItem.Children.Select(LoadProjectItem);
      return new ProjectItemViewModel(projectItem.Name, children);
    }

    private RulesViewModel LoadRules (RulesConfigurationElement rules)
    {
      var solutionRuleGroup = ViewModelFactory.CreateSolutionRuleGroupViewModel(CreateMirroringObservableCollection(rules.SolutionRules));
      var solutionRules = ViewModelFactory.CreateSolutionRulesViewModel(solutionRuleGroup);

      var projectRuleGroups = MirroringObservableCollection.Create(rules.ProjectRuleGroups, vm => vm.RuleGroup, LoadProjectRuleGroup);
      var projectRules = ViewModelFactory.CreateProjectRulesViewModel(projectRuleGroups);

      var projectItemRuleGroups = MirroringObservableCollection.Create(rules.ProjectItemRuleGroups, vm => vm.RuleGroup, LoadProjectItemRuleGroup);
      var projectItemRules = ViewModelFactory.CreateProjectItemRulesViewModel(projectItemRuleGroups);

      return ViewModelFactory.CreateRulesViewModel(solutionRules, projectRules, projectItemRules);
    }

    private ProjectRuleGroupViewModel LoadProjectRuleGroup (ProjectRuleGroupConfigurationElement ruleGroupElement)
    {
      return ViewModelFactory.CreateProjectRuleGroupViewModel(ruleGroupElement, CreateMirroringObservableCollection(ruleGroupElement.Rules));
    }

    private ProjectItemRuleGroupViewModel LoadProjectItemRuleGroup (ProjectItemRuleGroupConfigurationElement ruleGroupElement)
    {
      return ViewModelFactory.CreateProjectItemRuleGroupViewModel(ruleGroupElement, CreateMirroringObservableCollection(ruleGroupElement.Rules));
    }

    private MirroringObservableCollection<RuleViewModel, RuleConfigurationElement> CreateMirroringObservableCollection (
      IList<RuleConfigurationElement> rules)
    {
      return MirroringObservableCollection.Create(rules, vm => vm.Rule, LoadRule);
    }

    // ReSharper disable once UnusedParameter.Local
    private ImportsViewModel LoadImports (ConfigurationElementCollection<RuleAssemblyImportConfigurationElement> imports)
    {
      return ViewModelFactory.CreateImportsViewModel();
    }

    private RuleViewModel LoadRule (RuleConfigurationElement rule)
    {
      var ruleTypeInfo = _ruleTypeResolver.Resolve(rule.RuleType);

      RuleConfigurationViewModel configurationViewModel = null;

      if (ruleTypeInfo.IsConfigurable)
      {
        var configuration = _ruleConfigurationInstantiator.Instantiate(ruleTypeInfo.ConfigurationType, rule.Element);
        var configurationProperties = LoadConfigurationProperties(configuration);
        configurationViewModel = ViewModelFactory.CreateRuleConfigurationViewModel(configurationProperties);
      }

      return ViewModelFactory.CreateRuleViewModel(
        rule,
        ruleTypeInfo.RuleType,
        _documentationRetriever.RetrieveRuleDocumentation(ruleTypeInfo.RuleType),
        configurationViewModel
      );
    }

    private IEnumerable<RuleConfigurationPropertyViewModel> LoadConfigurationProperties (ConfigurationElement configuration)
    {
      var properties =
          configuration.GetType()
              .GetProperties()
              .Where(p => p.GetCustomAttribute<ConfigurationPropertyAttribute>() != null);

      foreach (var property in properties)
      {
        var value = property.GetValue(configuration);
        var description = property.GetCustomAttribute<DescriptionAttribute>();

        var configurationControl = _configurationControlRetriever.GetControlFor(value);
        var viewModel = configurationControl.CreateViewModel(value, _serviceLocator);
        yield return
            ViewModelFactory.CreateRuleConfigurationPropertyViewModel(
              property.Name,
              property.PropertyType,
              description.Description,
              viewModel);
      }
    }

    
  }
}