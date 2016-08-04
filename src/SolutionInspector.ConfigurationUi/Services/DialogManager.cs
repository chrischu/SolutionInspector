using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace SolutionInspector.ConfigurationUi.Services
{
  internal interface IDialogManager
  {
    string OpenFile (params DialogManager.FileFilter[] filters);
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

    internal class FileFilter
    {
      public static FileFilter All = new FileFilter("All files", "*");

      public FileFilter (string name, params string[] extensions)
      {
        //"SolutionInspector configuration files (*.SolutionInspectorConfig)|*.SolutionInspectorConfig"
        var extensionList = string.Join(";", extensions.Select(e => $"*.{e}"));

        Filter = $"{name} ({extensionList})|{extensionList}";
      }

      public string Filter { get; }
    }
  }
}