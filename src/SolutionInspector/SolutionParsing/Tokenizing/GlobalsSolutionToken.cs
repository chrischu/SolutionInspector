using System;

namespace SolutionInspector.SolutionParsing.Tokenizing
{
  internal class GlobalsSolutionToken : SolutionToken
  {
    public GlobalsSolutionToken(string rawValue, int lineNumber) : base(rawValue, lineNumber)
    {
    }
  }
}