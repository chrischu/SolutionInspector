using System;
using MahApps.Metro.Controls;

namespace SolutionInspector.ConfigurationUi.Services
{
  internal interface IWindowManager
  {
    void SwitchTo<T>()
        where T : MetroWindow;
  }

  internal class WindowManager : IWindowManager
  {
    public void SwitchTo<T>()
      where T : MetroWindow
    {
      var newWindow = (T)Activator.CreateInstance(typeof(T));

      var previousMainWindow = App.Current.MainWindow;
      App.Current.MainWindow = newWindow;
      previousMainWindow.Close();
      newWindow.Show();
    }
  }
}
