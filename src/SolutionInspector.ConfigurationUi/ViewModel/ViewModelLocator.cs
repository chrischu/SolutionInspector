using System;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Microsoft.Practices.ServiceLocation;
using SolutionInspector.ConfigurationUi.Services;

namespace SolutionInspector.ConfigurationUi.ViewModel
{
  /// <summary>
  ///   This class contains static references to all the view models in the
  ///   application and provides an entry point for the bindings.
  /// </summary>
  internal class ViewModelLocator
  {
    public ViewModelLocator ()
    {
      var builder = new ContainerBuilder();

      ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(builder.Build()));

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

      builder.RegisterType<MainViewModel>().AsSelf();
      builder.RegisterType<StartViewModel>().AsSelf();
      builder.RegisterType<WindowManager>().As<IWindowManager>();
      builder.RegisterType<DialogManager>().As<IDialogManager>();
    }

    public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
    public StartViewModel Start => ServiceLocator.Current.GetInstance<StartViewModel>();

    public static void Cleanup ()
    {
      // TODO Clear the ViewModels
    }
  }
}