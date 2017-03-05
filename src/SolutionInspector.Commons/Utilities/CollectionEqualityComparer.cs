using System.Collections.Generic;
using System.Linq;

namespace SolutionInspector.Commons.Utilities
{
  /// <summary>
  ///   Compares two collections for equality ignoring element order.
  /// </summary>
  public class CollectionEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
  {
    private readonly IEqualityComparer<T> _comparer;

    public CollectionEqualityComparer (IEqualityComparer<T> comparer = null)
    {
      _comparer = comparer ?? EqualityComparer<T>.Default;
    }

    public bool Equals (IEnumerable<T> x, IEnumerable<T> y)
    {
      var cnt = new Dictionary<T, int>(_comparer);
      foreach (var s in x)
      {
        if (cnt.ContainsKey(s))
          cnt[s]++;
        else
          cnt.Add(s, 1);
      }
      foreach (var s in y)
      {
        if (cnt.ContainsKey(s))
          cnt[s]--;
        else
          return false;
      }
      return cnt.Values.All(c => c == 0);
    }

    public int GetHashCode (IEnumerable<T> obj)
    {
      return HashCodeHelper.GetHashCode(obj.Select(x => _comparer.GetHashCode(x)).ToArray());
    }
  }
}