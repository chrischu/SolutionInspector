using System.Collections.Generic;
using System.Linq;

namespace SolutionInspector.Utilities
{
  /// <summary>
  /// Finds all differences (adds/removes) between two collections.
  /// </summary>
  public interface ICollectionDifferenceFinder
  {
    /// <summary>
    /// Returns a collection of all the differences (adds/removes) <paramref name="c2"/> has compared to <paramref name="c1"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <returns></returns>
    CollectionDifferences<T> FindDifferences<T>(IEnumerable<T> c1, IEnumerable<T> c2);
  }

  /// <inheritdoc />
  public class CollectionDifferenceFinder : ICollectionDifferenceFinder
  {
    /// <inheritdoc />
    public CollectionDifferences<T> FindDifferences<T>(IEnumerable<T> c1, IEnumerable<T> c2)
    {
      var a1 = c1 as T[] ?? c1.ToArray();
      var a2 = c2 as T[] ?? c2.ToArray();

      var adds = a2.Except(a1);
      var removes = a1.Except(a2);

      return new CollectionDifferences<T>(adds, removes);
    } 
  }
}