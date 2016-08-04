using System;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;
using SolutionInspector.ConfigurationUi.Messages;

namespace SolutionInspector.ConfigurationUi.ViewModel
{
  [UsedImplicitly /* by ViewModelLocator */]
  internal class MainViewModel : ViewModelBase
  {
    public MainViewModel ()
    {
      ////if (IsInDesignMode)
      ////{
      ////    // Code runs in Blend --> create design time data.
      ////}
      ////else
      ////{
      ////    // Code runs "for real"
      ////}

      MessengerInstance.Register<LoadConfigurationFileMessage>(this, LoadConfigurationFile);
    }

    private void LoadConfigurationFile (LoadConfigurationFileMessage loadConfigurationFileMessage)
    {
      throw new NotImplementedException();
    }
  }
}