using System;
using System.Collections.Generic;
using System.Linq;

namespace SolutionInspector.Configuration
{
  public class CommaSeparatedStringCollection : ConfigurationValue<CommaSeparatedStringCollection>
  {
    private List<string> _collection = new List<string>();

    public CommaSeparatedStringCollection (Action<string> updateAction)
      : base(updateAction)
    {
    }

    public CommaSeparatedStringCollection (Action<string> updateAction, IEnumerable<string> elements)
      : base(updateAction)
    {
      AddRange(elements);
    }

    public override string Serialize ()
    {
      return string.Join(",", _collection);
    }

    public override void Deserialize (string serialized)
    {
      var collection = string.IsNullOrEmpty(serialized) ? Enumerable.Empty<string>() : serialized.Split(',');
      _collection = new List<string>(collection);
      Update();
    }

    public IEnumerator<string> GetEnumerator ()
    {
      return _collection.GetEnumerator();
    }

    public void Add (string item)
    {
      AddRange(new[] { item });
    }

    public void Add(params string[] items)
    {
      AddRange(items);
    }

    public void AddRange(IEnumerable<string> items)
    {
      _collection.AddRange(items);
      Update();
    }

    public void Clear ()
    {
      _collection.Clear();
      Update();
    }

    public bool Contains (string item)
    {
      return _collection.Contains(item);
    }

    public bool Remove (string item)
    {
      var removed = _collection.Remove(item);
      if (removed)
        Update();
      return removed;
    }

    public int Count => _collection.Count;
  }
}