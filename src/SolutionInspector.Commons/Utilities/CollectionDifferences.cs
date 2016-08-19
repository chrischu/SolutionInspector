using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SolutionInspector.Commons.Utilities
{
  /// <summary>
  ///   Container class for all the differences between two collections with elements of type <typeparamref name="T" />.
  /// </summary>
  public class CollectionDifferences<T>
  {
    /// <summary>
    ///   A instance of <see cref="CollectionDifferences{T}" /> that represents no differences.
    /// </summary>
    [PublicAPI]
    public static readonly CollectionDifferences<T> None = new CollectionDifferences<T>(Enumerable.Empty<T>(), Enumerable.Empty<T>());

    /// <summary>
    ///   All the elements that were added.
    /// </summary>
    public IReadOnlyCollection<T> Adds { get; }

    /// <summary>
    ///   All the elements that were removed.
    /// </summary>
    public IReadOnlyCollection<T> Removes { get; }

    /// <summary>
    ///   <c>True</c> if differences were found, <c>false</c> otherwise.
    /// </summary>
    public bool DifferencesFound => Adds.Any() || Removes.Any();

    /// <summary>
    ///   Creates an instance of <see cref="CollectionDifferences{T}" />.
    /// </summary>
    public CollectionDifferences (IEnumerable<T> adds, IEnumerable<T> removes)
    {
      Adds = adds.ToArray();
      Removes = removes.ToArray();
    }
  }
}