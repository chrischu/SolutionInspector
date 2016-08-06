using System;
using JetBrains.Annotations;

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
}