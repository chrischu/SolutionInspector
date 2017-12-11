using System;

namespace SolutionInspector.SolutionParsing.Tokenizing
{
  internal class ProjectSectionEndSolutionToken : SolutionToken
  {
    public ProjectSectionEndSolutionToken(string rawValue, int lineNumber) : base(rawValue, lineNumber)
    {

    }
  }
}