using System;
using JetBrains.Annotations;

namespace SolutionInspector.Commons.Attributes
{
  /// <summary>
  ///   This attribute is intended to mark APIs that are not currently used, but might be used in the future.
  /// </summary>
  [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
  [AttributeUsage(AttributeTargets.All, Inherited = false)]
  public class ForFutureUseAttribute : Attribute
  {
  }
}