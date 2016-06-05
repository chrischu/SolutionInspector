using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Build.Construction;
using SolutionInspector.Api.Utilities;

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
  internal sealed class ProjectPropertyCondition : IProjectPropertyCondition, IEquatable<ProjectPropertyCondition>
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

    public bool Equals (ProjectPropertyCondition other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return string.Equals(Self, other.Self) && string.Equals(Parent, other.Parent);
    }

    public override bool Equals (object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      return obj is ProjectPropertyCondition && Equals((ProjectPropertyCondition) obj);
    }

    public override int GetHashCode ()
    {
      return HashCodeHelper.GetHashCode(Self, Parent);
    }

    public static bool operator == (ProjectPropertyCondition left, ProjectPropertyCondition right)
    {
      return Equals(left, right);
    }

    public static bool operator != (ProjectPropertyCondition left, ProjectPropertyCondition right)
    {
      return !Equals(left, right);
    }
  }
}