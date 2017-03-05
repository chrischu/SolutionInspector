using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Api.Rules;
using SolutionInspector.Configuration;
using SolutionInspector.ConfigurationUi.Features.Controls.Configuration;
using SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels;
using SolutionInspector.ConfigurationUi.Infrastructure;
using SolutionInspector.Internals;

namespace SolutionInspector.ConfigurationUi.Features.Dialogs.AddRule
{
  internal interface IRuleRepository
  {
    IEnumerable<AvailableRuleViewModel> GetAvailableSolutionRules ();
    IEnumerable<AvailableRuleViewModel> GetAvailableProjectRules ();
    IEnumerable<AvailableRuleViewModel> GetAvailableProjectItemRules ();
    RuleViewModel GetRule (AvailableRuleViewModel availableRule);
  }

  internal class RuleRepository : IRuleRepository
  {
    private readonly IServiceLocator _serviceLocator;
    private readonly IRuleTypeResolver _ruleTypeResolver;
    private readonly IConfigurationControlRetriever _configurationControlRetriever;

    public RuleRepository (
      IServiceLocator serviceLocator,
      IRuleTypeResolver ruleTypeResolver,
      IConfigurationControlRetriever configurationControlRetriever)
    {
      _serviceLocator = serviceLocator;
      _ruleTypeResolver = ruleTypeResolver;
      _configurationControlRetriever = configurationControlRetriever;
    }

    public IEnumerable<AvailableRuleViewModel> GetAvailableSolutionRules ()
    {
      return GetAvailableRules(typeof(ISolutionRule));
    }

    public IEnumerable<AvailableRuleViewModel> GetAvailableProjectRules()
    {
      return GetAvailableRules(typeof(IProjectRule));
    }

    public IEnumerable<AvailableRuleViewModel> GetAvailableProjectItemRules()
    {
      return GetAvailableRules(typeof(IProjectItemRule));
    }

    private IEnumerable<AvailableRuleViewModel> GetAvailableRules (Type ruleInterfaceType)
    {
      return AppDomain.CurrentDomain.GetAssemblies()
          .Where(a => !a.IsDynamic)
          .SelectMany(a => a.GetExportedTypes())
          .Where(t => ruleInterfaceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
          .Select(LoadRule);
    }

    public RuleViewModel GetRule (AvailableRuleViewModel availableRule)
    {
      RuleConfigurationElement ruleConfigurationElement;

      if (availableRule.RuleTypeInfo.IsConfigurable)
      {
        var configurationElement = ConfigurationElement.Create("rule", availableRule.RuleTypeInfo.ConfigurationType);
        ruleConfigurationElement = ConfigurationElement.Load<RuleConfigurationElement>(
          configurationElement.Element,
          e => e.RuleType = availableRule.RuleTypeInfo.RuleTypeName);

        var configurationProperties = LoadConfigurationProperties(configurationElement);
        var configurationViewModel = new RuleConfigurationViewModel(configurationProperties);

        return ViewModelFactory.CreateRuleViewModel(
          ruleConfigurationElement,
          availableRule.RuleTypeInfo.RuleType,
          availableRule.Documentation,
          configurationViewModel);
      }

      ruleConfigurationElement = ConfigurationElement.Create<RuleConfigurationElement>("rule");
      ruleConfigurationElement.RuleType = availableRule.RuleTypeInfo.RuleTypeName;

      return ViewModelFactory.CreateRuleViewModel(ruleConfigurationElement, availableRule.RuleTypeInfo.RuleType, availableRule.Documentation, null);
    }

    private AvailableRuleViewModel LoadRule (Type ruleType)
    {
      var ruleTypeInfo = _ruleTypeResolver.Resolve(ruleType);
      return ViewModelFactory.CreateAvailableRuleViewModel(ruleTypeInfo);
    }

    private IEnumerable<RuleConfigurationPropertyViewModel> LoadConfigurationProperties (ConfigurationElement configuration)
    {
      var properties = configuration.GetType().GetProperties().Where(p => p.GetCustomAttribute<ConfigurationPropertyAttribute>() != null);

      foreach (var property in properties)
      {
        var value = property.GetValue(configuration);
        var description = property.GetCustomAttribute<DescriptionAttribute>();

        var configurationControl = _configurationControlRetriever.GetControlFor(value);
        var viewModel = configurationControl.CreateViewModel(value, _serviceLocator);

        yield return new RuleConfigurationPropertyViewModel(property.Name, property.PropertyType, description.Description, viewModel);
      }
    }
  }
}