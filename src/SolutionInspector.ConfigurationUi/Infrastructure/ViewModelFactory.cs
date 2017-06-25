using System;
using System.Collections.Generic;
using System.Configuration;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;
using Microsoft.Practices.ServiceLocation;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration;
using SolutionInspector.ConfigurationUi.Features.Controls.Configuration;
using SolutionInspector.ConfigurationUi.Features.Dialogs;
using SolutionInspector.ConfigurationUi.Features.Dialogs.AddRule;
using SolutionInspector.ConfigurationUi.Features.Edit;
using SolutionInspector.ConfigurationUi.Features.Main;
using SolutionInspector.ConfigurationUi.Features.Ruleset;
using SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels;
using SolutionInspector.ConfigurationUi.Features.Start;
using SolutionInspector.ConfigurationUi.Features.Undo;
using SolutionInspector.ConfigurationUi.Infrastructure.AdvancedObservableCollections;
using SolutionInspector.Internals;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Xml.Linq;
using Wrapperator.Wrappers;

namespace SolutionInspector.ConfigurationUi.Infrastructure
{
  /// <summary>
  ///   This class contains static references to all the view models in the
  ///   application and provides an entry point for the bindings.
  /// </summary>
  internal class ViewModelFactory
  {
    private static ViewModelFactory s_instance;
    private static IContainer s_container;

    static ViewModelFactory ()
    {
      if (!ServiceLocator.IsLocationProviderSet)
        RegisterServices();
    }

    public static ViewModelFactory Instance => s_instance ?? (s_instance = new ViewModelFactory());

    private static void RegisterServices ()
    {
      var builder = new ContainerBuilder();

      ////if (ViewModelBase.IsInDesignModeStatic)
      ////{
      ////    // Create design time view services and models
      ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
      ////}
      ////else
      ////{
      ////    // Create run time view services and models
      ////    SimpleIoc.Default.Register<IDataService, DataService>();
      ////}

      builder.RegisterType<StartViewModel>().AsSelf().SingleInstance();
      builder.RegisterType<MainViewModel>().AsSelf().SingleInstance();
      builder.RegisterType<EditViewModel>().AsSelf().SingleInstance();
      builder.RegisterType<TopLevelUndoContext>().As<IUndoContext>().SingleInstance();
      builder.RegisterType<DialogManager>().As<IDialogManager>();
      builder.RegisterType<RuleTypeResolver>().As<IRuleTypeResolver>();
      builder.RegisterType<RulesetLoader>().As<IRulesetLoader>();
      builder.RegisterType<Configuration.ConfigurationManager>().As<IConfigurationManager>();
      builder.RegisterType<RuleConfigurationInstantiator>().As<IRuleConfigurationInstantiator>();
      builder.RegisterType<ConfigurationControlRetriever>().As<IConfigurationControlRetriever>().SingleInstance( /* cache */);
      builder.RegisterType<DocumentationRetriever>().As<IDocumentationRetriever>().SingleInstance( /* cache */);
      builder.RegisterType<RuleRepository>().As<IRuleRepository>();
      builder.RegisterType<FilterEvaluator>().As<IFilterEvaluator>().SingleInstance( /* cache */);
      builder.RegisterType<SolutionLoader>().As<ISolutionLoader>();

      var configuration = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      var solutionInspectorConfiguration = (ISolutionInspectorConfiguration) configuration.GetSectionGroup("solutionInspector").AssertNotNull();
      builder.Register(ctx => solutionInspectorConfiguration.MsBuildParsing).As<IMsBuildParsingConfiguration>();

      builder.Register(ctx => Wrapper.File).As<IFileStatic>();
      builder.Register(ctx => Wrapper.XDocument).As<IXDocumentStatic>();
      builder.Register(ctx => ServiceLocator.Current).As<IServiceLocator>().SingleInstance();

      s_container = builder.Build();
      ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(s_container));
    }

    public MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>();
    public StartViewModel StartViewModel => ServiceLocator.Current.GetInstance<StartViewModel>();
    public EditViewModel EditViewModel => ServiceLocator.Current.GetInstance<EditViewModel>();

    public static AvailableRuleAssemblyViewModel CreateAvailableRuleAssemblyViewModel (
      string name,
      IEnumerable<AvailableRuleViewModel> availableRules)
    {
      return new AvailableRuleAssemblyViewModel(name, availableRules);
    }

    public static AvailableRuleViewModel CreateAvailableRuleViewModel (RuleTypeInfo ruleTypeInfo)
    {
      var documentationRetriever = s_container.Resolve<IDocumentationRetriever>();
      var documentation = documentationRetriever.RetrieveRuleDocumentation(ruleTypeInfo.RuleType);

      return new AvailableRuleViewModel(ruleTypeInfo, documentation);
    }

    public static ImportsViewModel CreateImportsViewModel ()
    {
      return new ImportsViewModel();
    }

    public static ProjectRulesViewModel CreateProjectRulesViewModel (IEnumerable<ProjectRuleGroupViewModel> ruleGroups)
    {
      return new ProjectRulesViewModel(ruleGroups);
    }

    public static ProjectRuleGroupViewModel CreateProjectRuleGroupViewModel (
      ProjectRuleGroupConfigurationElement ruleGroupElement,
      AdvancedObservableCollection<RuleViewModel> rules)
    {
      return new ProjectRuleGroupViewModel(s_container.Resolve<IUndoContext>(), s_container.Resolve<IDialogManager>(), ruleGroupElement, rules);
    }

    public static ProjectItemRulesViewModel CreateProjectItemRulesViewModel (IEnumerable<ProjectItemRuleGroupViewModel> ruleGroups)
    {
      return new ProjectItemRulesViewModel(ruleGroups);
    }

    public static ProjectItemRuleGroupViewModel CreateProjectItemRuleGroupViewModel (
      ProjectItemRuleGroupConfigurationElement ruleGroup,
      AdvancedObservableCollection<RuleViewModel> rules)
    {
      return new ProjectItemRuleGroupViewModel(s_container.Resolve<IUndoContext>(), s_container.Resolve<IDialogManager>(), ruleGroup, rules);
    }

    public static RuleConfigurationPropertyViewModel CreateRuleConfigurationPropertyViewModel (
      string name,
      Type type,
      string documentation,
      ViewModelBase viewModel)
    {
      return new RuleConfigurationPropertyViewModel(name, type, documentation, viewModel);
    }

    public static RuleConfigurationViewModel CreateRuleConfigurationViewModel (
      IEnumerable<RuleConfigurationPropertyViewModel> ruleConfigurationProperties)
    {
      return new RuleConfigurationViewModel(ruleConfigurationProperties);
    }

    public static RulesetViewModel CreateRulesetViewModel (
      RulesetConfigurationDocument rulesetConfigurationDocument,
      ImportsViewModel importsViewModel,
      RulesViewModel rulesViewModel,
      SolutionViewModel solution)
    {
      return new RulesetViewModel(rulesetConfigurationDocument, importsViewModel, rulesViewModel, solution);
    }

    public static RulesViewModel CreateRulesViewModel (
      SolutionRulesViewModel solutionRules,
      ProjectRulesViewModel projectRules,
      ProjectItemRulesViewModel projectItemRules)
    {
      return new RulesViewModel(solutionRules, projectRules, projectItemRules);
    }

    public static RuleViewModel CreateRuleViewModel (
      RuleConfigurationElement ruleConfigurationElement,
      Type ruleType,
      string documentation,
      [CanBeNull] RuleConfigurationViewModel configuration)
    {
      return new RuleViewModel(s_container.Resolve<IFilterEvaluator>(), ruleConfigurationElement, ruleType, documentation, configuration);
    }

    public static SolutionRulesViewModel CreateSolutionRulesViewModel (SolutionRuleGroupViewModel solutionRuleGroupViewModel)
    {
      return new SolutionRulesViewModel(solutionRuleGroupViewModel);
    }

    public static SolutionRuleGroupViewModel CreateSolutionRuleGroupViewModel (AdvancedObservableCollection<RuleViewModel> rules)
    {
      return new SolutionRuleGroupViewModel(
        s_container.Resolve<IUndoContext>(),
        s_container.Resolve<IDialogManager>(),
        rules);
    }
  }
}