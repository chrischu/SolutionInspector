using System;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace SolutionInspector.ConfigurationUi.Services
{
  internal interface ILoadManager
  {
    Task<T> Load<T> (string title, string message, Func<Task<T>> load);
  }

  internal class LoadManager : ILoadManager
  {
    public async Task<T> Load<T> (string title, string message, Func<Task<T>> load)
    {
      var controller = await ((MetroWindow) Application.Current.MainWindow).ShowProgressAsync(title, message);
      controller.SetIndeterminate();

      var result = await Task.Run(load);

      await controller.CloseAsync();
      return result;
    }
  }
}