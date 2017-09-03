using System;
using JetBrains.Annotations;

namespace SolutionInspector.ConfigurationUi.Infrastructure
{
  /// <summary>
  /// Marks a property as used by the view via MVVM.
  /// </summary>
  [MeansImplicitUse(ImplicitUseKindFlags.Access | ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.Itself)]
  public class UsedByViewAttribute : Attribute
  {
  }
}