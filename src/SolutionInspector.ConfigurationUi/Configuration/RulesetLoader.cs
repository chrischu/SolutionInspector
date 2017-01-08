using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Configuration;
using SolutionInspector.ConfigurationUi.Controls;
using SolutionInspector.ConfigurationUi.Infrastructure;
using SolutionInspector.Internals;

namespace SolutionInspector.ConfigurationUi.Configuration
{
  internal interface IRulesetLoader
  {
    RulesetViewModel Load (string configurationFilePath);
  }

  internal class RulesetLoader : IRulesetLoader
  {
    private readonly IConfigurationManager _configurationManager;
    private readonly IConfigurationControlRetriever _configurationControlRetriever;
    private readonly IRuleTypeResolver _ruleTypeResolver;
    private readonly IRuleConfigurationInstantiator _ruleConfigurationInstantiator;
    private readonly IDocumentationRetriever _documentationRetriever;

    public RulesetLoader (
      IConfigurationManager configurationManager,
      IConfigurationControlRetriever configurationControlRetriever,
      IRuleTypeResolver ruleTypeResolver,
      IRuleConfigurationInstantiator ruleConfigurationInstantiator,
      IDocumentationRetriever documentationRetriever)
    {
      _configurationManager = configurationManager;
      _configurationControlRetriever = configurationControlRetriever;
      _ruleTypeResolver = ruleTypeResolver;
      _ruleConfigurationInstantiator = ruleConfigurationInstantiator;
      _documentationRetriever = documentationRetriever;
    }

    public RulesetViewModel Load (string configurationFilePath)
    {
      var ruleset = LoadConfiguration(configurationFilePath);
      return new RulesetViewModel(ruleset, LoadImports(ruleset.RuleAssemblyImports), LoadRules(ruleset.Rules));
    }

    private RulesViewModel LoadRules (RulesConfigurationElement rules)
    {
      return new RulesViewModel(LoadSolutionRules(rules.SolutionRules));
    }

    private ImportsViewModel LoadImports (ConfigurationElementCollection<RuleAssemblyImportConfigurationElement> imports)
    {
      return new ImportsViewModel();
    }

    private SolutionRuleGroupViewModel LoadSolutionRules (ConfigurationElementCollection<RuleConfigurationElement> solutionRules)
    {
      return new SolutionRuleGroupViewModel(solutionRules.Select(LoadRule));
    }

    private RuleViewModel LoadRule (IRuleConfiguration rule)
    {
      var ruleTypeInfo = _ruleTypeResolver.Resolve(rule.RuleType);

      var configuration = _ruleConfigurationInstantiator.Instantiate(ruleTypeInfo.ConfigurationType, rule.Element);

      var configurationProperties = LoadConfigurationProperties(configuration);

      return new RuleViewModel(
        ruleTypeInfo.RuleType,
        _documentationRetriever.RetrieveRuleDocumentation(ruleTypeInfo.RuleType),
        new RuleConfigurationViewModel(configurationProperties));
    }

    private IEnumerable<RuleConfigurationPropertyViewModel> LoadConfigurationProperties (ConfigurationElement configuration)
    {
      var properties =
          configuration.GetType()
              .GetProperties()
              .Where(p => p.GetCustomAttribute<ConfigurationPropertyAttribute>() != null);

      foreach(var property in properties)
      {
        var value = property.GetValue(configuration);
        var description = property.GetCustomAttribute<DescriptionAttribute>();

        var configurationControl = _configurationControlRetriever.GetControlFor(value);
        var viewModel = configurationControl.CreateViewModel(value);
        yield return
            new RuleConfigurationPropertyViewModel(
              property.Name,
              property.PropertyType,
              description.Description,
              viewModel);
      }
    }

    private RulesetConfigurationDocument LoadConfiguration (string configurationFilePath)
    {
      return _configurationManager.LoadDocument<RulesetConfigurationDocument>(configurationFilePath);
    }
  }

  internal class RulesetViewModel : ViewModelBase
  {
    private readonly RulesetConfigurationDocument _ruleset;

    public RulesetViewModel (RulesetConfigurationDocument ruleset, ImportsViewModel imports, RulesViewModel rules)
    {
      _ruleset = ruleset;
      var element = _ruleset.Rules.SolutionRules.AddNew();
      element.RuleType = "asdf";
      Imports = imports;
      Rules = rules;
    }

    public ImportsViewModel Imports { get; }
    public RulesViewModel Rules { get; }

    public void Save (string path)
    {
    }
  }

  internal class RulesViewModel : ViewModelBase
  {
    public RulesViewModel (SolutionRuleGroupViewModel solutionRules)
    {
      SolutionRules = solutionRules;
    }

    public SolutionRuleGroupViewModel SolutionRules { get; }
  }

  internal class ImportsViewModel : ViewModelBase
  {
  }

  internal class RuleGroupViewModel : ViewModelBase
  {
    public RuleGroupViewModel (IEnumerable<RuleViewModel> rules)
    {
      Rules = new AdvancedObservableCollection<RuleViewModel>(rules);
    }

    public AdvancedObservableCollection<RuleViewModel> Rules { get; }
  }

  internal class SolutionRuleGroupViewModel : RuleGroupViewModel
  {
    public SolutionRuleGroupViewModel (IEnumerable<RuleViewModel> rules)
      : base(rules)
    {
    }
  }

  internal class RuleViewModel : ViewModelBase
  {
    public RuleViewModel (Type type, string documentation, RuleConfigurationViewModel configuration)
    {
      Type = type;
      Documentation = documentation;
      Configuration = configuration;
    }

    public string Name => Type.Name;

    public Type Type { get; }
    public string Documentation { get; }
    public RuleConfigurationViewModel Configuration { get; }
  }

  internal class RuleConfigurationViewModel : ViewModelBase
  {
    public RuleConfigurationViewModel (IEnumerable<RuleConfigurationPropertyViewModel> properties)
    {
      Properties =
          new ReadOnlyObservableCollection<RuleConfigurationPropertyViewModel>(
            new AdvancedObservableCollection<RuleConfigurationPropertyViewModel>(properties));
    }

    public ReadOnlyObservableCollection<RuleConfigurationPropertyViewModel> Properties { get; }
  }

  internal class RuleConfigurationPropertyViewModel : ViewModelBase
  {
    public RuleConfigurationPropertyViewModel (string name, Type type, [CanBeNull] string documentation, ViewModelBase valueViewModel)
    {
      Name = name;
      Type = type;
      Documentation = documentation;
      Value = valueViewModel;
    }

    public string Name { get; }
    public Type Type { get; }
    public string Documentation { get; }
    public ViewModelBase Value { get; }
  }
}