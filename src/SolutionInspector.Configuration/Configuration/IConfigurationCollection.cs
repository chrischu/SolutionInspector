using System.Collections.Generic;

namespace SolutionInspector.Api.Configuration
{
  public interface ISimpleCollection<T> : IEnumerable<T>
  {
    void Add(T item);
    void Clear();
    bool Contains(T item);
    void Remove(T item);

    int Count { get; }
  }

  public interface IConfigurationCollection<T> : ISimpleCollection<T>
  {
  }

  public interface IKeyedConfigurationCollection<TElement, in TKey> : IConfigurationCollection<TElement>
  {
    bool Contains(TKey key);
    void Remove(TKey key);
  }
}