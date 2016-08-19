using System;
using System.ComponentModel;
using System.Windows.Input;
using JetBrains.Annotations;

namespace SolutionInspector.ConfigurationUi.Infrastructure
{
  internal sealed class AsyncCommand : ICommand, IDisposable
  {
    private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
    private readonly Func<bool> _canExecute;

    public AsyncCommand (
        Action action,
        Func<bool> canExecute = null,
        Action<object> completed = null,
        Action<Exception> error = null)
    {
      _backgroundWorker.DoWork += (s, e) =>
      {
        CommandManager.InvalidateRequerySuggested();
        action();
      };

      _backgroundWorker.RunWorkerCompleted += (s, e) =>
      {
        if (completed != null && e.Error == null)
          completed(e.Result);

        if (error != null && e.Error != null)
          error(e.Error);

        CommandManager.InvalidateRequerySuggested();
      };

      _canExecute = canExecute;
    }

    public void Cancel ()
    {
      if (_backgroundWorker.IsBusy)
        _backgroundWorker.CancelAsync();
    }

    public bool CanExecute ([CanBeNull] object parameter = null)
    {
      return _canExecute == null
          ? !_backgroundWorker.IsBusy
          : !_backgroundWorker.IsBusy && _canExecute();
    }

    public void Execute ([CanBeNull] object parameter = null)
    {
      _backgroundWorker.RunWorkerAsync();
    }

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    public void Dispose ()
    {
      _backgroundWorker?.Dispose();
    }
  }
}