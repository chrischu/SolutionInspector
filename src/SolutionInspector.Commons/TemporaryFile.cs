using System;
using System.IO;

namespace SolutionInspector.Commons
{
  /// <summary>
  ///   Utility class that creates a temporary file and deletes it when calling <see cref="Dispose" />.
  /// </summary>
  public sealed class TemporaryFile : IDisposable
  {
    /// <summary>
    ///   Creates a new temporary file path (but does not create an actual physical file yet).
    /// </summary>
    public TemporaryFile ()
    {
      Path = System.IO.Path.GetTempFileName();
    }

    /// <summary>
    ///   The path to the <see cref="TemporaryFile" />.
    /// </summary>
    public string Path { get; }

    /// <summary>
    ///   Get a writeable <see cref="Stream" /> for the file.
    /// </summary>
    /// <returns></returns>
    public Stream GetStream ()
    {
      return new FileStream(Path, FileMode.Open, FileAccess.ReadWrite);
    }

    /// <summary>
    ///   Write the given string to the file.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public void Write (string content)
    {
      File.WriteAllText(Path, content);
    }

    /// <summary>
    ///   Read all file contents as a string.
    /// </summary>
    public string Read ()
    {
      return File.ReadAllText(Path);
    }

    /// <inheritdoc />
    public void Dispose ()
    {
      File.Delete(Path);
    }
  }
}