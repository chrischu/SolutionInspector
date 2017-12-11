using System;

namespace SolutionInspector.SolutionParsing.Tokenizing
{
  internal class ProjectStartSolutionToken : SolutionToken
  {
    public ProjectStartSolutionToken (string rawValue, int lineNumber, Guid id, string name, string relativePath, Guid type)
      : base(rawValue, lineNumber)
    {
      Id = id;
      Name = name;
      RelativePath = relativePath;
      Type = type;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string RelativePath { get; }
    public Guid Type { get; }
  }
}