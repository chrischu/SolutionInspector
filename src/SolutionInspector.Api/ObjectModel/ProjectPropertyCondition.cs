using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Build.Construction;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents the condition that must be fulfilled for the project property to be active.
  /// </summary>
  [PublicAPI]
  public interface IProjectPropertyCondition
  {
    /// <summary>
    ///   The property's condition. This must be true for the property to be active.
    /// </summary>
    [CanBeNull]
    string Self { get; }

    /// <summary>
    ///   The parent condition. This must also be true for the property to be active.
    /// </summary>
    [CanBeNull]
    string Parent { get; }
  }

  [DebuggerDisplay ("{DebuggerDisplay,nq}")]
  internal class ProjectPropertyCondition : IProjectPropertyCondition
  {
    [CanBeNull]
    public string Self { get; }

    [CanBeNull]
    public string Parent { get; }

    public ProjectPropertyCondition ([CanBeNull] string self, [CanBeNull] string parent)
    {
      Self = ConvertCondition(self);
      Parent = ConvertCondition(parent);
    }

    public ProjectPropertyCondition (ProjectPropertyElement element)
        : this(element.Condition, element.Parent?.Condition)
    {
    }

    private string ConvertCondition ([CanBeNull] string condition)
    {
      return string.IsNullOrWhiteSpace(condition) ? null : condition;
    }

    private string DebuggerDisplay
    {
      get
      {
        if (Self == null)
          return Parent;

        if (Parent == null)
          return Self;

        return $"{Parent} AND {Self}";
      }
    }
  }
}