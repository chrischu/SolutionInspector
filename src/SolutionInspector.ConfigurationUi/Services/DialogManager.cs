using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace SolutionInspector.ConfigurationUi.Services
{
  internal interface IDialogManager
  {
    string OpenFile (params DialogManager.FileFilter[] filters);
    Task<string> RequestInput (string title, string question, string defaultValue = null);
  }

  internal class DialogManager : IDialogManager
  {
    [CanBeNull]
    public string OpenFile (params FileFilter[] filters)
    {
      var openFileDialog = new OpenFileDialog
                           {
                             Multiselect = false,
                             Filter = string.Join("|", filters.Select(f => f.Filter)),
                             InitialDirectory = Environment.CurrentDirectory
                           };

      return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
    }

    public async Task<string> RequestInput (string title, string message, string defaultValue = null)
    {
      var result = await ((MetroWindow) Application.Current.MainWindow).ShowInputAsync(
        title,
        message,
        new MetroDialogSettings { DefaultText = defaultValue });
      return result ?? defaultValue;
    }

    internal class FileFilter
    {
      public static FileFilter All = new FileFilter("All files", "*");

      public FileFilter (string name, params string[] extensions)
      {
        var extensionList = string.Join(";", extensions.Select(e => $"*.{e}"));
        Filter = $"{name} ({extensionList})|{extensionList}";
      }

      public string Filter { get; }
    }
  }
}