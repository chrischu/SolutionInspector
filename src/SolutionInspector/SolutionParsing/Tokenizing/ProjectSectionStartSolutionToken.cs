using System;

namespace SolutionInspector.SolutionParsing.Tokenizing
{
  internal class ProjectSectionStartSolutionToken : SolutionToken
  {
    public ProjectSectionStartSolutionToken(string rawValue, int lineNumber) : base(rawValue, lineNumber)
    {

    }
  }
}