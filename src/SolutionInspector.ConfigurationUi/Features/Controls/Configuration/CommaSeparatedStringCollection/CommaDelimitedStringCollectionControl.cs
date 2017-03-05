using System;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;
using Microsoft.Practices.ServiceLocation;
using SolutionInspector.Configuration;
using SolutionInspector.ConfigurationUi.Features.Dialogs;
using SolutionInspector.ConfigurationUi.Features.Undo;

namespace SolutionInspector.ConfigurationUi.Features.Controls.Configuration.CommaSeparatedStringCollection
{
  /// <summary>
  ///   Configuration control for <see cref="CommaSeparatedStringCollection" />.
  /// </summary>
  [UsedImplicitly]
  public class CommaDelimitedStringCollectionControl : ConfigurationControlBase
  {
    public override Type ValueType => typeof(SolutionInspector.Configuration.CommaSeparatedStringCollection);

    public override ViewModelBase CreateViewModel (object value, IServiceLocator serviceLocator)
    {
      return new CommaSeparatedStringCollectionViewModel(
        (SolutionInspector.Configuration.CommaSeparatedStringCollection) value,
        serviceLocator.GetInstance<IDialogManager>(),
        serviceLocator.GetInstance<IUndoManager>());
    }
  }
}