using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  /// Represents the condition that must be fulfilled for the project property to be active.
  /// </summary>
  [PublicAPI]
  public interface IProjectPropertyCondition
  {
    /// <summary>
    /// The property's condition. This must be true for the property to be active.
    /// </summary>
    string Self { get; }

    /// <summary>
    /// The parent condition. This must also be true for the property to be active.
    /// </summary>
    string Parent { get; }
  }

  [DebuggerDisplay("{DebuggerDisplay,nq}")]
  internal class ProjectPropertyCondition : IProjectPropertyCondition
  {
    public string Self { get; }
    public string Parent { get; }

    public ProjectPropertyCondition ([CanBeNull] string self, [CanBeNull] string parent)
    {
      Self = ConvertCondition(self);
      Parent = ConvertCondition(parent);
    }

    private string ConvertCondition([CanBeNull] string condition)
    {
      return string.IsNullOrWhiteSpace(condition) ? null : condition;
    }

    private string DebuggerDisplay
    {
      get
      {
        if(Self == null)
          return Parent;

        if (Parent == null)
          return Self;

        return $"{Parent} AND {Self}";
      }
    }
  }
}