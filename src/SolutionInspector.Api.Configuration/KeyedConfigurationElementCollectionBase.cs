using System.Configuration;
using JetBrains.Annotations;

namespace SolutionInspector.Api.Configuration
{
  /// <summary>
  ///   Base class for <see cref="ConfigurationElementCollection" />s with elements of type <typeparamref name="TElement" /> that are
  ///   uniquely identified by a key of type <typeparamref name="TKey" />.
  /// </summary>
  public abstract class KeyedConfigurationElementCollectionBase<TElement, TKey>
      : ConfigurationElementCollectionBase<TElement>, IKeyedConfigurationCollection<TElement, TKey>
    where TElement : ConfigurationElement, IKeyedConfigurationElement<TKey>, new()
  {
    /// <inheritdoc />
    public bool Contains (TKey key)
    {
      return BaseGet(key) != null;
    }

    /// <inheritdoc />
    public void Remove (TKey key)
    {
      BaseRemove(key);
    }

    /// <inheritdoc />
    protected sealed override object GetElementKey (ConfigurationElement element)
    {
      return GetElementKeyInternal(element);
    }

    private TKey GetElementKeyInternal (ConfigurationElement element)
    {
      return ((TElement) element).Key;
    }

    /// <inheritdoc />
    protected override void BaseAdd (ConfigurationElement element)
    {
      var configurationElement = (TElement) element;

      var key = GetElementKeyInternal(configurationElement);
      if (GetElement(key) != null)
        throw new ConfigurationErrorsException(
          $"The value for the property '{configurationElement.KeyName}' is not valid. " +
          $"The error is: The key '{configurationElement.Key}' was already added to the collection once.");
      base.BaseAdd(element);
    }

    /// <inheritdoc />
    [CanBeNull]
    public TElement GetElement (TKey key)
    {
      return (TElement) BaseGet(key);
    }
  }
}