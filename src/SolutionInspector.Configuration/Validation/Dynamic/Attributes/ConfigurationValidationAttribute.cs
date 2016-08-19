using System;

namespace SolutionInspector.Configuration.Validation.Dynamic.Attributes
{
  [AttributeUsage (AttributeTargets.Property)]
  internal abstract class ConfigurationValidationAttribute : Attribute
  {
  }
}