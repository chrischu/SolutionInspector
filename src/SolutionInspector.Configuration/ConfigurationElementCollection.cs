using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SolutionInspector.Configuration
{
  public class ConfigurationElementCollection<T> : IReadOnlyCollection<T>
      where T : ConfigurationElement, new()
  {
    private readonly XElement _collectionElement;
    private readonly List<T> _collection = new List<T>();
    private readonly string _elementName;

    public ConfigurationElementCollection (XElement collectionElement, string elementName)
    {
      _collectionElement = collectionElement;
      _collection.AddRange(_collectionElement.Elements().Select(ConfigurationElement.Load<T>));
      _elementName = elementName;
    }

    public T this [int index] =>  _collection[index];

    public T AddNew ()
    {
      var configurationElement = ConfigurationElement.Create<T>(_elementName);
      Add(configurationElement);
      return configurationElement;
    }

    public void Add (T element)
    {
      _collection.Add(element);
      _collectionElement.Add(element.Element);
    }

    public void Remove (T element)
    {
      _collection.Remove(element);
      element.Element.Remove();
    }

    public void Clear ()
    {
      _collection.Clear();
      _collectionElement.RemoveNodes();
    }

    public int Count => _collection.Count;

    public IEnumerator<T> GetEnumerator ()
    {
      return _collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}