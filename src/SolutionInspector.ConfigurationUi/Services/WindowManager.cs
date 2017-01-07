using System;
using System.Windows;
using MahApps.Metro.Controls;

namespace SolutionInspector.ConfigurationUi.Services
{
  internal interface IWindowManager
  {
    void SwitchTo<T> ()
      where T : MetroWindow;
  }

  internal class WindowManager : IWindowManager
  {
    public void SwitchTo<T> ()
      where T : MetroWindow
    {
      var newWindow = (T) Activator.CreateInstance(typeof(T));

      var previousMainWindow = Application.Current.MainWindow;
      Application.Current.MainWindow = newWindow;
      previousMainWindow.Close();
      newWindow.Show();
    }
  }
}