using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  /// Represents a project property <see cref="Value"/> that is dependent on a <see cref="Condition"/>.
  /// </summary>
  [PublicAPI]
  public interface IConditionalProjectPropertyValue
  {
    /// <summary>
    /// The property value's condition.
    /// </summary>
    IProjectPropertyCondition Condition { get; }

    /// <summary>
    /// The property value.
    /// </summary>
    string Value { get; }
  }

  [DebuggerDisplay("{Condition} => {Value}")]
  internal class ConditionalProjectPropertyValue : IConditionalProjectPropertyValue
  {
    public IProjectPropertyCondition Condition { get; }
    public string Value { get; }

    public ConditionalProjectPropertyValue (IProjectPropertyCondition condition, string value)
    {
      Condition = condition;
      Value = value;
    }
  }
}