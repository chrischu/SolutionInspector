using System;
using System.IO;

namespace SolutionInspector.TestInfrastructure
{
  public class TemporaryFile : IDisposable
  {
    public TemporaryFile ()
    {
      Path = System.IO.Path.GetTempFileName();
    }
    public string Path { get; }

    // ReSharper disable once UnusedMember.Global
    public void Write(string content)
    {
      File.WriteAllText(Path, content);
    }

    public string Read()
    {
      return File.ReadAllText(Path);
    }

    public void Dispose ()
    {
      File.Delete(Path);
    }
  }
}
