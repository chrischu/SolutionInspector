using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace SolutionInspector.SolutionParsing
{
  [Serializable]
  internal class SolutionParsingException : Exception
  {
    public SolutionParsingException (int lineNumber, string message) : base($"Error on line {lineNumber}: {message}.")
    {
    }

    [ExcludeFromCodeCoverage]
    protected SolutionParsingException (SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}