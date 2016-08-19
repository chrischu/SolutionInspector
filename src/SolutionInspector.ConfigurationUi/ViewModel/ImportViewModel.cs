using System.IO;
using GalaSoft.MvvmLight;

namespace SolutionInspector.ConfigurationUi.ViewModel
{
  internal class ImportViewModel : ViewModelBase
  {
    public ImportViewModel (string path)
    {
      Path = path;
      Type = IsDirectory(path) ? ImportType.Directory : ImportType.File;
    }

    public string Path { get; set; }
    public ImportType Type { get; }

    private bool IsDirectory (string path)
    {
      var attributes = File.GetAttributes(path);
      return attributes.HasFlag(FileAttributes.Directory);
    }
  }
}