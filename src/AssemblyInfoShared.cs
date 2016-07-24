using System;
using System.Reflection;
using NullGuard;

[assembly: AssemblyConfiguration ("Debug")]
[assembly: AssemblyCompany ("chrischu")]
[assembly: AssemblyProduct ("SolutionInspector")]
[assembly: AssemblyTrademark ("")]
[assembly: AssemblyVersion ("0.0.1.0")]
[assembly: AssemblyFileVersion ("0.0.1.0")]
[assembly: AssemblyInformationalVersion ("0.0.0")]

#if DISABLE_IMPLICIT_NULLABILITY
[assembly: NullGuard(ValidationFlags.None)]
#else
[assembly: NullGuard (ValidationFlags.Arguments | ValidationFlags.NonPublic)]
[assembly: AssemblyMetadata ("ImplicitNullability.AppliesTo", "InputParameters, RefParameters")]
#endif