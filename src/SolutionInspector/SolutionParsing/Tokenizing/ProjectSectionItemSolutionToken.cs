using System;

namespace SolutionInspector.SolutionParsing.Tokenizing
{
  internal class ProjectSectionItemSolutionToken : SolutionToken
  {
    public string Name { get; }

    public ProjectSectionItemSolutionToken (string rawValue, int lineNumber, string name)
      : base(rawValue, lineNumber)
    {
      Name = name;
    }
  }
}