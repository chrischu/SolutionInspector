using System;

namespace SolutionInspector.SolutionParsing.Tokenizing
{
  internal class PreambleSolutionToken : SolutionToken
  {
    public PreambleSolutionToken (string rawValue, int lineNumber) : base(rawValue, lineNumber)
    {
    }
  }
}