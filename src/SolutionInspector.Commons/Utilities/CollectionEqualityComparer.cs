using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SolutionInspector.Commons.Utilities
{
  /// <summary>
  ///   Compares two collections for equality ignoring element order.
  /// </summary>
  public class CollectionEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
  {
    private readonly IEqualityComparer<T> _comparer;

    /// <summary>
    ///   Creates a new <see cref="CollectionEqualityComparer{T}"/>.
    /// </summary>
    public CollectionEqualityComparer (IEqualityComparer<T> comparer = null)
    {
      _comparer = comparer ?? EqualityComparer<T>.Default;
    }

    /// <inheritdoc />
    public bool Equals ([CanBeNull] IEnumerable<T> x, [CanBeNull] IEnumerable<T> y)
    {
      var cnt = new Dictionary<T, int>(_comparer);

      if (x != null)
      {
        foreach (var s in x)
        {
          if (cnt.ContainsKey(s))
            cnt[s]++;
          else
            cnt.Add(s, 1);
        }
      }

      if (y != null)
      {
        foreach (var s in y)
        {
          if (cnt.ContainsKey(s))
            cnt[s]--;
          else
            return false;
        }
      }
      return cnt.Values.All(c => c == 0);
    }

    /// <inheritdoc />
    public int GetHashCode (IEnumerable<T> obj)
    {
      return HashCodeHelper.GetOrderIndependentHashCode(obj.Select(x => _comparer.GetHashCode(x)).ToArray());
    }
  }
}