using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  /// Represents a property of a <see cref="IProject"/> that has different values depending on a condition.
  /// </summary>
  [PublicAPI]
  public interface IConditionalProjectProperty
  {
    /// <summary>
    /// The property's name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The property's values along with their conditions.
    /// </summary>
    IReadOnlyCollection<IConditionalProjectPropertyValue> Values { get; }
  }

 internal class ConditionalProjectProperty : IConditionalProjectProperty
  {
    private readonly List<IConditionalProjectPropertyValue> _values = new List<IConditionalProjectPropertyValue>();

    public string Name { get; }
    public IReadOnlyCollection<IConditionalProjectPropertyValue> Values => _values;

    public ConditionalProjectProperty (string name)
    {
      Name = name;
    }

    public void AddValue(IProjectPropertyCondition condition, string value)
    {
      _values.Add(new ConditionalProjectPropertyValue(condition, value));
    }
  }
}