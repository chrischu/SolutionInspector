using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Build.Construction;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  /// Represents a property of a <see cref="IProject"/> that has different values depending on a condition.
  /// </summary>
  [PublicAPI]
  public interface IConditionalProjectProperty : IProjectPropertyBase
  {
    /// <summary>
    /// The property's values along with their conditions.
    /// </summary>
    IReadOnlyCollection<IConditionalProjectPropertyValue> Values { get; }
  }

 internal class ConditionalProjectProperty : ProjectPropertyBase, IConditionalProjectProperty
  {
    private readonly List<IConditionalProjectPropertyValue> _values = new List<IConditionalProjectPropertyValue>();

    public IReadOnlyCollection<IConditionalProjectPropertyValue> Values => _values;

    public ConditionalProjectProperty (ProjectPropertyElement property) : base(property)
    {
    }

    public void AddValue(IProjectPropertyCondition condition, string value)
    {
      _values.Add(new ConditionalProjectPropertyValue(condition, value));
    }
  }
}