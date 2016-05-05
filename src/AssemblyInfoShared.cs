using System;
using System.Reflection;
# if !DISABLE_IMPLICIT_NULLABILITY
using NullGuard;

#endif

[assembly: AssemblyConfiguration ("Debug")]
[assembly: AssemblyCompany ("chrischu")]
[assembly: AssemblyProduct ("SolutionInspector")]
[assembly: AssemblyTrademark ("")]
[assembly: AssemblyVersion ("0.0.1.0")]
[assembly: AssemblyFileVersion ("0.0.1.0")]
[assembly: AssemblyInformationalVersion ("0.0.0")]

# if !DISABLE_IMPLICIT_NULLABILITY

[assembly: NullGuard (
#if DEBUG
    ValidationFlags.Arguments | ValidationFlags.NonPublic
#else
  ValidationFlags.Arguments
#endif
    )]
[assembly: AssemblyMetadata ("ImplicitNullability.AppliesTo", "InputParameters, RefParameters")]

#endif