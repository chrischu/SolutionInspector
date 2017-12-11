using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SolutionInspector.SolutionParsing
{
  internal sealed class LineEnumerator : IEnumerator<string>
  {
    private readonly IEnumerator<string> _innerEnumerator;
    private int _lineNumber;

    public LineEnumerator (IEnumerator<string> innerEnumerator)
    {
      _innerEnumerator = innerEnumerator;
    }

    public bool MoveNext ()
    {
      _lineNumber++;
      var moveNextResult = _innerEnumerator.MoveNext();
      ReachedEnd = !moveNextResult;
      return moveNextResult;
    }

    public void Reset ()
    {
      _lineNumber = 0;
      ReachedEnd = false;
      _innerEnumerator.Reset();
    }

    public bool ReachedEnd { get; private set; }

    public string Current => _innerEnumerator.Current;

    public int LineNumber => _lineNumber;

    [ExcludeFromCodeCoverage]
    object IEnumerator.Current => ((IEnumerator) _innerEnumerator).Current;

    public void Dispose ()
    {
      _innerEnumerator.Dispose();
    }
  }
}