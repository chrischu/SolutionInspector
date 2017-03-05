using System;
using System.IO;

namespace SolutionInspector.Commons
{
  /// <summary>
  ///   Utility class that creates a temporary file and deletes it when calling <see cref="Dispose" />.
  /// </summary>
  public class TemporaryFile : IDisposable
  {
    public TemporaryFile ()
    {
      Path = System.IO.Path.GetTempFileName();
    }

    public Stream GetStream ()
    {
      return new FileStream(Path, FileMode.Open, FileAccess.ReadWrite);
    }

    public string Path { get; }

    public void Dispose ()
    {
      File.Delete(Path);
    }
  }
}