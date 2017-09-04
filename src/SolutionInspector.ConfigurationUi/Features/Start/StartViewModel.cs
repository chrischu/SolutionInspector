using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using SolutionInspector.ConfigurationUi.Infrastructure.Messages;

namespace SolutionInspector.ConfigurationUi.Features.Start
{
  [UsedImplicitly /* by ViewModelFactory */]
  internal class StartViewModel : ViewModelBase
  {
    [UsedImplicitly /* by Databinding */]
    public ICommand CreateNewConfigurationCommand => new RelayCommand(() => MessengerInstance.Send(new CreateNewConfigurationMessage()));

    [UsedImplicitly /* by Databinding */]
    public ICommand LoadExistingConfigurationCommand => new RelayCommand(() => MessengerInstance.Send(new LoadExistingConfigurationMessage()));
  }
}