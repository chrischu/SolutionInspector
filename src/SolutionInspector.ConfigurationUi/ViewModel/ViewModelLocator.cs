using Autofac;
using Autofac.Extras.CommonServiceLocator;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.ServiceLocation;
using SolutionInspector.Configuration;
using SolutionInspector.ConfigurationUi.Configuration;
using SolutionInspector.ConfigurationUi.Controls;
using SolutionInspector.ConfigurationUi.Infrastructure;
using SolutionInspector.ConfigurationUi.Services;
using SolutionInspector.Internals;
using Wrapperator.Interfaces.Configuration;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Xml.Linq;
using Wrapperator.Wrappers;

namespace SolutionInspector.ConfigurationUi.ViewModel
{
  /// <summary>
  ///   This class contains static references to all the view models in the
  ///   application and provides an entry point for the bindings.
  /// </summary>
  internal class ViewModelLocator
  {
    static ViewModelLocator()
    {
      if (!ServiceLocator.IsLocationProviderSet)
        RegisterServices();
    }

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

      //builder.Register(ctx => new MainViewModel(ctx)).AsSelf().SingleInstance();
      builder.RegisterType<StartViewModel>().AsSelf().SingleInstance();
      builder.RegisterType<MainViewModel>().AsSelf().SingleInstance();
      builder.RegisterType<WindowManager>().As<IWindowManager>();
      builder.Register(ctx => DialogCoordinator.Instance).As<IDialogCoordinator>();
      builder.RegisterType<Services.DialogManager>().As<IDialogManager>();
      builder.RegisterType<LoadManager>().As<ILoadManager>();
      builder.RegisterType<RuleTypeResolver>().As<IRuleTypeResolver>();
      builder.RegisterType<RulesetLoader>().As<IRulesetLoader>();
      builder.RegisterType<ConfigurationManager>().As<IConfigurationManager>();
      builder.RegisterType<RuleConfigurationInstantiator>().As<IRuleConfigurationInstantiator>();
      builder.RegisterInstance(new ConfigurationControlRetriever()).As<IConfigurationControlRetriever>().SingleInstance();
      builder.RegisterType<DocumentationRetriever>().As<IDocumentationRetriever>().SingleInstance();
      builder.Register(ctx => Wrapper.File).As<IFileStatic>();
      builder.Register(ctx => Wrapper.ConfigurationManager).As<IConfigurationManagerStatic>();
      builder.Register(ctx => Wrapper.XDocument).As<IXDocumentStatic>();

      var container = builder.Build();
      ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));
    }

    public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
    public StartViewModel Start => ServiceLocator.Current.GetInstance<StartViewModel>();

    public static IDialogManager DialogManager => ServiceLocator.Current.GetInstance<IDialogManager>();

    public static void Cleanup ()
    {
      // TODO Clear the ViewModels
    }
  }
}