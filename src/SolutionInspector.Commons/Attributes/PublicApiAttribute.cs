using System;
using JetBrains.Annotations;

namespace SolutionInspector.Commons.Attributes
{
  /// <summary>
  ///   This attribute is intended to mark publicly available API which should not be removed and so is treated as used.
  /// </summary>
  [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
  [AttributeUsage(AttributeTargets.All, Inherited = false)]
  public class PublicApiAttribute : Attribute
  {
  }
}