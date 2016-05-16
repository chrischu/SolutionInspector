using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Build.Construction;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  /// Represents a property of a <see cref="IProject"/> that is dependent on a <see cref="Condition"/>.
  /// </summary>
  [PublicAPI]
  public interface IConditionalProjectProperty : IProjectProperty
  {
    /// <summary>
    /// The property's condition. This must be true for the property to be active.
    /// </summary>
    string Condition { get; }

    /// <summary>
    /// The parent condition. This must also be true for the property to be active.
    /// </summary>
    string ParentCondition { get; }
  }

  [DebuggerDisplay("{Name} = {Value} (when {Condition} and {ParentCondition})")]
  internal class ConditionalProjectProperty : ProjectProperty, IConditionalProjectProperty
  {
    public string Condition { get; }
    public string ParentCondition { get; }

    public ConditionalProjectProperty(ProjectPropertyElement property) : base(property.Name, property.Value)
    {
      Condition = ConvertCondition(property.Condition);
      ParentCondition = ConvertCondition(property.Parent?.Condition);
    }

    private string ConvertCondition([CanBeNull] string condition)
    {
      return string.IsNullOrWhiteSpace(condition) ? null : condition;
    }
  }
}