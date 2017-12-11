using System;

namespace SolutionInspector.SolutionParsing.Tokenizing
{
  internal class ProjectEndSolutionToken : SolutionToken
  {
    public ProjectEndSolutionToken (string rawValue, int lineNumber) : base(rawValue, lineNumber)
    {
      
    }
  }
}