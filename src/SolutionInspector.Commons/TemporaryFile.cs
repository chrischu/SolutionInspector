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

    /// <summary>
    ///   Get a writeable <see cref="Stream" /> for the file.
    /// </summary>
    /// <returns></returns>
    public Stream GetStream ()
    {
      return new FileStream(Path, FileMode.Open, FileAccess.ReadWrite);
    }

    /// <summary>
    ///   The path to the <see cref="TemporaryFile" />.
    /// </summary>
    public string Path { get; }

    public void Dispose ()
    {
      File.Delete(Path);
    }
  }
}