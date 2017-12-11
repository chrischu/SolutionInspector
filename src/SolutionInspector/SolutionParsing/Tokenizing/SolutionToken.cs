using System;

namespace SolutionInspector.SolutionParsing.Tokenizing
{
  internal abstract class SolutionToken
  {
    private readonly string _rawValue;

    protected SolutionToken (string rawValue, int lineNumber)
    {
      _rawValue = rawValue;
      LineNumber = lineNumber;
    }

    public int LineNumber { get; }

    public sealed override string ToString ()
    {
      return _rawValue;
    }
  }
}